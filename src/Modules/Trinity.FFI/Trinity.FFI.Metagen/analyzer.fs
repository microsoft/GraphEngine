
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
        let (is_primitive, tail) = 
            match tydesc with 
            | {TypeCode = LIST; ElementType = elems} ->
                false, elems |> List.ofSeq |> List.collect recur
            | {TypeCode = CELL _; Members = membs }
            | {TypeCode = STRUCT; Members = membs } ->
                false, membs |>  List.ofSeq |> List.collect (member_type >> recur)
            | _ -> true, []
        in if is_primitive then []
           else 
           if tail |> List.contains tydesc 
           then raise (Exception(tydesc.TypeName |> sprintf "found recursive type `%s`."))
           else 
           let result = tydesc :: tail 
           tb.[tydesc.QualifiedName] <- result 
           result 
   in 
   Seq.collect recur tydescs |> List.ofSeq |> List.distinctByRef

let generate_chaining_verb(tydescs: TypeDescriptor seq): (TypeDescriptor * (Verb list)) list = 
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
    let rec recur (tydesc: TypeDescriptor) : Verb list * Verb list = 
        match find tb tydesc.QualifiedName with
        | Some v -> v
        | _      ->
            let (subs, chaining, non_chaining) = 
                match tydesc with 
                | {TypeCode = LIST; ElementType = elem} ->
                    let (l_e, r_e) = recur <| Seq.head elem in 
                    (l_e |> List.append r_e, [LGet], [BNew; BGet; BSet; LGet; LSet; LContains; LCount; LInsertAt; LRemoveAt; LAppend])

                | {TypeCode = CELL _; Members = membs}
                | {TypeCode = STRUCT; Members = membs} ->
                    let (subs, getters, setters) = 
                        membs |> List.ofSeq 
                              |> List.map(fun memb -> 
                                            let field = memb.Name in
                                            let (l_e, r_e) = recur memb.Type
                                            (l_e |> List.append r_e,  SGet field, SSet field))
                              |> List.unzip3
                    in (subs |> List.concat, getters, List.concat [[BNew; BGet; BSet;]; getters; setters])
                    
                | _ -> ([], [], [BGet])
            in
            let chained = [
                for l in subs do
                for r in chaining -> 
                    ComposedVerb(l, r)
                ]
            in 
            let result = (chained, non_chaining) in 
            tb.[tydesc.QualifiedName] <- result 
            result 
    in 
    tydescs 
    |> Seq.map(fun each ->
            let (l, r) = recur each in 
            (each, List.append l r))
    |> List.ofSeq
                            


    

            
            
        



        
        

        


        
    



