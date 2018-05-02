module GraphEngine.Jit.Utils
#nowarn "9"

open Microsoft.FSharp.Reflection
open Microsoft.FSharp.NativeInterop
open Trinity.Core.Lib
open System.Text
open System
open System.Linq
open System
open System.Runtime.InteropServices

let DebugDump x = 
    printfn "========="
    printfn "%A" x
    printfn "========="
    x

let ParseCase<'a> (value: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun x -> x.Name = value) with
    | [| case |] -> (FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _          -> failwith "Cannot parse union case"

let ToStringCase (value: 'a) = 
    let caseInfo, _ = FSharpValue.GetUnionFields(value, typeof<'a>)
    caseInfo.Name

[<DllImport("msvcrt")>]
extern int strlen(IntPtr)

let Malloc = uint64 >> Memory.malloc >> IntPtr

//  Copy a value type to unmanaged heap
let Alloc (v: 'a) =
    let buf = Malloc sizeof<'a> |> NativePtr.ofNativeInt<'a>
    NativePtr.write buf v
    buf

let AddTslHead (p: IntPtr) len = 
    let buf = Malloc (sizeof<int> + len)
    NativePtr.ofNativeInt<int>(buf) |> NativePtr.write <| len
    let pcontent = NativePtr.add (NativePtr.ofNativeInt<uint8>(buf)) (sizeof<int>) |> NativePtr.toNativeInt
    Memory.memcpy(pcontent.ToPointer(), p.ToPointer(), uint64 len) |> ignore
    buf

let ToUtf8 (str: string) =
    let buf = Encoding.UTF8.GetBytes(str)
    let ret = buf.Length + 1 |> Malloc
    let bp  = NativePtr.ofNativeInt<uint8> ret
    Memory.Copy(buf, ret.ToPointer(), buf.Length)
    NativePtr.write (NativePtr.add bp buf.Length) 0uy
    ret

let SeqToNative (s: seq<'a>) =
    let arr = Seq.toArray s
    let buflen = arr.Count() * sizeof<'a> |> uint64
    if buflen = 0UL then IntPtr.Zero
    else let p = buflen |> Memory.malloc
         use buf = fixed arr
         Memory.memcpy(p, (NativePtr.toNativeInt buf).ToPointer(), buflen) |> IntPtr

let ToUnionTag (c: 'a) =
    c |> box |> FSharpValue.PreComputeUnionTagReader typeof<'a>
