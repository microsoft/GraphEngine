namespace Trinity.FFI.OperationFactory

open Trinity.FFI.OperationFactory

module TGEN = 
    open GraphEngine.Jit
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open Trinity.FFI.OperationFactory.Command
    open Trinity.FFI.OperationFactory.Operator

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
    
    let render'operations (render'verb : TypeDescriptor -> TypeDescriptor -> Verb -> 'T) (type': TypeDescriptor) : seq<FunctionDescriptor * 'T> =
        match type' with
        | {TypeCode=CELL; Members=members}
        | {TypeCode=STRUCT; Members=members}    ->
           members
           |> Seq.collect (
                fun (member': MemberDescriptor) ->
                    let fieldName   = member'.Name
                    let memberType  = member'.Type
                    let fnDescMaker = fun verb  -> {DeclaringType=memberType; Verb=verb} 
                    [SGet; SSet]
                    |> Seq.map (fun it   -> it fieldName)
                    |> Seq.map (fun verb -> (fnDescMaker verb, render'verb type' memberType verb))
           )
        
        | {TypeCode=LIST;ElementType=elemTypes}  ->
             let elemType = Seq.head elemTypes
             [LGet; LSet;  LContains; LCount;]
             |> Seq.map (fun verb -> ({DeclaringType=elemType; Verb=verb}, render'verb type' elemType verb))
        
        | _                                      -> 
             failwith "Unexpected type descrriptor."                                 
                  
        
        

        

    let render'struct'methods (render'verb: Verb -> 'T) (rootType: TypeDescriptor) (memberDesc: MemberDescriptor) : seq<FunctionDescriptor * 'T> =
        let memberName       = memberDesc.Name
        let memberType       = memberDesc.Type
        let fnDescMaker verb = {DeclaringType=memberType ; Verb=verb}
        match rootType.TypeCode with
        | CELL
        | STRUCT  -> 
            [SGet; SSet]
            |> Seq.map (fun it -> it memberName)
            |> Seq.map (fun verb -> (fnDescMaker verb, render'verb verb))
        | _       -> failwith "unexpected type with members"
  

    let render'list'methods (render'verb: Verb -> 'T) (rootType: TypeDescriptor) (elemType: TypeDescriptor) : seq<FunctionDescriptor * SwigCode> =
        (**
        where to announce a inline index method?
        **)
        failwith "out of date"

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