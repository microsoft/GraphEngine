module GraphEngine.Jit.Helper

open System.Runtime.InteropServices

// B-Series

type i8setter   = delegate of nativeint * int8              -> unit
type i8getter   = delegate of nativeint                     -> int8

type i16setter  = delegate of nativeint * int16             -> unit
type i16getter  = delegate of nativeint                     -> int16

type i32setter  = delegate of nativeint * int32             -> unit
type i32getter  = delegate of nativeint                     -> int32

type i64setter  = delegate of nativeint * int64             -> unit
type i64getter  = delegate of nativeint                     -> int64

type u8setter   = delegate of nativeint * uint8             -> unit
type u8getter   = delegate of nativeint                     -> uint8

type u16setter  = delegate of nativeint * uint16            -> unit
type u16getter  = delegate of nativeint                     -> uint16

type u32setter  = delegate of nativeint * uint32            -> unit
type u32getter  = delegate of nativeint                     -> uint32

type u64setter  = delegate of nativeint * uint64            -> unit
type u64getter  = delegate of nativeint                     -> uint64

type f32setter  = delegate of nativeint * float32           -> unit
type f32getter  = delegate of nativeint                     -> float32

type f64setter  = delegate of nativeint * double            -> unit
type f64getter  = delegate of nativeint                     -> double

type strsetter  = delegate of nativeint * string            -> unit
type strgetter  = delegate of nativeint                     -> string

type psetter    = delegate of nativeint * nativeint         -> unit
type pgetter    = delegate of nativeint                     -> nativeint

// L-Series

type li8setter  = delegate of nativeint * int32 * int8      -> unit
type li8getter  = delegate of nativeint * int32             -> int8

type li16setter = delegate of nativeint * int32 * int16     -> unit
type li16getter = delegate of nativeint * int32             -> int16

type li32setter = delegate of nativeint * int32 * int32     -> unit
type li32getter = delegate of nativeint * int32             -> int32

type li64setter = delegate of nativeint * int32 * int64     -> unit
type li64getter = delegate of nativeint * int32             -> int64

type lu8setter  = delegate of nativeint * int32 * uint8     -> unit
type lu8getter  = delegate of nativeint * int32             -> uint8

type lu16setter = delegate of nativeint * int32 * uint16    -> unit
type lu16getter = delegate of nativeint * int32             -> uint16

type lu32setter = delegate of nativeint * int32 * uint32    -> unit
type lu32getter = delegate of nativeint * int32             -> uint32

type lu64setter = delegate of nativeint * int32 * uint64    -> unit
type lu64getter = delegate of nativeint * int32             -> uint64

type lf32setter = delegate of nativeint * int32 * float32   -> unit
type lf32getter = delegate of nativeint * int32             -> float32

type lf64setter = delegate of nativeint * int32 * double    -> unit
type lf64getter = delegate of nativeint * int32             -> double

type lstrsetter = delegate of nativeint * int32 * string    -> unit
type lstrgetter = delegate of nativeint * int32             -> string

type lpsetter   = delegate of nativeint * int32 * nativeint -> unit
type lpgetter   = delegate of nativeint * int32             -> nativeint

let getter<'a>() =
    match typeof<'a> with
    | x when x = typeof<int8>   -> typeof<i8getter>
    | x when x = typeof<int16>  -> typeof<i16getter>
    | x when x = typeof<int32>  -> typeof<i32getter>
    | x when x = typeof<int64>  -> typeof<i64getter>

    | x when x = typeof<uint8>  -> typeof<u8getter>
    | x when x = typeof<uint16> -> typeof<u16getter>
    | x when x = typeof<uint32> -> typeof<u32getter>
    | x when x = typeof<uint64> -> typeof<u64getter>

    | x when x = typeof<float32>-> typeof<f32getter>
    | x when x = typeof<double> -> typeof<f64getter>

    | x when x = typeof<string> -> typeof<strgetter>
    | _                         -> typeof<pgetter>


let setter<'a>() =
    match typeof<'a> with
    | x when x = typeof<int8>   -> typeof<i8setter>
    | x when x = typeof<int16>  -> typeof<i16setter>
    | x when x = typeof<int32>  -> typeof<i32setter>
    | x when x = typeof<int64>  -> typeof<i64setter>

    | x when x = typeof<uint8>  -> typeof<u8setter>
    | x when x = typeof<uint16> -> typeof<u16setter>
    | x when x = typeof<uint32> -> typeof<u32setter>
    | x when x = typeof<uint64> -> typeof<u64setter>

    | x when x = typeof<float32>-> typeof<f32setter>
    | x when x = typeof<double> -> typeof<f64setter>

    | x when x = typeof<string> -> typeof<strsetter>
    | _                         -> typeof<psetter> 

