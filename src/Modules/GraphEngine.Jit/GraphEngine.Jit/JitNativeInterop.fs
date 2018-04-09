module GraphEngine.Jit.JitNativeInterop
#nowarn "9"

open System.Runtime.InteropServices
open TypeSystem
open Trinity.Storage
open Microsoft.FSharp.NativeInterop
open System
open System.Linq
open Microsoft.FSharp.Reflection
open Utils
open Trinity.Core.Lib

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeAttributeDescriptor = {
    mutable Name : nativeint
    mutable Value: nativeint
}

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeTypeDescriptor = {
    mutable TypeName        : nativeint
    mutable QualifiedName   : nativeint // Full clr type name
    mutable ElementType     : nativeint
    mutable Members         : nativeint
    mutable TSLAttributes   : nativeint
    mutable NrMember        : int32
    mutable NrTSLAttribute  : int32
    mutable ElementArity    : int32
    mutable TypeCode        : int32
} 

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeMemberDescriptor = {
    mutable Name : nativeint
    mutable Type : NativeTypeDescriptor
    mutable Optional : uint8
}

let AttributeDescriptorToNative(desc: AttributeDescriptor) =
    {
        Name = desc.Name |> ToUtf8
        Value = desc.Value |> ToUtf8
    }

let rec MemberDescriptorToNative(mdesc: MemberDescriptor) =
    {
        Name = ToUtf8 mdesc.Name
        Type = TypeDescriptorToNative mdesc.Type
        Optional = if mdesc.Optional then 1uy else 0uy
    }

and TypeDescriptorToNative(desc: TypeDescriptor) = 
    let mutable ret = { 
        TypeName       = desc.TypeName |> ToUtf8
        QualifiedName  = desc.QualifiedName |> ToUtf8
        TypeCode       = FSharpValue.PreComputeUnionTagReader typeof<TypeCode> <| (desc.TypeCode :> obj)
        NrMember       = desc.Members.Count()
        NrTSLAttribute = desc.TSLAttributes.Count()
        ElementArity   = desc.ElementType.Count()
        ElementType    = IntPtr.Zero
        Members        = IntPtr.Zero
        TSLAttributes  = IntPtr.Zero }

    let pmember = NativePtr.ofNativeInt<NativeMemberDescriptor> ret.Members
    let pelement = NativePtr.ofNativeInt<NativeTypeDescriptor> ret.ElementType
    let pattr = NativePtr.ofNativeInt<NativeAttributeDescriptor> ret.TSLAttributes

    if ret.NrMember <> 0 then ret.Members <- (ret.NrMember * sizeof<NativeMemberDescriptor>) |> uint64 |> Memory.malloc |> IntPtr
    if ret.ElementArity <> 0 then ret.ElementType <- (ret.ElementArity * sizeof<NativeTypeDescriptor>) |> uint64 |> Memory.malloc |> IntPtr
    if ret.NrTSLAttribute <> 0 then ret.TSLAttributes <- (ret.NrTSLAttribute * sizeof<NativeAttributeDescriptor>) |> uint64 |> Memory.malloc |> IntPtr

    for i = 0 to ret.NrMember do
        NativePtr.write (NativePtr.add pmember i) (desc.Members.ElementAt(i) |> MemberDescriptorToNative)
    for i = 0 to ret.ElementArity do
        NativePtr.write (NativePtr.add pelement i) (desc.ElementType.ElementAt(i) |> TypeDescriptorToNative)
    for i = 0 to ret.NrTSLAttribute do
        NativePtr.write (NativePtr.add pattr i) (desc.TSLAttributes.ElementAt(i) |> AttributeDescriptorToNative)

    ret


let Make(desc: ICellDescriptor) = Make(desc) |> TypeDescriptorToNative
