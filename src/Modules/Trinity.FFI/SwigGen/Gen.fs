namespace SwigGen

open SwigGen

module SwigTemplates = 
    
    let ListGet = "
    void* {_}{ListType}{_}Get(void* dataPtr, int idx, {elemType} &elem) = {funcPtrAddr};
    void {ListType}{_}Get(void* dataPtr, int idx, {elemType} &elem){{
        {ListType}{_}Get(dataPtr, idx, elem);
    }}
    "

    let ListSet = "
    void* {_}{ListType}{_}Set(void* dataPtr, int idx, {elemType} elem) = {funcPtrAddr};
    void {ListType}{_}Set(void* dataPtr, int idx, {elemType} elem){{
        {_}{ListType}{_}Set(dataPtr, idx, elem);
    }}
    "
    
    let Contains = "
    void* {_}{ListType}{_}Contains(void* dataPtr, {elemType} elem) = {funcPtrAddr};
    void {ListType}{_}Contains(void* dataPtr, {elemType} elem){{
        {_}{ListType}{_}Contains(dataPtr, elem);
    }}
    "

    let Count = "
    void* {_}{ListType}{_}Count(void* dataPtr) = {funcPtrAddr};
    void {ListType}{_}Count(void* dataPtr){{
        {_}{ListType}{_}Count(dataPtr);
    }}
    "

    let FieldGet = "
    void* {_}{rootName}{_}Get{_}{fieldName}(void* dataPtr, {{fieldType}} &field) = {funcPtrAddr};
    void {rootName}{_}Get{_}{fieldName}(void* dataPtr, {{fieldType}} &field){{
        {_}{rootName}{_}Get{_}{fieldName}(dataPtr, field);
    }}
    "

    let FieldSet = "
    void* {_}{rootName}{_}Set{_}{fieldName}(void* dataPtr, {{fieldType}} field) = {funcPtrAddr};
    void {rootName}{_}Set{_}{fieldName}(void* dataPtr, {{fieldType}} field){{
        {_}{rootName}{_}Set{_}{fieldName}(dataPtr, field);
    }}
    "

module TGEN = 
    open GraphEngine.Jit
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open SwigGen.Command
    open SwigGen.Operator

    type SwigCode      = string
    type CodeGenerator = (string -> string)

    
    let m_code = '_'
    let mangling (name: string) = name.Replace(m_code, m_code + m_code)
    
    
    let make'verbs (typeCode) : seq<Verb> = 
        raise (NotImplementedException())
        (** 
          maybe implemented as:
          | List   -> seq [ LGet; LSet; LCount; LContains ]
          | Struct -> seq [ SGet; SSet  ]
          | _      -> seq [ BGet; BSet  ]
    
        what's accurately a `Verb`?

        LSet means set values on List or set List values?
        SSet means set values on Struct or set Struct values?

        **)

    let render'swig (verb: Verb): SwigCode = raise (NotImplementedException())
        (**
        match ext with "swig"
        | LSet      -> SwigTemplates.ListSet
        | LGet      -> SwigTemplates.ListGet
        | LCount    -> SwigTemplates.ListCount
        | LContains -> SwigTemplates.ListContains

        | SGet
        | BGet      -> SwigTemplates.FieldGet

        | SSet
        | BSet      -> SwigTemplates.FieldSet
        | _         -> failwiths "NotImplemented"
        **)

    let make'operations' (typeDesc: TypeDescriptor) : seq<Verb * SwigCode> = 
        (**
        a Verb is for asmjit to make jit method.
        a CodeGenerator can makes the swig codes by using a function pointer address.
        **)
        typeDesc.TypeCode
        |> make'verbs
        |> fun verbs ->
                verbs
                |> Seq.map render'swig
                |> Seq.zip verbs
        

    let make'operations (typeCode) : seq<string> = failwith "NotImplemented"
        (** 
            constant function, also called by constant 
        **)
        (**
        TODO:
            This one could be incorrect because I might mix up subjects with objects.
       
        **)

        //match typeCode with 
        //| LIST   -> 
        //        seq [SwigTemplates.ListGet; 
        //             SwigTemplates.ListSet; 
        //             SwigTemplates.Contains; 
        //             SwigTemplates.Count]
        //| _     ->
        //        seq [SwigTemplates.FieldGet;
        //             SwigTemplates.FieldSet]
   

    let make'arg'type(typeDesc: TypeDescriptor) : string = 
        match typeDesc.TypeCode with
        | STRUCT
        | CELL // maybe a cell can be used as a field one day
        | LIST -> "void*"
        | _    -> typeDesc.TypeName.ToLower()
    
    let rec make'name (desc: TypeDescriptor) = 
            match desc with
            | {TypeCode=LIST; ElementType=elemType} -> 
                    let elemTypeName = elemType |> Seq.head |> make'name
                    PString.format "List{_}{elem}" (Map["_" ->> m_code; "elem" ->> elemTypeName])
            | {TypeCode=CELL}                       ->
                     sprintf "Cell%c" m_code 
            | {TypeCode=STRUCT}                     ->
                     sprintf "Struct%c" m_code
            | _                                     ->
                    desc.TypeName.ToLower()
    
    let define'struct'method  (struct': TypeDescriptor) : seq<Verb * CodeGenerator> = seq {
        (** a cell is a struct, too **)
        let rootName = make'name struct'
        for eachMember in struct'.Members do
            let fieldName = mangling eachMember.Name
            for verb, swigCode in make'operations' struct' do
                let codeGenerator (funcPtrAddr: string)  = 
                     PString.format swigCode (
                        Map[
                            "_"           ->> m_code;
                            "rootName"    ->> rootName;
                            "fieldName"   ->> fieldName;
                            "funcPtrAddr" ->> funcPtrAddr;
                            "fieldType"   ->> (make'arg'type eachMember.Type)
                        ])
                yield (verb, codeGenerator)
        }
    
    let define'list'method (list': TypeDescriptor) : seq<Verb * CodeGenerator> = seq{
        let elem = list'.ElementType |> Seq.head
        for verb, swigCode in make'operations' list' do
            let codeGenerator (funcPtrAddr: string) =
                PString.format swigCode (
                    Map[
                        "_"           ->> m_code;
                        "elemName"    ->> (make'name elem);
                        "funcPtrAddr" ->> funcPtrAddr;
                        "elemType"    ->> (make'arg'type elem)
                    ])
            yield (verb, codeGenerator)
        }
    
    let define'method = function 
        | {TypeCode=LIST} as typeDesc ->  define'list'method typeDesc
        | _               as typeDesc ->  define'struct'method typeDesc
    
    let rec TypeInfer(anyType: TypeDescriptor): seq<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
            | {TypeCode=LIST; ElementType = elemType}  -> 
                    anyType >>> TypeInfer (Seq.head elemType)

            | {TypeCode=CELL; Members=members}
            | {TypeCode=STRUCT; Members=members}      -> 
                    members
                    |> Seq.map (fun field -> field.Type)
                    |> fun tail -> anyType >>> tail
            | _                                       -> 
                    Seq.empty
   

    let GenerateSrcCode(schema: IStorageSchema): seq<TypeDescriptor * array<Verb * CodeGenerator>> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> define'method
                |> Seq.toArray
                |> fun methods -> (typeDesc, methods))