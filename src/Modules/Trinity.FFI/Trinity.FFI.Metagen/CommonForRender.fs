namespace Trinity.FFI.Metagen
open GraphEngine.Jit.Verbs
open GraphEngine.Jit.TypeSystem
open Trinity.FFI.Metagen.Operator
module CommonForRender = 
    
    
    let rec TypeName (t: TypeDescriptor) = 
        match t.TypeCode with
        | LIST   -> sprintf "List<%s>" (t.ElementType |> Seq.head |> TypeName)
        | _      -> t.TypeName
        
    let getMemberTypeFromSubject  (memberName: string) (subject: TypeDescriptor) =
        subject.Members
        |> Seq.filter (fun it -> it.Name.Equals memberName)
        |> Seq.head
        |> fun it -> it.Type
     
    let getElemTypeFromSubject (subject: TypeDescriptor) = 
        subject.ElementType |> Seq.head

    let getObjectFromSubjectAndVerb (subject: TypeDescriptor) (verb: Verb) =
        if 
            subject.TypeCode = LIST
        then
            subject |> getElemTypeFromSubject  
        else
            match verb with
            | SGet fieldName
            | SSet fieldName ->
                subject |> getMemberTypeFromSubject fieldName
            | _             -> failwith "Impossible."