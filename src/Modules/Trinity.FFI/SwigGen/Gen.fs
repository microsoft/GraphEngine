namespace SwigGen

open SwigGen

module SwigTemplates = 
    
    let ListGet = "
    void* {_}List{_}Get{_}{elemName}(void* dataPtr, {elemType} &elem) = {funcPtrAddr};
    void List{_}Get{_}{elemName}(void* dataPtr, {elemType} &elem){{
        {_}List{_}Get{_}{elemName}(dataPtr, elem);
    }}
    "

    let ListSet = "
    void* {_}List{_}Set{m_code}{elemName}(void* dataPtr, {elemType} elem) = {funcPtrAddr};
    void List{_}Set{_}{elemName}(void* dataPtr, {elemType} elem){{
        {_}List{_}Set{_}{elemName}(dataPtr, elem);
    }}
    "
    
    let Contains = "
    void* {_}List{_}Contains{_}{elemName}(void* dataPtr, {elemType} elem) = {funcPtrAddr};
    void List{_}Contains{_}{elemName}(void* dataPtr, {elemType} elem){{
        {_}List{_}Contains{_}{elemName}(dataPtr, elem);
    }}
    "

    let Count = "
    void* {_}List{_}Count{_}{elemName}(void* dataPtr) = {funcPtrAddr};
    void List{_}Count{_}{elemName}(void* dataPtr){{
        {_}List{_}Count{_}{elemName}(dataPtr);
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
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open SwigGen.Command
    open SwigGen.Operator
    
    let m_code = '_'
    let mangling (name: string) = name.Replace(m_code, m_code + m_code)

    type HashSet<'T> = System.Collections.Generic.HashSet<'T>
    type Dict<'K, 'V> = System.Collections.Generic.Dictionary<'K, 'V>

    let make'operations (typeCode) : seq<string> =
        (** constant function, also called by constant **)
        
        match typeCode with 
            | LIST   -> 
                    seq [SwigTemplates.ListGet; 
                         SwigTemplates.ListSet; 
                         SwigTemplates.Contains; 
                         SwigTemplates.Count]
            | _     ->
                    seq [SwigTemplates.FieldGet;
                         SwigTemplates.FieldSet]

    let make'arg'type(typeDesc: TypeDescriptor) : string = 
        match typeDesc.TypeCode with
        | STRUCT
        | CELL // maybe a cell can be used as a field one day
        | LIST -> "void*"
        | _    -> typeDesc.TypeName.ToLower()
    
    let rec make'name (desc: TypeDescriptor) = 
            match desc with
            | {TypeCode=LIST; ElementType=elem} -> 
                    let elemFmt = elem |> Seq.head |> make'name
                    PString.format "List{_}{elem}" (Map["_" ->> m_code; "elem" ->> elemFmt])
            | {TypeCode=CELL}                   ->
                     sprintf "Cell%c" m_code 
            | {TypeCode=STRUCT}                 ->
                     sprintf "Struct%c" m_code
            | _                                 ->
                    desc.TypeName.ToLower()
    
    let define'struct'method  (struct': TypeDescriptor) : seq<string -> string> = seq {
        (** a cell is a struct, too **)
        let rootName = make'name struct'
        for eachMember in struct'.Members do
            let fieldName = mangling eachMember.Name
            for eachOps in make'operations STRUCT do
                yield fun (funcPtrAddr: string) ->
                    PString.format eachOps (
                        Map[
                            "_"           ->> m_code;
                            "rootName"    ->> rootName;
                            "fieldName"   ->> fieldName;
                            "funcPtrAddr" ->> funcPtrAddr;
                            "fieldType"   ->> (make'arg'type eachMember.Type)
                        ])
        }
    
    let define'list'method (typeDesc: TypeDescriptor) : seq<string -> string> = seq{
        let elem = typeDesc.ElementType |> Seq.head
        for eachOps in make'operations LIST do
            yield fun (funcPtrAddr: string) ->
                PString.format eachOps (
                        Map[
                            "_"           ->> m_code;
                            "elemName"    ->> (make'name elem);
                            "funcPtrAddr" ->> funcPtrAddr;
                            "elemType"    ->> (make'arg'type elem)
                        ])
        }
    
    let define'method = function 
        | {TypeCode=LIST} as typeDesc ->  define'list'method typeDesc
        | _               as typeDesc ->  define'struct'method typeDesc
    
    let rec TypeInfer(anyType: TypeDescriptor): seq<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
            | {TypeCode=LIST; ElementType = elemType} 
                 -> anyType >>> TypeInfer (Seq.head elemType)
            | {TypeCode=CELL; Members=members}
            | {TypeCode=STRUCT; Members=members} 
                 -> members
                    |> Seq.map (fun field -> field.Type)
                    |> fun tail -> anyType >>> tail
            | _  -> Seq.empty
    
    type CodeGenerator = (string -> string)[]
   

    let GenerateSrcCode(schema: IStorageSchema): seq<TypeDescriptor * CodeGenerator> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> define'method
                |> Seq.toArray
                |> (<^.^^>) typeDesc)