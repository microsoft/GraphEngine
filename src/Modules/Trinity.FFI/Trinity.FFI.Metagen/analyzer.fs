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
    let rec recur (tydesc: TypeDescriptor) : Verb list = 
        match find tb tydesc.QualifiedName with
        | Some v -> v 
        | _      ->
        let chaining_methods = 
            match tydesc with 
            | {TypeCode = LIST; ElementType = elem} ->
                let subs = recur <| Seq.head elem in
                LSet     ::
                LContains::
                LCount   ::
                LInsertAt::
                LRemoveAt::
                LAppend  ::
                LGet     :: [for sub_verb in subs -> ComposedVerb(LGet, sub_verb)]
            | {TypeCode = CELL _; Members = membs}
            | {TypeCode = STRUCT; Members = membs} ->
                membs |> List.ofSeq 
                        |> List.collect(
                        fun memb ->
                            let field = memb.Name 
                            in
                            let subs  = recur memb.Type 
                            in
                            let sget  = SGet field 
                            in
                            SSet field::sget::
                                [for sub_verb in subs 
                                    -> ComposedVerb(sget, sub_verb)])
            | _ -> []
        in 
        let result = BGet::BSet::chaining_methods in 
        tb.[tydesc.QualifiedName] <- result 
        result 
    in 
    tydescs 
    |> Seq.map(fun each ->
            (each, BNew :: recur each))
    |> List.ofSeq





        
        

        


        
    



