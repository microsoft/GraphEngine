namespace GraphEngine.Jit

open GraphEngine.Jit.TypeSystem

type Verb = Get       | Set                               // Basic getter/setter
          | IndexGet  | IndexSet | Count    | Contains    // Basic array-ish operators
          | EtorAlloc | EtorFree | EtorNext | EtorCurrent // Basic enumerator

type FunctionDescriptor = {
    DeclaringType         : TypeDescriptor
    Verb                  : Verb
} with member x.Inputs = match x.Verb with
                            | Get -> seq []
                            | Set -> seq [x.DeclaringType]
                            //| IndexGet -> match x.DeclaringType with
                            //    | ListType _ -> seq [Builder.MakeFromType <| typeof<int32>]
                            //| IndexSet -> seq [Builder.MakeFromType <| typeof<int32>; x.DeclaringType]

