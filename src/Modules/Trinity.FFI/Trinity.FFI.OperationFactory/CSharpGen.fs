namespace Trinity.FFI.OperationFactory

module CSharp = 
     open GraphEngine.Jit.Verbs
     open GraphEngine.Jit.TypeSystem
     open System
     open Trinity.FFI.OperationFactory.Operator
     open Trinity.FFI.OperationFactory.PString
     
     type Name = string
             
     type Code = string
         
     type FunctionId = string
         
     type TypeStr = string
 
     type ManglingChar = char
     
     let render  (manglingChar        : ManglingChar)
                 (name'maker          : ManglingChar   -> TypeDescriptor -> Name) 
                 (subject             : TypeDescriptor) 
                 (object              : TypeDescriptor) = 
             
             
             let subject'name = name'maker manglingChar subject
             let subject'type = subject.TypeName

             let object'name  = name'maker manglingChar object
             let object'type  = object.TypeName
             
             (** 
             Any syntax like the following?
             
                on fun it -> name'maker manglingChar it, it.TypeName
                   subject'name, subject'type when subject
                   object'name , object'type  when object
                    
                
             **)
             
             let Pritimive    = isPrimitive object.TypeCode
             let arg'type     = if Pritimive then object'type else "void*"             
             let operationSig = PString.format "{suject name}{_}{object_name}" (
                                         Map [
                                            "subject name" ->> subject'name;
                                            "_"            ->> object'name;
                                            "object name"  ->> object'name
                                            ])
             function verb ->
             match verb with
             | LGet -> 
                let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "Get"])
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
                 let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "Set"])
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
             
                let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "Get"])
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
                  let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "Set"])
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
                  let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "LCount"])
                  
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
                  let toFnName = fun head -> PString.format "{0}{_}{1}{_}{2}" (Map["_" ->> manglingChar; "0" ->> head; "1" ->> operationSig; "2" ->> "Contains"])                  
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
                                                            (Map["_" ->> manglingChar; 
                                                                 "0" ->> head; 
                                                                 "1" ->> operationSig; 
                                                                 "2" ->> "Get"; 
                                                                 "3" ->> fieldName])
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
                                                             (Map["_" ->> manglingChar; 
                                                                  "0" ->> head; 
                                                                  "1" ->> operationSig; 
                                                                  "2" ->> "Set"; 
                                                                  "3" ->> fieldName])

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
                             Map [ "FUNC" ->> FUNC
                                   "DELE" ->> DELE
                                   "INST" ->> INST
                                   "ADDR" ->> ADDR
                                   "RET" ->> RET
                                   "arg type" ->> arg'type
                                   "subject type" ->> subject'type ]
                         PString.format template renderMap
                                
             
             
                  
                  
                  
                  
             
             
            
