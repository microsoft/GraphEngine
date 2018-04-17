module GraphEngine.Jit.JitNativeInterop
#nowarn "9"

open Microsoft.FSharp.Reflection
open System.Runtime.InteropServices
open System.Linq
open Trinity.Storage
open TypeSystem
open Utils
open Verbs
open System.Xml.Linq
open System
open Microsoft.FSharp.NativeInterop

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

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeVerb = 
    { Code            : int32 // tag of Verb
      Data            : nativeint }

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeFunctionDescriptor = 
    { Type            : NativeTypeDescriptor
      Verbs           : nativeint
      NrVerbs         : int32 }

[<StructLayout(LayoutKind.Sequential, Pack = 1)>]
[<Struct>]
type NativeCellAccessor = 
    { mutable CellPtr         : nativeint
      mutable CellId          : int64 
      mutable Size            : int32 
      mutable EntryIndex      : int32
      mutable Type            : uint16 }

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
      TypeCode       = desc.TypeCode      |> ToUnionTag
      NrMember       = desc.Members       |> Seq.length
      NrTSLAttribute = desc.TSLAttributes |> Seq.length
      ElementArity   = desc.ElementType   |> Seq.length
      ElementType    = desc.ElementType   |> Seq.map TypeDescriptorToNative      |> SeqToNative
      Members        = desc.Members       |> Seq.map MemberDescriptorToNative    |> SeqToNative
      TSLAttributes  = desc.TSLAttributes |> Seq.map AttributeDescriptorToNative |> SeqToNative }

let rec VerbsCount = function
    | ComposedVerb(_, x) -> 1 + (VerbsCount x)
    | _ -> 1

let rec VerbsToSeq (v: Verb) = seq {
    match v with
    | ComposedVerb(x, y) ->
        yield x
        yield! VerbsToSeq y
    | _ -> yield v

}

let VerbToNative (v: Verb) = 
    let data = 
        match v with
        | LInlineGet i | LInlineSet i -> i |> nativeint
        | SGet s       | SSet s       -> s |> ToUtf8
        | GSGet t      | GSSet t      -> t |> TypeDescriptorToNative |> Alloc |> NativePtr.toNativeInt
        // TODO forbid EAlloc etc.
        | ComposedVerb _              -> failwith "Cannot convert ComposedVerb to native"
        | _                           -> 0 |> nativeint
    DebugDump
        { Code = v |> ToUnionTag; Data = data }

let VerbsToNative (v: Verb) =
    VerbsToSeq v |> Seq.map VerbToNative   |> SeqToNative

let FunctionDescriptorToNative (fdesc: FunctionDescriptor) = 
    { Type           = fdesc.DeclaringType |> TypeDescriptorToNative 
      Verbs          = fdesc.Verb          |> VerbsToNative
      NrVerbs        = fdesc.Verb          |> VerbsCount}

let Make(desc: seq<ICellDescriptor>) = 
    desc |> Seq.map (Make >> TypeDescriptorToNative)
         |> Seq.toArray
