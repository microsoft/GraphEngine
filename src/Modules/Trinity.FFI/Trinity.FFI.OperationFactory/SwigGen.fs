namespace Trinity.FFI.OperationFactory


module SwigGen = 
    open Trinity.FFI.OperationFactory.CommonForRender
    open Trinity.FFI.OperationFactory.Operator
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System

    type Name = string
        
    type Code = string
        
    type FunctionId = string
        
    type TypeStr = string

    type ManglingChar = char
    
    
    let to'templates'then (verb : Verb) = 
        match verb with
        | LGet           -> "
            static void (* {_}List{_}Get{_}{subject name}{_}{object name})(void* subject, int idx, {object type} &object) = {!fn addr};
            static void List{_}Get{_}{subject name}{_}{object name}(void* subject, int idx, {object type} &object){{
                    {_}List{_}Get{_}{subject name}{_}{object name}(subject, idx, object);
            }}
            "
        | LSet           -> "
            static void (* {_}List{_}Set{_}{subject name}{_}{object name})(void* subject, int idx, {object type} &object) = {!fn addr};
            static void List{_}Set{_}{subject name}{_}{object name}(void* subject, int idx, {object type} &object){{
                    {_}List{_}Set{_}{subject name}{_}{object name}(subject, idx, object);
            }}
            "
        | LInlineGet idx -> "
            static void (* {_}List{_}Get{_}{subject name}{_}{object name})_At" + idx.ToString() + "(void* subject, {object type} &object) = {!fn addr};
            static void List{_}Get{_}{subject name}{_}{object name}" + idx.ToString() + "(void* subject, {object type} &object){{
                    {_}List{_}Get{_}{subject name}{_}{object name}" + idx.ToString() + "(subject, object);
            }}
            "
        | LInlineSet idx -> "
            static void (* {_}List{_}Set{_}{subject name}{_}{object name})_At" + idx.ToString() + "(void* subject, {object type} object) = {!fn addr};
            static void List{_}Set{_}{subject name}{_}{object name}" + idx.ToString() + "(void* subject, {object type} object){{
                    {_}List{_}Set{_}{subject name}{_}{object name}" + idx.ToString() + "(subject, object);
            }}
           "
        | LContains      -> "
            static bool (* {_}List{_}Contains{_}{subject name}{_}{object name})(void* subject, {object type} &object) = {!fn addr};
            static bool List{_}Contains{_}{subject name}{_}{object name}(void* subject, {object type} &object){{
                    return {_}List{_}Contains{_}{subject name}{_}{object name}(subject, object);
            }}
            "
        | LCount         -> "
            static int (* {_}List{_}Count{_}{subject name})(void* subject) = {!fn addr};
            static int List{_}Count{_}{subject name}(void* subject){{
                    return {_}List{_}Count{_}{subject name}(subject);
            }}
            "
        | SGet fieldName -> "
            static void (* {_}Struct{_}Get{_}" + fieldName + "{_}{object name})(void* subject, {object type} &object) = {!fn addr};
            static void Struct{_}Get{_}" + fieldName + "{_}{object name}(void* subject, {object type} &object){{
                    {_}Struct{_}Get{_}" + fieldName + "{_}{object name}(subject, object);
            }}
            "
        | SSet fieldName -> "
            static void (* {_}Struct{_}Set{_}" + fieldName + "{_}{object name})(void* subject, {object type} object) = {!fn addr};
            static void Struct{_}Set{_}" + fieldName + "{_}{object name}(void* subject, {object type} object){{
                    {_}Struct{_}Set{_}" + fieldName + "{_}{object name}(subject, object);
            }}
            "
        //| BSet          -> raise (NotImplementedException())
        | _ -> raise (NotImplementedException())
    
    let swig'typestr'mapper (typeDesc: TypeDescriptor) = 
        if 
            isPrimitive typeDesc.TypeCode
        then
            typeDesc.TypeName.ToLower()
        else
            "void*"
           
    let render (manglingChar        : ManglingChar)
               (name'maker          : ManglingChar   -> TypeDescriptor -> Name) 
               (subject             : TypeDescriptor) 
               (verb                : Verb) 
               : Verb * (FunctionId -> Code) =
         
        let subject'name = name'maker manglingChar subject
        let object = getObjectFromSubjectAndVerb subject verb
        
        let object'name  = name'maker manglingChar object
        let object'type  = swig'typestr'mapper object 
        
        let partial'format = 
            PString.format'cond 
                        (fun it -> it.Head <> '!') 
                        (to'templates'then verb) 
                        [ 
                            "_"            ->> manglingChar;
                            "subject name" ->> subject'name;
                            "object name"  ->> object'name;
                            "object type"  ->> object'type 
                        ]
        
        verb, fun fnId -> PString.format partial'format ["!fn addr" ->> fnId]
    
    let genSourceCode (moduleName: Name) (sourceCode: Code) = 
        "
        %module {moduleName}
        %{{
        #define SWIG_FILE_WITH_INIT
        {sourceCode}
        %}}
        {sourceCode}
        " |> fun template -> PString.format template ["moduleName" ->> moduleName; "sourceCode" ->> sourceCode]

