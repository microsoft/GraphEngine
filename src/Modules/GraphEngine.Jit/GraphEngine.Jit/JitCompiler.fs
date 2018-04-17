module GraphEngine.Jit.JitCompiler 
#nowarn "9"

open TypeSystem
open Verbs
open JitNativeInterop
open System.Collections.Generic
open System
open System.Runtime.InteropServices
open Utils
open Microsoft.FSharp.NativeInterop

type NativeFunction = { CallSite: nativeint; Descriptor: FunctionDescriptor }

let s_types = new Dictionary<TypeDescriptor, NativeFunction [] >()

[<DllImport("GraphEngine.Jit.Native.dll")>]
extern nativeint CompileFunctionToNative (nativeint)

let CompileFunction (f: FunctionDescriptor): NativeFunction =
    let p = f |> FunctionDescriptorToNative |> Alloc |> NativePtr.toNativeInt
    //printf "NativeFunctionDescriptor = %d" p
    //let callsite = CompileFunctionToNative p
    //printf "CallSite = %d" callsite

    { NativeFunction.CallSite = (CompileFunctionToNative p)
      Descriptor = f }

(** for test **)
let CellIdToNativeCellAccessor (cellId: int64) = 
    let mutable accessor: NativeCellAccessor = {
        CellPtr = IntPtr.Zero
        CellId = cellId
        Type = 0us
        EntryIndex = -1
        Size = 0
    }
    let mutable p: nativeptr<byte> = NativePtr.ofNativeInt (nativeint 0)
    Trinity.Global.LocalStorage.GetLockedCellInfo(accessor.CellId, &accessor.Size, &accessor.Type, &p, &accessor.EntryIndex) |> ignore
    &&accessor |> NativePtr.toNativeInt