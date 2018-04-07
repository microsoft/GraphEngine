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
    
    let make'arg'type(typeDesc: TypeDescriptor) : string = 
        (** 
            transfrom an argument's typedescriptor into a name of supported swig type.
            eg.            
            Int ->
            void xxx(void* data, int arg);
                                 ^
                                  
            List<int> ->
            void xxx(void* data, void* &arg);
                                 ^
         **)
        match typeDesc.TypeCode with
        | STRUCT
        | CELL // maybe a cell can be used as a field one day
        | LIST -> "void*"

        | _    -> typeDesc.TypeName.ToLower() // primitive type
    
    let rec make'name (desc: TypeDescriptor) = 
        (** transform a typedescriptor into a name signature 
            
            eg. List<List<int>> ->
                List_List_int (if `_` is the mangling code
                
                List<My_Struct> ->
                List_My__Struct(if `_` is the mangling code
        **)
        
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
    
    let render'struct'methods (rootType: TypeDescriptor) (memberDesc: MemberDescriptor) : seq<FunctionDescriptor * SwigCode> = seq{
        let memberName = memberDesc.Name
        let memberType = memberDesc.Type
        match rootType.TypeCode with
        | CELL
        | STRUCT  -> 
            yield ({DeclaringType=memberType ; Verb=SGet memberName}, SwigTemplates.FieldGet)
            yield ({DeclaringType=memberType ; Verb=SSet memberName}, SwigTemplates.FieldSet)
        | _       -> failwith "unexpected type with members"
    }

    let render'list'methods (rootType: TypeDescriptor) (elemType: TypeDescriptor) : seq<FunctionDescriptor * SwigCode> = seq{
        (** the rootType might be used in the future **)
        (**
        where to announce a inline index method?
        **)
        yield ({DeclaringType=elemType; Verb=LGet}, SwigTemplates.FieldGet)
        yield ({DeclaringType=elemType; Verb=LSet}, SwigTemplates.FieldSet)
        yield ({DeclaringType=elemType; Verb=LContains}, SwigTemplates.Contains)
        yield ({DeclaringType=elemType; Verb=LCount}, SwigTemplates.Count)
    }
    
    let define'struct'methods  (struct': TypeDescriptor) : seq<FunctionDescriptor * CodeGenerator> = seq {
        (** a cell is a struct, too **)
        let rootName = make'name struct'
        let memberMaker = render'struct'methods struct'
        for eachMember in struct'.Members do
            let fieldName = mangling eachMember.Name
            let fieldTypeName = make'arg'type eachMember.Type
            for fnDesc, swigCode in memberMaker eachMember do
                let codeGenerator (funcPtrAddr: string)  = 
                     PString.format swigCode (
                        Map[
                            "_"           ->> m_code;
                            "rootName"    ->> rootName;
                            "fieldName"   ->> fieldName;
                            "funcPtrAddr" ->> funcPtrAddr;
                            "fieldType"   ->> fieldTypeName
                        ])
                yield (fnDesc, codeGenerator)
        }
    
    let define'list'methods (list': TypeDescriptor) : seq<FunctionDescriptor * CodeGenerator> = seq{
        let elemType = Seq.head list'.ElementType
        for fnDesc, swigCode in render'list'methods list' elemType do
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
    
    let define'methods = function 
        | {TypeCode=LIST} as typeDesc ->  define'list'methods typeDesc
        | _               as typeDesc ->  define'struct'methods typeDesc
    
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
   

    let Generate(schema: IStorageSchema): seq<TypeDescriptor * seq<FunctionDescriptor * CodeGenerator>> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> define'methods
                |> fun methods   -> (typeDesc, methods))