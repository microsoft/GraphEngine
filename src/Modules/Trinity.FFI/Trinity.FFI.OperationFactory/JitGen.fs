namespace Trinity.FFI.OperationFactory




module JitGen = 
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.FFI.OperationFactory.Operator

    type ManglingCode = char
    type Name = string

    let render  (manglingCode        : ManglingCode)
                (name'maker          : ManglingCode -> TypeDescriptor -> Name) 
                (subject             : TypeDescriptor) 
                (verb                : Verb) : seq<Verb* FunctionDescriptor> = 
        
        match subject with
        | {TypeCode=CELL; Members=members}
        | {TypeCode=STRUCT; Members=members}    ->
           members
           |> Seq.collect (
                fun (member': MemberDescriptor) ->
                    let fieldName   = member'.Name
                    let fnDescMaker = fun verb  -> verb, {DeclaringType=subject; Verb=verb} 
                    [SGet; SSet]
                    |> Seq.map (fun it   -> it fieldName)
                    |> Seq.map fnDescMaker)
        
        | {TypeCode=LIST;ElementType=elemTypes}  ->
             let elemType = Seq.head elemTypes
             [LGet; LSet;  LContains; LCount;]
             |> Seq.map (fun verb -> verb, {DeclaringType=elemType; Verb=verb})
        
        | _                                      -> 
             failwith "Unexpected type descrriptor."