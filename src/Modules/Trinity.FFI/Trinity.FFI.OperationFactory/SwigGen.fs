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

    type FunctionDecl = string
    
    let swig'typestr'mapper (typeDesc: TypeDescriptor) = 
        if 
            isPrimitive typeDesc.TypeCode
        then
            typeDesc.TypeName.ToLower()
        else
            "void*"
           
    let render (manglingCode        : ManglingChar)
               (name'maker          : ManglingChar   -> TypeDescriptor -> Name) 
               (subject             : TypeDescriptor) 
               (verb                : Verb) 
               : FunctionDecl * (FunctionId -> Code) =
        
        let mutable decl: string = null
         
        let subject'name = name'maker manglingCode subject
        
        let object = getObjectFromSubjectAndVerb subject verb

        let object'type  = swig'typestr'mapper object 

        let TemplateArgs = ["object type" ->> object'type; "subject name" ->> subject'name; "_" ->> manglingCode]


        let render'filter = fun (lst: list<char>) -> lst.Head <> '!'
        match verb with
        | LGet -> 
            decl <- PString.format "static {object type} {subject name}{_}Get(void* subject, int idx)" TemplateArgs 
            "
            static {object type} (* {_}{subject name}{_}Get)(void*, int) =  ({object type} (*)(void*, int)){!fn addr};
            static {object type} {subject name}{_}Get(void* subject, int idx)
            {{
                    return {_}{subject name}{_}Get(subject, idx);
            }}
            "
        
        | LSet ->
            decl <- PString.format "static void {subject name}{_}Get(void* subject, int idx, {object type} object)" TemplateArgs
            "
            static void (* {_}{subject name}{_}Get)(void*, int) = (void (*)(void*, int, {object type} object)){!fn addr};
            static void {subject name}{_}Get(void* subject, int idx, {object type} object){{
                    return {_}{subject name}{_}Get(subject, idx, object);
            }}
            "

        | LCount ->
            decl <- PString.format "static int {subject name}{_}Count(void* subject)" TemplateArgs;
            "
            static int (* {_}{subject name}{_}Count)(void*) = (int (*)(void* )) {!fn addr};
            static int {subject name}{_}Count(void* subject)
            {{
                return {_}{subject name}{_}Count(subject);
            }}
            "
        | LContains ->

            decl <- PString.format "static bool {subject name}{_}Contains(void* subject, {object type} object)" TemplateArgs;
            "
            static bool (* {_}{subject name}{_}Contains)(void*, {object type}) = (bool (*)(void*, {object type})) {!fn addr};
            static bool {subject name}{_}Contains(void* subject, {object type} object)
            {{
                return {_}{subject name}{_}Contains(subject, object);
            }}
            "
        | SGet fieldName ->
            let fnName = sprintf "{subject name}{_}Get{_}%s" fieldName
            decl <- PString.format "static {object type} {fnName}(void* subject))"  ("fnName" ->> fnName ::TemplateArgs);

            "
            static {object type} (* {_}{:fnName})(void*) = ({object type} (*)(void*)){!fn addr};
            static {object type} {:fnName}(void* subject)
            {{
                    return {_}{:fnName}(subject);
            }}
            "
            |> fun template -> PString.format'cond (fun lst -> lst.Head = ':') template [":fnName" ->> fnName]
        
        | SSet fieldName ->
            let fnName = sprintf "{subject name}{_}Set{_}%s" fieldName
            decl <- PString.format "static void {fnName}(void* subject, {object type} object)" ("fnName" ->> fnName ::TemplateArgs);

            "
            static void (* {_}{:fnName})(void*, {object type}) = (void (*)(void*, {object type})){!fn addr};
            static void {:fnName}(void* subject, {object type} object)
            {{
                    return {_}{:fnName}(subject, object);
            }}
            "
            |> fun template -> PString.format'cond (fun lst -> lst.Head = ':') template [":fnName" ->> fnName]
        
        | _ -> raise (NotImplementedException())
        
        |> fun template -> PString.format'cond render'filter template TemplateArgs
        
        |> fun template -> (decl, fun fnAddr -> PString.format template ["!fn addr" ->> fnAddr])
                 
    let generateFile((subject: TypeDescriptor, fields: seq<FunctionDecl * (FunctionId -> Code)>)) = 
        let additional'code =
           let 
           "
              {subject name}
           "
        0


    


