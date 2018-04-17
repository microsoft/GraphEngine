module GraphEngine.Jit.Helper

open System.Runtime.InteropServices

type i8setter = delegate of nativeint * int8 -> unit
type i8getter = delegate of nativeint -> int8

type i16setter = delegate of nativeint * int16 -> unit
type i16getter = delegate of nativeint -> int16

type i32setter = delegate of nativeint * int32 -> unit
type i32getter = delegate of nativeint -> int32

type i64setter = delegate of nativeint * int64 -> unit
type i64getter = delegate of nativeint -> int64

type u8setter = delegate of nativeint * uint8 -> unit
type u8getter = delegate of nativeint -> uint8

type u16setter = delegate of nativeint * uint16 -> unit
type u16getter = delegate of nativeint -> uint16

type u32setter = delegate of nativeint * uint32 -> unit
type u32getter = delegate of nativeint -> uint32

type u64setter = delegate of nativeint * uint64 -> unit
type u64getter = delegate of nativeint -> uint64

type f32setter = delegate of nativeint * float32 -> unit
type f32getter = delegate of nativeint -> float32

type f64setter = delegate of nativeint * double -> unit
type f64getter = delegate of nativeint -> double

type strsetter   = delegate of nativeint * string -> unit
type strgetter   = delegate of nativeint -> string

type psetter   = delegate of nativeint * nativeint -> unit
type pgetter   = delegate of nativeint -> nativeint

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

type setter<'a> = delegate of nativeint * 'a -> unit

type CallHelper =
    static member CallByVal(callsite: nativeint, paccessor: nativeint, arg0: 'a) = 
        Marshal.GetDelegateForFunctionPointer(callsite, setter<'a>()).DynamicInvoke(paccessor, arg0) |> ignore
    static member CallByVal<'a> (callsite: nativeint, paccessor: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, getter<'a>()).DynamicInvoke(paccessor) :?> 'a

    static member CallByPtr(callsite: nativeint, paccessor: nativeint, arg0: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, setter<nativeint>()).DynamicInvoke(paccessor, arg0) |> ignore
    static member CallByPtr<'a> (callsite: nativeint, paccessor: nativeint) = 
        Marshal.GetDelegateForFunctionPointer(callsite, getter<nativeint>()).DynamicInvoke(paccessor) :?> nativeint

    static member GetPushedPtr (callsite: nativeint, paccessor: nativeint): nativeint = 
        Marshal.GetDelegateForFunctionPointer(callsite, typeof<pgetter>).DynamicInvoke(paccessor) :?> nativeint