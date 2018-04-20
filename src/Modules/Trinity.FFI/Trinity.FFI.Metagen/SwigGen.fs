namespace Trinity.FFI.Metagen


module SwigGen = 
    open Trinity.FFI.Metagen.CommonForRender
    open Trinity.FFI.Metagen.Operator
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open System

    type Name = string
        
    type Code = string
        
    type FunctionId = string
        
    type TypeStr = string

    type ManglingChar = char

    type FunctionDecl = string
    
    let swig'typestr'mapper (typeDesc: TypeDescriptor) = 
        match typeDesc.TypeCode with
        | NULL     -> "void"
        
        | U8       -> "uint8_t"
        | U16      -> "uint16_t"
        | U32      -> "uint32_t"
        | U64      -> "uint64_t"
        | I8       -> "int8_t"
        | I16      -> "int16_t"
        | I32      -> "int32_t"
        | I64      -> "int64_t"
        
        | F32      -> "float"
        | F64      -> "double"
        
        | BOOL     -> "bool"
        | CHAR     -> "char"
        | STRING   -> "char*"
        | U8STRING -> "wchar*"
        

        | _        -> "void*"

           
    let render (manglingCode        : ManglingChar)
               (name'maker          : ManglingChar   -> TypeDescriptor -> Name) 
               (subject             : TypeDescriptor) 
               (verb                : Verb) 
               : FunctionDecl * (FunctionId -> Code) =
        
        let mutable decl: string = null
        
        let subject'name = name'maker manglingCode subject
        
        let object'type  = 
            match verb with
            | BSet
            | BGet -> String.Empty
            | _    -> swig'typestr'mapper (getObjectFromSubjectAndVerb subject verb) 

        let TemplateArgs = ["object type" ->> object'type; "subject name" ->> subject'name; "_" ->> manglingCode]

        
        let render'filter = fun (lst: list<char>) -> lst.Head <> '!'
        match verb with
        (** BSet/BGet works for cell **)
        | BGet ->
            decl <- "static void* Get{_}{subject name}(void *);"
            
            "static void* (* {_}Get{_}{subject name})(void*) = (void* (*)(void*)){!fn addr};" +/
            "static void* Get{_}{subject name}(void *subject)" +/
            "{{" +/
            "       return {_}Get{_}{subject name}(subject);" +/ 
            "}}"
        
        | BSet ->
            decl <- "static void Set{_}{subject name}(void*, void*);"

            "static void (* {_}Set{_}{subject name})(void*, void*) = (void (*)(void*, void*)){!fn addr};" +/
            "static void Set{_}{subject name}(void* subject, void* object)" +/ 
            "{{" +/ 
            "       return {_}Set{_}{subject name}(subject, object);" +/
            "}}"

        | ComposedVerb (LGet, BGet)
        | LGet -> 
            decl <- PString.format "static {object type} {subject name}{_}Get(void*, int);" TemplateArgs 
            
            "static {object type} (* {_}{subject name}{_}Get)(void*, int) =  ({object type} (*)(void*, int)){!fn addr};" +/
            "static {object type} {subject name}{_}Get(void* subject, int idx)" +/ 
            "{{" +/
            "        return {_}{subject name}{_}Get(subject, idx);" +/
            "}}"
            
        
        | LSet ->
            decl <- PString.format "static void {subject name}{_}Set(void*, int, {object type});" TemplateArgs
            "static void (* {_}{subject name}{_}Set)(void*, int,  {object type}) = (void (*)(void*, int, {object type} object)){!fn addr};" +/
            "static void {subject name}{_}Set(void* subject, int idx, {object type} object)" +/
            "{{" +/ 
            "return {_}{subject name}{_}Set(subject, idx, object);" +/ 
            "}}"
            

        | LCount ->
            decl <- PString.format "static int Count{_}{subject name}(void* subject);" TemplateArgs;
            
            "static int (* {_}Count{_}{subject name})(void*) = (int (*)(void* )) {!fn addr};" +/
            "static int Count{_}{subject name}(void* subject)" +/
            "{{" +/
            "    return {_}Count{_}{subject name}(subject);"+/
            "}}"

        | LContains ->

            decl <- PString.format "static bool Contains{_}{subject name}(void*, {object type});" TemplateArgs;
            
            "static bool (* {_}Contains{_}{subject name})(void*, {object type}) = (bool (*)(void*, {object type})) {!fn addr};" +/
            "static bool Contains{_}{subject name}(void* subject, {object type} object)" +/
            "{{" +/
            "    return {_}Contains{_}{subject name}(subject, object);" +/
            "}}"
        
        | ComposedVerb (SGet fieldName, BGet)
        | SGet fieldName ->
            let fnName = sprintf "{subject name}{_}Get{_}%s" fieldName
            decl <- PString.format "static {object type} {fnName}(void*);"  ("fnName" ->> fnName ::TemplateArgs);
            "static {object type} (* {_}{:fnName})(void*) = ({object type} (*)(void*)){!fn addr};" +/
            "static {object type} {:fnName}(void* subject)" +/
            "{{" +/
            "    return {_}{:fnName}(subject);" +/
            "}}"
            
            |> fun template -> PString.format'cond (fun lst -> lst.Head = ':') template [":fnName" ->> fnName]
        
        | SSet fieldName ->
            let fnName = sprintf "{subject name}{_}Set{_}%s" fieldName
            decl <- PString.format "static void {fnName}(void*, {object type});" ("fnName" ->> fnName ::TemplateArgs);

            "static void (* {_}{:fnName})(void*, {object type}) = (void (*)(void*, {object type})){!fn addr};" +/ 
            "static void {:fnName}(void* subject, {object type} object)" +/
            "{{" +/
            "    return {_}{:fnName}(subject, object);" +/
            "}}"

            |> fun template -> PString.format'cond (fun lst -> lst.Head = ':') template [":fnName" ->> fnName]
        
        | _ -> raise (NotImplementedException())
        
        |> fun template -> PString.format'cond render'filter template TemplateArgs
        
        |> fun template -> (PString.format decl TemplateArgs, fun fnAddr -> PString.format template ["!fn addr" ->> fnAddr])
                 
    

