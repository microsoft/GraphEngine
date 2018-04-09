module GraphEngine.Jit.Utils

open Microsoft.FSharp.Reflection
open Microsoft.FSharp.NativeInterop
open System.Runtime.CompilerServices
open Trinity.Core.Lib
open System.Text
open System

let ParseCase<'a> (value: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun x -> x.Name = value) with
    | [| case |] -> (FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _          -> failwith "Cannot parse union case"

let ToStringCase (value: 'a) = 
    let caseInfo, _ = FSharpValue.GetUnionFields(value, typeof<'a>)
    caseInfo.Name

[<MethodImpl(MethodImplOptions.AggressiveInlining)>]
let ToUtf8 (str: string) =
    let buf = Encoding.UTF8.GetBytes(str);
    let ret = Memory.malloc(buf.Length + 1 |> uint64);
    Memory.Copy(buf, ret, buf.Length);
    buf.SetValue(0, 0)
    buf.SetValue(0, 1)
    buf.SetValue(0, 2)
    buf.SetValue(0, 3)
    Memory.Copy(buf, 0, ret, buf.Length, 4)
    ret |> IntPtr