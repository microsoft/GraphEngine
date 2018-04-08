module Basic

open Xunit
open Verbs
open TypeSystem
open Utils
open Trinity.Storage
open System.Collections.Generic
open System
open JitCompiler
open Trinity.Core.Lib
open Microsoft.FSharp.NativeInterop

[<Fact>]
let ``Verb allocation`` () =
    Verb.BGet

[<Theory>]
[<InlineData("U16")>]
[<InlineData("I32")>]
[<InlineData("F64")>]
let ``TypeDescriptor allocation`` (tcode: string) =
    { TypeDescriptor.ElementType = []
      TypeName                   = "Type"
      QualifiedName              = "QType"
      Members                    = []
      TSLAttributes              = []
      TypeCode                   = ParseCase tcode }

[<Theory>]
[<InlineData(typeof<int32>, 4, "I32")>]
[<InlineData(typeof<string>, 0, "STRING")>]
[<InlineData(typeof<System.DateTime>, 0, "S")>]
[<InlineData(typeof<System.Collections.Generic.List<int>>, 0, "L")>]
let ``TypeDescriptor allocation from System.Type`` (t: System.Type) (w: int32) (tt: string) =
    let desc = MakeFromType t
    if w > 0 then Assert.Equal(w, (TryGetTypeWidth desc).Value)
    match t with
    | PrimitiveType tcode -> Assert.Equal(tt, ToStringCase tcode)
    | ListType      _ -> Assert.Equal(tt, "L")
    | StructType    _ -> Assert.Equal(tt, "S")
    desc

let attrs_empty() = 
    { new IAttributeCollection with
          member this.Attributes: System.Collections.Generic.IReadOnlyDictionary<string,string> = 
              new Dictionary<string, string>() :> IReadOnlyDictionary<string, string>
          member this.GetAttributeValue(attributeKey: string): string = 
              failwith "???" }

let celldesc (name: string, fields: seq<IFieldDescriptor>, ct) = 
    { new ICellDescriptor with
        member this.Attributes: System.Collections.Generic.IReadOnlyDictionary<string,string> = 
            attrs_empty().Attributes
        member this.CellType: uint16 = 
            ct
        member this.GetAttributeValue(attributeKey: string): string = 
            attrs_empty().GetAttributeValue attributeKey
        member this.GetFieldAttributes(fieldName: string): IAttributeCollection = 
            attrs_empty()
        member this.GetFieldDescriptors(): System.Collections.Generic.IEnumerable<IFieldDescriptor> = 
            fields
        member this.GetFieldNames(): System.Collections.Generic.IEnumerable<string> = 
            raise (System.NotImplementedException())
        member this.IsList(): bool = 
            false
        member this.IsOfType(): bool = 
            false
        member this.Type: System.Type = 
            this.GetType()
        member this.TypeName: string = 
            name }

let ICellDescriptorGen() = seq {
    yield  [| "C1" :> obj; new List<IFieldDescriptor>() :> obj ; 0us  :> obj|]
}

let TypeDescriptorCollection() = seq {
    yield [| ``TypeDescriptor allocation`` "U32" :> obj |]
}

[<Theory>]
[<MemberData("ICellDescriptorGen")>]
let ``TypeDescriptor allocation from ICellDescriptor`` (a,b,c) = (a,b,c) |> celldesc |> Make

[<Theory>]
[<MemberData("TypeDescriptorCollection")>]
let ``FunctionDescriptor allocation``(tdesc: TypeDescriptor) =
    { DeclaringType = tdesc
      Verb          = BGet }

[<Theory>]
[<MemberData("TypeDescriptorCollection")>]
let ``JitCompiler basic compilation`` (tdesc: TypeDescriptor) =
    let fget, fset = { DeclaringType = tdesc
                       Verb          = BGet },
                     { DeclaringType = tdesc
                       Verb          = BGet }
    let nfget :: nfset :: _ = [fget; fset] |> List.map CompileFunction

    let p = GraphEngine.Jit.Native.Helper.malloc 4

    try
        GraphEngine.Jit.Native.Helper.Call(nfset.CallSite, p, 123)
        Assert.Equal(123,
                     GraphEngine.Jit.Native.Helper.Call(nfget.CallSite, p))
    finally
        Memory.free(p.ToPointer())


    //GraphEngine.Jit.Native.Helper.Call(nfget.CallSite, 