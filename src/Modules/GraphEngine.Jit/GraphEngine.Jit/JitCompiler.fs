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
