namespace Trinity.FFI.OperationFactory

open Trinity.FFI.OperationFactory

module TGEN = 
    open GraphEngine.Jit
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open Trinity.FFI.OperationFactory.Operator

    type SwigCode      = string
    type CodeGenerator = (string -> string)

    
    let manglingCharmanglingChar = '_'
    let mangling (name: string) = name.Replace(manglingCharmanglingChar, manglingCharmanglingChar + manglingCharmanglingChar)
    
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
                PString.format "List{_}{elem}" (Map["_" ->> manglingCharmanglingChar; "elem" ->> elemTypeName])
        | {TypeCode=CELL; TypeName=cellName}     ->
                PString.format "Cell{_}{cellName}" (Map ["_" ->> manglingCharmanglingChar; "cellName" ->> mangling cellName])  
        | {TypeCode=STRUCT; TypeName=structName} ->
                    PString.format "Struct{_}{structName}" (Map["_" ->> manglingCharmanglingChar; "structName" ->> mangling structName])
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
   

    let Generate (render'verb : TypeDescriptor -> TypeDescriptor -> Verb -> 'T) (schema: IStorageSchema): seq<TypeDescriptor * seq<FunctionDescriptor * 'T>> =
        schema.CellDescriptors
        |> Seq.map Make
        |> Seq.collect TypeInfer
        |> Seq.distinct
        |> Seq.map (fun typeDesc ->
                typeDesc
                |> render'operations render'verb
                |> fun methods   -> (typeDesc, methods))
    

    open Trinity.FFI.OperationFactory.Swig
    let Generate'Swig it = Swig.render manglingCharmanglingChar make'name make'arg'type |> Generate <| it

