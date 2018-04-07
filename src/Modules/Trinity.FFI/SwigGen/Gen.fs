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
          | LIST   -> seq [ LGet; LSet; LCount; LContains ]
          | CELL
          | Struct -> seq [ SGet; SSet  ]
          | _      -> seq [ BGet; BSet  ]

        **)

    let render'swig (verb: Verb): SwigCode = raise (NotImplementedException())
        (**
        match verb with
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

    let make'operations (typeCode: TypeSystem.TypeCode) : seq<Verb * SwigCode> = 
        (**
        a Verb is for asmjit to make jit method.
        a CodeGenerator can makes the swig codes by using a function pointer address.
        **)
        typeCode
        |> make'verbs
        |> fun verbs ->
                verbs
                |> Seq.map render'swig
                |> Seq.zip verbs
   

    let make'arg'type(typeDesc: TypeDescriptor) : string = 
        match typeDesc.TypeCode with
        | STRUCT
        | CELL // maybe a cell can be used as a field one day
        | LIST -> "void*"

        | _    -> typeDesc.TypeName.ToLower() // primitive type
    
    let rec make'name (desc: TypeDescriptor) = 
            match desc with
            | {TypeCode=LIST; ElementType=elemType}  -> 
                    let elemTypeName = elemType |> Seq.head |> make'name
                    PString.format "List{_}{elem}" (Map["_" ->> m_code; "elem" ->> elemTypeName])
            | {TypeCode=CELL; TypeName=cellName}     ->
                    PString.format "Cell{_}{cellName}" (Map ["_" ->> m_code; "cellName" ->> mangling cellName])  
            | {TypeCode=STRUCT; TypeName=structName} ->
                     PString.format "Struct{_}{structName}" (Map["_" ->> m_code; "structName" ->> mangling structName])
            | _                                      ->
                    desc.TypeName.ToLower() // primitive type
    
    let define'struct'method  (struct': TypeDescriptor) : seq<FunctionDescriptor * CodeGenerator> = seq {
        (** a cell is a struct, too **)
        let rootName = make'name struct'
        let operations = struct'.TypeCode |> make'operations |> Seq.toArray
        for eachMember in struct'.Members do
            let fieldName = mangling eachMember.Name
            for verb, swigCode in operations do
                let fnDesc = {DeclaringType=eachMember.Type; Verb=verb}
                let codeGenerator (funcPtrAddr: string)  = 
                     PString.format swigCode (
                        Map[
                            "_"           ->> m_code;
                            "rootName"    ->> rootName;
                            "fieldName"   ->> fieldName;
                            "funcPtrAddr" ->> funcPtrAddr;
                            "fieldType"   ->> (make'arg'type eachMember.Type)
                        ])
                yield (fnDesc, codeGenerator)
        }
    
    let define'list'method (list': TypeDescriptor) : seq<FunctionDescriptor * CodeGenerator> = seq{
        let elemType = Seq.head list'.ElementType
        for verb, swigCode in make'operations list'.TypeCode do
            let fnDesc = {DeclaringType=elemType; Verb=verb}
            let codeGenerator (funcPtrAddr: string) =
                PString.format swigCode (
                    Map[
                        "_"           ->> m_code;
                        "ListType"    ->> (make'name list');
                        "funcPtrAddr" ->> funcPtrAddr;
                        "elemType"    ->> (elemType |> make'arg'type)
                    ])
            yield (fnDesc, codeGenerator)
        }
    
    let define'method = function 
        | {TypeCode=LIST} as typeDesc ->  define'list'method typeDesc
        | _               as typeDesc ->  define'struct'method typeDesc
    
    let rec TypeInfer(anyType: TypeDescriptor): seq<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
            | {TypeCode=LIST; ElementType = elemType} -> 
                    anyType >>> TypeInfer (Seq.head elemType)

            | {TypeCode=CELL; Members=members}
            | {TypeCode=STRUCT; Members=members}      -> 
                    members
                    |> Seq.map (fun field -> field.Type)
                    |> fun tail -> anyType >>> tail
            | _                                       -> 
                    Seq.empty
   

    let GenerateSrcCode(schema: IStorageSchema): seq<TypeDescriptor * seq<FunctionDescriptor * CodeGenerator>> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> define'method
                |> fun methods   -> (typeDesc, methods))