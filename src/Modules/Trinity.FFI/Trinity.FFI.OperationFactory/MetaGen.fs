namespace Trinity.FFI.OperationFactory

open Trinity.FFI.OperationFactory

module MetaGen = 
    open GraphEngine.Jit
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open Trinity.FFI.OperationFactory.Operator

    type SwigCode      = string
    type CodeGenerator = (string -> string)

    
    let mangling (manglingCode: char) (name: string) = name.Replace(manglingCode, manglingCode + manglingCode)
         
    let rec make'name (manglingCode: char) (desc: TypeDescriptor) = 
        (** transform a typedescriptor into a name signature 
            
            eg. List<List<int>> ->
                List_List_int (if `_` is the mangling code
                
                List<My_Struct> ->
                List_My__Struct(if `_` is the mangling code
        **)
        let m_mangling = mangling manglingCode
        
        match desc with
        | {TypeCode=LIST; ElementType=elemType}  -> 
                let elemTypeName = elemType |> Seq.head |> (make'name manglingCode)
                PString.format "List{_}{elem}" ["_" ->> manglingCode; "elem" ->> elemTypeName]
        | {TypeCode=CELL; TypeName=cellName}     ->
                PString.format "Cell{_}{cellName}" ["_" ->> manglingCode; "cellName" ->> m_mangling cellName]
        | {TypeCode=STRUCT; TypeName=structName} ->
                    PString.format "Struct{_}{structName}" ["_" ->> manglingCode; "structName" ->> m_mangling structName]
        | _                                      ->
                desc.TypeName.ToLower() // primitive type
    
    let render'operations (render : TypeDescriptor -> Verb -> 'T ) (type': TypeDescriptor) : seq<'T> =
        match type' with
        | {TypeCode=CELL; Members=members}
        | {TypeCode=STRUCT; Members=members}    ->
           members
           |> Seq.collect (
                fun (member': MemberDescriptor) ->
                    let fieldName   = member'.Name
                    [SGet; SSet]
                    |> Seq.map (fun verbMaker -> verbMaker fieldName))
                    |> Seq.map (fun it -> render type' it)
        
        | {TypeCode=LIST;ElementType=elemTypes}  ->
             [LGet; LSet; LContains; LCount;]
             |> Seq.map (fun it -> render type' it)
        | _                                      -> 
             failwith (sprintf "Unexpected type descrriptor (%s)."  (type'.TypeCode.ToString()))            
    
    let rec TypeInfer(anyType: TypeDescriptor): seq<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
        | {TypeCode=LIST; ElementType = elemType} -> 
                anyType >>> TypeInfer (Seq.head elemType)

        | {TypeCode=CELL; Members=members}
        | {TypeCode=STRUCT; Members=members}      -> 
                members
                |> Seq.collect (fun field -> field.Type |> TypeInfer)
                |> fun tail -> anyType >>> tail
        | _                                       -> 
                Seq.empty
   

    let Generate (render : TypeDescriptor -> Verb -> 'T) 
                 (schema : IStorageSchema)
                 : seq<TypeDescriptor * seq<'T>> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> render'operations render
                |> fun methods   -> (typeDesc, methods))
        

    
    open Trinity.FFI.OperationFactory.SwigGen
    open Trinity.FFI.OperationFactory.CSharpGen
    open Trinity.FFI.OperationFactory.JitGen
    let GenerateSwig (manglingChar: char) = SwigGen.render manglingChar make'name |> Generate
    let GenerateCSharp (manglingChar: char) = CSharpGen.render manglingChar make'name |> Generate
    let GenerateJit (manglingChar: char) = JitGen.render manglingChar make'name |> Generate

    let GenerateSwigJit (manglingChar: char) = 
        
        let swig = SwigGen.render manglingChar make'name
        let jit  = JitGen.render manglingChar make'name

        let new'render (type': TypeDescriptor) (verb: Verb) =
            let swig'result = swig type' verb 
            let jit'result  = jit  type' verb
            jit'result, swig'result
        Generate new'render
   
    open Trinity.FFI.OperationFactory.CodeGen
    let CodeGenSwigJit (manglingChar: char) (schema: IStorageSchema)= 
        let f = CodeGen.generateSwigFile manglingChar make'name
        let g = GenerateSwigJit manglingChar schema
        f(g)

        