let lgetter<'a>() =
    match typeof<'a> with
    | x when x = typeof<int8>   -> typeof<li8getter>
    | x when x = typeof<int16>  -> typeof<li16getter>
    | x when x = typeof<int32>  -> typeof<li32getter>
    | x when x = typeof<int64>  -> typeof<li64getter>

    | x when x = typeof<uint8>  -> typeof<lu8getter>
    | x when x = typeof<uint16> -> typeof<lu16getter>
    | x when x = typeof<uint32> -> typeof<lu32getter>
    | x when x = typeof<uint64> -> typeof<lu64getter>

    | x when x = typeof<float32>-> typeof<lf32getter>
    | x when x = typeof<double> -> typeof<lf64getter>

    | x when x = typeof<string> -> typeof<lstrgetter>
    | _                         -> typeof<lpgetter>


let lsetter<'a>() =
    match typeof<'a> with
    | x when x = typeof<int8>   -> typeof<li8setter>
    | x when x = typeof<int16>  -> typeof<li16setter>
    | x when x = typeof<int32>  -> typeof<li32setter>
    | x when x = typeof<int64>  -> typeof<li64setter>

    | x when x = typeof<uint8>  -> typeof<lu8setter>
    | x when x = typeof<uint16> -> typeof<lu16setter>
    | x when x = typeof<uint32> -> typeof<lu32setter>
    | x when x = typeof<uint64> -> typeof<lu64setter>

    | x when x = typeof<float32>-> typeof<lf32setter>
    | x when x = typeof<double> -> typeof<lf64setter>

    | x when x = typeof<string> -> typeof<lstrsetter>
    | _                         -> typeof<lpsetter> 

type setter<'a> = delegate of nativeint * 'a -> unit

type CallHelper =
    static member CallByVal(callsite: nativeint, paccessor: nativeint, arg0: 'a) = 
        Marshal.GetDelegateForFunctionPointer(callsite, setter<'a>()).DynamicInvoke(paccessor, arg0) |> ignore
    static member CallByVal<'a> (callsite: nativeint, paccessor: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, getter<'a>()).DynamicInvoke(paccessor) :?> 'a

    static member CallByVal(callsite: nativeint, paccessor: nativeint, arg0: int, arg1: 'a) = 
        Marshal.GetDelegateForFunctionPointer(callsite, lsetter<'a>()).DynamicInvoke(paccessor, arg0, arg1) |> ignore
    static member CallByVal<'a> (callsite: nativeint, paccessor: nativeint, arg0: int) = 
        Marshal.GetDelegateForFunctionPointer(callsite, lgetter<'a>()).DynamicInvoke(paccessor, arg0) :?> 'a

    static member CallByPtr(callsite: nativeint, paccessor: nativeint, arg0: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, setter<nativeint>()).DynamicInvoke(paccessor, arg0) |> ignore
    static member CallByPtr<'a> (callsite: nativeint, paccessor: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, getter<nativeint>()).DynamicInvoke(paccessor) :?> nativeint

    static member CallByPtr(callsite: nativeint, paccessor: nativeint, arg0: int, arg1: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, typeof<lpsetter>).DynamicInvoke(paccessor, arg0, arg1) |> ignore
    static member CallByPtr<'a> (callsite: nativeint, paccessor: nativeint, arg0: int) = 
        Marshal.GetDelegateForFunctionPointer(callsite, typeof<lpgetter>).DynamicInvoke(paccessor, arg0) :?> nativeint

    static member GetPushedPtr (callsite: nativeint, paccessor: nativeint): nativeint = 
        Marshal.GetDelegateForFunctionPointer(callsite, typeof<pgetter>).DynamicInvoke(paccessor) :?> nativeint
    static member GetPushedPtr (callsite: nativeint, paccessor: nativeint, arg0: int): nativeint = 
        Marshal.GetDelegateForFunctionPointer(callsite, typeof<lpgetter>).DynamicInvoke(paccessor, arg0) :?> nativeint