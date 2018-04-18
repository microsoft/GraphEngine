namespace Trinity.FFI.Metagen

module CSharpGen = 
     open Trinity.FFI.Metagen.CommonForRender
     open Trinity.FFI.Metagen.Operator
     open GraphEngine.Jit.Verbs
     open GraphEngine.Jit.TypeSystem
     open System
     
     type Name = string
             
     type Code = string
         
     type FunctionAddrGetterName = string

     type ManglingCode = char
        
     let render  (manglingCode        : ManglingCode)
                 (name'maker          : ManglingCode -> TypeDescriptor -> Name) 
                 (subject             : TypeDescriptor) 
                 (verb                : Verb) : FunctionAddrGetterName * Code = 
             
             let subject'name = name'maker manglingCode subject
             let subject'type = subject |> TypeName
                          
             let getObjectInfo (object: TypeDescriptor) = 
                 let object'name  = name'maker manglingCode object
                 let object'type  = object |> TypeName
                 let Pritimive    = isPrimitive object.TypeCode
                 let arg'type     = if Pritimive then object'type else "void*"           
                 let operationSig = PString.format "{subject name}{_}{object name}" 
                                                    [
                                                        "subject name" ->> subject'name;
                                                        "_"            ->> manglingCode;
                                                        "object name"  ->> object'name
                                                    ]
                 (object'name, object'type, Pritimive, arg'type, operationSig)
             
             (** Although do pattern matching twice when it's a struct, the code is less.
                 CodeGen doesn't cost much so we just try a more readable way.
              **)
             let (object'name, object'type, Pritimive, arg'type, operationSig) = 
                if 
                    subject.TypeCode = LIST
                then
                    subject |> getElemTypeFromSubject |> getObjectInfo       
                else
                    match verb with
                    | SGet fieldName
                    | SSet fieldName ->
                        subject |> getMemberTypeFromSubject fieldName |> getObjectInfo
                    | _             -> failwith "Impossible."
             
             
             match verb with
             | LGet -> 
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}"  
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "Get"]
                let RHS  = if Pritimive then "src[idx]" else "GCHandle.ToIntPtr(GCHandle.Alloc(src[idx])).ToPointer()"
                let RET  = sprintf "@object = %s" RHS
                
                " 
                public delegate void {DELE}(void* subject, int idx, out {arg type} @object);
                public static void {FUNC}(void* subject, int idx, out {arg type} @object)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET) 

                                
             | LSet -> 
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}" 
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "Set"]
                let RHS  = if Pritimive then "@object" else sprintf "((%s)GCHandle.FromIntPtr((IntPtr)@object).Target)" object'type
                let RET  = sprintf "src[idx] = %s" RHS
                 
                " 
                public delegate void {DELE}(void* subject, int idx, {arg type} @object);
                public static void {FUNC}(void* subject, int idx, {arg type} @object)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)

             | LInlineGet idx ->
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}" 
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "Get"]

                let RHS  = sprintf (if Pritimive then "src[%d]" else "GCHandle.ToIntPtr(GCHandle.Alloc(src[%d])).ToPointer()") idx
                let RET  = sprintf "@object = %s" RHS
                    
                " 
                public delegate void {DELE}(void* subject, out {arg type} @object);
                public static void {FUNC}(void* subject, out {arg type} @object)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)
                    
                                   
             | LInlineSet idx -> 
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}" 
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "Set"]

                let LHS  = sprintf "src[%d]" idx
                let RHS  = if Pritimive then "@object" else sprintf "((%s)GCHandle.FromIntPtr((IntPtr)@object).Target)" object'type
                let RET  = sprintf "%s = %s" LHS RHS;
                " 
                public delegate void {DELE}(void* subject, {arg type} @object);
                public static void {FUNC}(void* subject, {arg type} @object)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)
             
             | LCount ->
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}" 
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "LCount"]
                  
                let RET  = "return src.Count()"
                  
                "
                public delegate int {DELE}(void* subject);
                public static int {FUNC}(void* subject)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)

             | LContains ->
                let toFnName = fun head -> 
                                    PString.format "{0}{_}{1}{_}{2}" 
                                                   ["_" ->> manglingCode; "0" ->> head; "1" ->> operationSig; "2" ->> "Contains"]
                                                   
                let ELEM  = if Pritimive then "@object" else sprintf "((%s)GCHandle.FromIntPtr((IntPtr)@object).Target)" object'type
                let RET   = sprintf "return src.Contains(%s)? 1 : 0" ELEM

                "
                public delegate int {DELE}(void* subject, {arg type} @object);
                public static int {FUNC}(void* subject, {arg type} @object)
                {{
                var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                {RET};                    
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                " 
                |> fun it -> (it, toFnName, RET)
                  
                                       
             | SGet fieldName ->
                let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}_{3}" 
                                                          [
                                                            "_" ->> manglingCode; 
                                                            "0" ->> head; 
                                                            "1" ->> operationSig; 
                                                            "2" ->> "Get"; 
                                                            "3" ->> fieldName
                                                          ]
                let LHS  = "@object"
                let RHS  = sprintf (if Pritimive then "src.%s" else "GCHandle.ToIntPtr(GCHandle.Alloc(src.%s)).ToPointer()") fieldName
                let RET  = sprintf "%s = %s" LHS RHS;
                  
                "
                public delegate void {DELE}(void* subject, out {arg type} @object);
                public static void {FUNC}(void* subject, out {arg type} @object)
                {{
                var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)
                  
             | SSet fieldName ->
                let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}_{3}" 
                                                          [
                                                            "_" ->> manglingCode; 
                                                            "0" ->> head; 
                                                            "1" ->> operationSig; 
                                                            "2" ->> "Set"; 
                                                            "3" ->> fieldName
                                                          ]

                let LHS  = sprintf "src.%s" fieldName
                let RHS  = if Pritimive then "@object" else sprintf "((%s)GCHandle.FromIntPtr((IntPtr)@object).Target)" object'type
                let RET  = sprintf "%s = %s" LHS RHS;
                   
                "
                public delegate void {DELE}(void* subject, out {arg type} @object);
                public static void {FUNC}(void* subject, out {arg type} @object)
                {{
                    var src = ({subject type}) GCHandle.FromIntPtr((IntPtr)subject).Target;
                    {RET};
                }}
                public static {DELE} {INST} = {FUNC};
                public static int {ADDR} => Marshal.GetFunctionPointerForDelegate({INST}).ToInt32();
                "
                |> fun it -> (it, toFnName, RET)
                  
             | _        -> raise (NotImplementedException())
             
             |>fun(template, toFnName, RET) -> 
                        let FUNC = toFnName "FUNC"
                        let DELE = toFnName "DELE"
                        let INST = toFnName "INST"
                        let ADDR = toFnName "ADDR"
                         
                        let renderMap = 
                               [ 
                                "FUNC" ->> FUNC
                                "DELE" ->> DELE
                                "INST" ->> INST
                                "ADDR" ->> ADDR
                                "RET"  ->> RET
                                "arg type"     ->> arg'type
                                "subject type" ->> subject'type 
                               ]

                        (ADDR, PString.format template renderMap)
                                
             
             
                  
                  
                  
                  
         
         
            
