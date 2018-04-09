module GraphEngine.Jit.JitNativeInterop
#nowarn "9"

open System.Runtime.InteropServices
open TypeSystem
open Trinity.Storage
open System.Linq
open Microsoft.FSharp.Reflection
open Utils

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeAttributeDescriptor = 
    { Name            : nativeint
      Value           : nativeint }

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeTypeDescriptor = 
    { TypeName        : nativeint
      QualifiedName   : nativeint // Full clr type name
      ElementType     : nativeint
      Members         : nativeint
      TSLAttributes   : nativeint
      NrMember        : int32
      NrTSLAttribute  : int32
      ElementArity    : int32
      TypeCode        : int32 } 

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeMemberDescriptor = 
    { Name            : nativeint
      Type            : NativeTypeDescriptor
      Optional        : uint8 }

let AttributeDescriptorToNative(desc: AttributeDescriptor) =
    { Name  = desc.Name  |> ToUtf8
      Value = desc.Value |> ToUtf8 }

let rec MemberDescriptorToNative(mdesc: MemberDescriptor) =
    { Name = ToUtf8 mdesc.Name
      Type = TypeDescriptorToNative mdesc.Type
      Optional = if mdesc.Optional then 1uy else 0uy }

and TypeDescriptorToNative(desc: TypeDescriptor) = 
    { TypeName       = desc.TypeName      |> ToUtf8
      QualifiedName  = desc.QualifiedName |> ToUtf8
      TypeCode       = FSharpValue.PreComputeUnionTagReader typeof<TypeCode> <| (desc.TypeCode :> obj)
      NrMember       = desc.Members.Count()
      NrTSLAttribute = desc.TSLAttributes.Count()
      ElementArity   = desc.ElementType.Count()
      ElementType    = desc.ElementType   |> Seq.map TypeDescriptorToNative      |> SeqToNative
      Members        = desc.Members       |> Seq.map MemberDescriptorToNative    |> SeqToNative
      TSLAttributes  = desc.TSLAttributes |> Seq.map AttributeDescriptorToNative |> SeqToNative }


let Make(desc: seq<ICellDescriptor>) = 
    desc |> Seq.map (Make >> TypeDescriptorToNative)
         |> Seq.toArray
