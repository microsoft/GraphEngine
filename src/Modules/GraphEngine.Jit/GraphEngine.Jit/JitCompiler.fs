module GraphEngine.Jit.JitCompiler 

open TypeSystem
open Verbs
open System.Collections.Generic
open System

type NativeFunction = { CallSite: nativeint; Descriptor: FunctionDescriptor }

let s_types = new Dictionary<TypeDescriptor, NativeFunction [] >()

type IL = 
    | NewArg of TypeCode * string
    | NewPtr of TypeCode * string
    | NewGp  of TypeCode * string
    | Mov    of string * string    // reg to reg
    | Load   of string * string    // mem to reg
    | ILoad  of string * int       // imm to reg
    | IAdd   of string * int       // add imm to reg
    | Add    of string * string    // add reg to reg
    | SetArg of int * string
    | Ret    of string
    | Call   of string
    | IRet

let CompileFunction (f: FunctionDescriptor): NativeFunction =
    let fs     = FunctionSignature f

    { NativeFunction.CallSite = IntPtr.Zero
      Descriptor = f }
