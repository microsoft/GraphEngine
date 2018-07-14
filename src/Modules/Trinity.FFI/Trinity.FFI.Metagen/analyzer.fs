
module Trinity.FFI.MetaGen.analyzer



open GraphEngine.Jit.TypeSystem
open GraphEngine.Jit.Verbs
open System

open FSharp.NativeInterop
open FSharp.Data

// ml style naming
type ('k, 'v) hashmap = System.Collections.Generic.Dictionary<'k, 'v>

let _distinctByRef (lst: 'a list) = 
(**
for our using case requires nested list comparison by memory address, 
but fsharp list module hasn't implemented it yet. 

and after a simple test our implementation is much more faster.
    
- using a list `List.chunkBySize 50 1000`applying 
both `List.distinct` and `_distinctByRef`, 

```
    benchmark (fun () -> List.distinct chunks |> ignore);;
    val it : float = 40.3861
    benchmark (fun () -> dt chunks |> ignore);;
    val it : float = 0.9669
```
where
```
let benchmark (test : unit -> unit) =
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    for i = 1 to 1000 do
        test()
    stopWatch.Stop()
    stopWatch.Elapsed.TotalMilliseconds
```
*)


    let rec recur l1 l2 = 
        match l1 with 
        | []      -> l2
        | x :: xs -> 
            if l2 |> List.exists (fun it -> obj.ReferenceEquals(it, x))
            then recur xs l2 
            else recur xs (x::l2) 
    in recur lst []

type 'a List with 
    static member distinctByRef(this: 'a List) = _distinctByRef this
        


let find (tb: ('k, 'v) hashmap) (k: 'k) : 'v option =
         if tb.ContainsKey(k) 
         then Some(tb.[k])
         else None

let member_type mem = mem.Type 

    
let collect_type (tydescs: TypeDescriptor seq): TypeDescriptor list =
   let tb = hashmap() in
   let rec recur tydesc =
       match find tb tydesc.QualifiedName with 
       | Some v -> v 
       | _ ->
        let tail = 
            match tydesc with 
            | {TypeCode = LIST; ElementType = elems} ->
                elems |> List.ofSeq |> List.collect recur
            | {TypeCode = CELL _; Members = membs }
            | {TypeCode = STRUCT; Members = membs } ->
                membs |>  List.ofSeq |> List.collect (member_type >> recur)
            | _ -> []
        in if tail |> List.contains tydesc 
            then raise (Exception(tydesc.TypeName |> sprintf "found recursive type `%s`."))
            else 
            let result = tydesc :: tail 
            tb.[tydesc.QualifiedName] <- result 
            result 
   in 
   Seq.collect recur tydescs |> List.ofSeq |> List.distinctByRef

   
open System.IO


let calc_chaining (tydescs: TypeDescriptor seq): TypeDescriptor list list = 
    (** 
    calculate out all the chains of type owning relationship.
    e.g
    ```
        cell C{
            S s;
            int b;
        }
        struct S{
            int a;
            int c;
        }
    ```
    then we find out all the chains which reach the final type(primitive type) as following:
    ```
        [S, int]
        [S, C]
        [C, S, int]
        [C, S, int]
        [C, int]
    ```

    *)
    //let tb = hashmap() in
    let rec recur (tydesc: TypeDescriptor) : TypeDescriptor list list = 
        //match find tb tydesc.QualifiedName with 
        //| Some v -> v 
        //| _      ->
            let chain = 
                match tydesc with 
                | {TypeCode = CELL _; Members = membs}
                | {TypeCode = STRUCT; Members = membs} ->
                        membs |> List.ofSeq 
                              |> List.collect (member_type >> recur) 
                              |> List.map (fun it -> tydesc :: it)
                | {TypeCode = LIST ; ElementType = elems} ->
                        elems |> List.ofSeq 
                              |> List.collect recur 
                              |> List.map (fun it -> tydesc :: it)
                | _ -> []
            in 
            
            File.WriteAllText("../../log.txt", chain.ToString() + tydesc.TypeName)
            chain 
            //tb.[tydesc.QualifiedName] <- chain
            //chain 

    in 
    //for each in tydescs do 
    //      recur(each) |> ignore 
    
    List.collect recur << List.ofSeq <| tydescs

    //tb.Values |> Seq.collect id |> List.ofSeq |> List.distinct


let generate_chaining_verb(tydesc_chains: TypeDescriptor list seq): Verb list = 
    (** 
    generate the methods for a type owning relationship chain:
    e.g
    ```
        cell C{
            S s;
            int b;
        }
        struct S{
            int a;
            int c;
        }
    ```
    for [C, S, int]:
        we generate following methods:
        - for S 

          * BSet
          * BGet

          * SSet "a"
          * SSet "c"

          * ComposedVerb(SGet "a", BGet)
          * ComposedVerb(SGet "c", BGet)

        - for C
          * BSet 
          * BGet

          * SSet "s"
          * SGet "s"
          
          * Composed(SGet "s", Composed(SGet "a", BGet))
          * Composed(SGet "s", SSet "a")
          
          * Composed(SGet "s", Composed(SGet "c", BGet))
          * Composed(SGet "s", SSet "c")

    *)
    let tb = hashmap() in 
    let rec recur tydesc_chain : Verb list * Verb list = 
        match find tb tydesc_chain with
        | Some v -> v
        | _      ->
        match tydesc_chain with 
        | [] ->
            failwith "Impossible"
        | [_] ->
            ([BGet], [])
        | subject :: object_tail ->
            let (l_perspective_chaining, l_no_chaining) = 
                match subject with 
                | {TypeCode = LIST} ->
                    ([LGet], [BNew; BGet; BSet; LGet; LSet; LContains; LCount; LInsertAt; LRemoveAt; LAppend])

                | {TypeCode = CELL _; Members = membs}
                | {TypeCode = STRUCT; Members = membs} ->
                    let (getters, setters) = 
                        membs |> List.ofSeq 
                                |> List.map(fun memb -> 
                                            let field = memb.Name
                                            in (SGet field, SSet field))
                                |> List.unzip
                    in (getters, List.concat [[BNew; BGet; BSet;]; getters; setters])
                    
                | _ -> failwith "Impossible"
            in 
            let (r_perspective_chaining, r_no_chaining) = recur(object_tail) in  
            let left = [
                for l in l_perspective_chaining do
                for r in r_perspective_chaining -> 
                    ComposedVerb(l, r)
                ]
            in 
            let right = List.append l_no_chaining r_no_chaining in 
            
            let result = (left, right) in 
            tb.[tydesc_chain] <- result 
            result 
    in 
    tb.Values |> Seq.map (fun (a, b) -> List.append a b) |> Seq.concat |> List.ofSeq

            
            
        



        
        

        


        
    



