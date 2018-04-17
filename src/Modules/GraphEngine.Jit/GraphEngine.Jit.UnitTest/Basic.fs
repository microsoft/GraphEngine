module GraphEngine.Jit.Basic

#nowarn "9"

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
open GraphEngine.Jit
open GraphEngine.Jit.Helper
open GraphEngine.Jit.TSL
open Trinity

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
let ``TypeDescriptor allocation from Type`` (t: System.Type) (w: int32) (tt: string) =
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

let ICellDescriptorGen() = 
    let _seq: obj array seq = seq {
        yield [|"C1"; new List<IFieldDescriptor>(); 0us|]
        yield! (Global.StorageSchema.CellDescriptors |> Seq.map<ICellDescriptor, obj array> 
            (fun x -> [| x.TypeName; x.GetFieldDescriptors(); x.CellType |]))
    }
    Array.ofSeq _seq

let TypeDescriptorCollection() : obj array array = 
    [| 
      [| ``TypeDescriptor allocation`` "U32" |]
      [| ``TypeDescriptor allocation`` "I16" |]
      [| ``TypeDescriptor allocation`` "U8"  |]
    |] 

[<Theory>]
[<MemberData("ICellDescriptorGen")>]
let ``TypeDescriptor allocation from ICellDescriptor`` (a,b,c) = (a,b,c) |> celldesc |> Make

[<Theory>]
[<MemberData("ICellDescriptorGen")>]
let ``NativeTypeDescriptor allocation from ICellDescriptor`` (a,b,c) = (a,b,c) |> celldesc |> Make |> JitNativeInterop.TypeDescriptorToNative

[<Theory>]
[<MemberData("TypeDescriptorCollection")>]
let ``FunctionDescriptor allocation``(tdesc: TypeDescriptor) =
    { DeclaringType = tdesc
      Verb          = BGet }

open JitNativeInterop
open System.Runtime.InteropServices
open Trinity.Diagnostics

let _AllocAccessor allocsize = 
    let p = Memory.malloc (uint64 allocsize) |> IntPtr
    {
        CellPtr    = p
        CellId     = 0L
        Size       = allocsize
        EntryIndex = 0
        Type       = 0us
    }

let _AllocAccessorWithHeader allocsize = 
    let p = Memory.malloc (uint64 (allocsize + 4)) |> IntPtr
    NativePtr.write (NativePtr.ofNativeInt<int> p) allocsize
    {
        CellPtr    = p
        CellId     = 0L
        Size       = allocsize + 4
        EntryIndex = 0
        Type       = 0us
    }

//  !Assume accessor.CellPtr is malloc'ed
let _BGetSet(tdesc: TypeDescriptor) (getaccessor) (set) (assert1) (assert2) =
    let fget, fset = { DeclaringType = tdesc
                       Verb          = BGet },
                     { DeclaringType = tdesc
                       Verb          = BSet }
    let [nfget; nfset] = [fget; fset] |> List.map CompileFunction

    Assert.NotEqual(IntPtr.Zero, nfget.CallSite)
    Assert.NotEqual(IntPtr.Zero, nfset.CallSite)

    let mutable accessor = getaccessor()
    let paccessor        = &&accessor |> NativePtr.toNativeInt
    try
        set nfset.CallSite paccessor     // setter invoke
        assert1 accessor.CellPtr         // manual inspect
        assert2 nfget.CallSite paccessor // getter inspect
    finally
        Memory.free(accessor.CellPtr.ToPointer())

let _IntegerTest (value: 'a) fn =
    fn  (fun site acc -> CallHelper.CallByVal(site, acc, value))
        (fun p        -> Assert.Equal<'a>(value, NativePtr.read <| NativePtr.ofNativeInt<'a> p)) 
        (fun site acc -> Assert.Equal<'a>(value, CallHelper.CallByVal<'a>(site, acc)))
        

let _IntegerBGetSet (tdesc: TypeDescriptor) (value: 'a) =
    _IntegerTest value <| _BGetSet tdesc (fun () -> _AllocAccessor sizeof<'a>)

let _FloatBGetSet = _IntegerBGetSet

// string setter expect "real" string
// string getter return "real" string
// but the accessor should contain a "tsl string"

let _StrTest (value: string) (strlen: int) (prawstr: nativeint) (ptslstr: nativeint) fn =
    fn
        (fun ()            -> _AllocAccessorWithHeader strlen)
        (fun site acc      -> CallHelper.CallByPtr(site, acc, prawstr))
        (fun (p:nativeint) -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), ptslstr.ToPointer(), uint64 (strlen + 4))))
        (fun site acc      -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc)))

let _U8StringBGetSet (tdesc: TypeDescriptor) (value: string) = 
    let _pu8str      = ToUtf8 value
    let lu8str       = strlen _pu8str
    let pu8str       = AddTslHead _pu8str lu8str

    try 
        _StrTest value lu8str _pu8str pu8str <| _BGetSet tdesc
    finally
        Memory.free(_pu8str.ToPointer())
        Memory.free(pu8str.ToPointer())

let _StringBGetSet (tdesc: TypeDescriptor) (value: string) =
    let mutable _val = value
    use _pu16str     = fixed _val
    let lu16str      = value.Length * 2
    let pu16str      = AddTslHead (NativePtr.toNativeInt _pu16str) lu16str

    try
        _StrTest value lu16str (NativePtr.toNativeInt _pu16str) pu16str <| _BGetSet tdesc
    finally
        Memory.free(pu16str.ToPointer())

[<Fact>]
let IntegerBGetBSet () =
    _IntegerBGetSet (``TypeDescriptor allocation`` "U8") 255uy
    _IntegerBGetSet (``TypeDescriptor allocation`` "I8") -71y

    _IntegerBGetSet (``TypeDescriptor allocation`` "U16") 46131us
    _IntegerBGetSet (``TypeDescriptor allocation`` "I16") -516s

    _IntegerBGetSet (``TypeDescriptor allocation`` "U32") 123u
    _IntegerBGetSet (``TypeDescriptor allocation`` "I32") 5108346

    _IntegerBGetSet (``TypeDescriptor allocation`` "U64") 4968173491UL
    _IntegerBGetSet (``TypeDescriptor allocation`` "I64") 6924560298457134L

[<Fact>]
let FloatBGetBSet () =
    _FloatBGetSet (``TypeDescriptor allocation`` "F32") 3.14f
    _FloatBGetSet (``TypeDescriptor allocation`` "F64") 12345678.90123

[<Fact>]
let StringBGetBSet () =
    _StringBGetSet (``TypeDescriptor allocation`` "STRING") "hello"
    _StringBGetSet (``TypeDescriptor allocation`` "STRING") "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
    //_StringBGetSet (``TypeDescriptor allocation`` "STRING") "UTF8中文测试"

    _U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "hello"
    _U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
    //_U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "UTF8中文测试"

//  !All cell-tests go through this routine instead of _BGetSet
let _CellGetSet (cell: ICell) action =
    let cdesc = cell :> ICellDescriptor
    let tdesc = TypeSystem.Make cdesc
    let mutable accessor: NativeCellAccessor = {
        CellPtr = IntPtr.Zero
        CellId = 0L
        Type = 0us
        EntryIndex = -1
        Size = 0
    }

    let mutable p: nativeptr<byte> = NativePtr.ofNativeInt (nativeint 0)

    Global.LocalStorage.SaveGenericCell(cell) |> ignore
    let mutable arr = [| 0uy |]
    Global.LocalStorage.LoadCell(0L, &arr) |> ignore

    Global.LocalStorage.GetLockedCellInfo(0L, &accessor.Size, &accessor.Type, &p, &accessor.EntryIndex) |> ignore
    try
        printfn ""
        printfn "Content:"
        Array.mapi (printfn "%02x %02x") [| for i in 0..accessor.Size -> NativePtr.read (NativePtr.add p i) |]
        printfn "Content End"
        printfn ""
        accessor.CellPtr <- NativePtr.toNativeInt p
        let paccessor = &&accessor |> NativePtr.toNativeInt
        action tdesc accessor paccessor
    finally
        Global.LocalStorage.ReleaseCellLock(0L, accessor.EntryIndex)

let _SGetSet (cell: ICell) field (set) (assert1) (assert2) =

    _CellGetSet cell (fun (tdesc: TypeDescriptor) (accessor: NativeCellAccessor) (paccessor: nativeint)  ->
        let fbget, fget, fset = 
                         { DeclaringType = tdesc
                           Verb          = ComposedVerb(SGet field, BGet) },
                         { DeclaringType = tdesc
                           Verb          = SGet field },
                         { DeclaringType = tdesc
                           Verb          = SSet field }
        let [nfbget; nfget; nfset] = [fbget; fget; fset] |> List.map CompileFunction

        Assert.NotEqual(IntPtr.Zero, nfbget.CallSite)
        Assert.NotEqual(IntPtr.Zero, nfget.CallSite)
        Assert.NotEqual(IntPtr.Zero, nfset.CallSite)

        printfn "paccessor  = %X" paccessor
        printfn "pcell      = %X" accessor.CellPtr
        printfn "pushed     = %X" (CallHelper.GetPushedPtr(nfget.CallSite, paccessor))

        printfn "set"
        set nfset.CallSite paccessor                                 // setter invoke
        printfn "assert1"
        assert1 (CallHelper.GetPushedPtr(nfget.CallSite, paccessor)) // manual inspect
        printfn "assert2"
        assert2 nfbget.CallSite paccessor                            // getter inspect
        )

let _IntegerSGetSet (cell: ICell) field (value: 'a) =
    _SGetSet cell field
        (fun site acc -> CallHelper.CallByVal(site, acc, value) )
        (fun p        -> Assert.Equal<'a>(value, NativePtr.read <| NativePtr.ofNativeInt<'a> p)) 
        (fun site acc -> Assert.Equal<'a>(value, CallHelper.CallByVal<'a>(site, acc)))

let _U8StringSGetSet (cell: ICell) field (value: string) = 
    let _pu8str      = ToUtf8 value
    let lu8str       = strlen _pu8str
    let pu8str       = AddTslHead _pu8str lu8str

    try
        _SGetSet cell field
            (fun site acc -> CallHelper.CallByPtr(site, acc, _pu8str))
            (fun p        -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), pu8str.ToPointer(), uint64 (lu8str + 4))))
            (fun site acc -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc)))
    finally
        Memory.free(_pu8str.ToPointer())
        Memory.free(pu8str.ToPointer())

let _StringSGetSet (cell: ICell) field (value: string)  =
    let mutable _val = value
    use _pu16str     = fixed _val
    let lu16str      = value.Length * 2
    let pu16str      = AddTslHead (NativePtr.toNativeInt _pu16str) lu16str

    try
        _SGetSet cell field
            (fun site acc -> CallHelper.CallByPtr(site, acc, NativePtr.toNativeInt _pu16str))
            (fun p        -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), pu16str.ToPointer(), uint64 (lu16str + 4))))
            (fun site acc -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc)))
    finally
        Memory.free(pu16str.ToPointer())

[<Fact>]
let IntegerSGetSet () =
    let mutable s1 = S1(0L, 641934, "", 123)
    _IntegerSGetSet s1 "f1" 465912345
    _IntegerSGetSet s1 "f3" 13451

    s1 <- S1(0L, 56256, "yargaaiawrguaw", 56892651)
    _IntegerSGetSet s1 "f1" 652634
    _IntegerSGetSet s1 "f3" 461371

    let mutable s2 = S2(0L, new System.Collections.Generic.List<int64>(seq [1L; 2L; 3L]), 123.456, 0)
    _IntegerSGetSet s2 "f3" 451

[<Fact>]
let StringSGetSet () =
    let mutable s1 = S1(0L, 641934, "", 123)
    _StringSGetSet s1 "f2" "hello"
    _StringSGetSet s1 "f2" "world"

    s1 <- S1(0L, 56256, "yargaaiawrguaw", 56892651)
    _StringSGetSet s1 "f2" "hello"
    _StringSGetSet s1 "f2" "world"

let _SLGetSet (cell: ICell) field index (set) (assert1) (assert2) =

    _CellGetSet cell (fun (tdesc: TypeDescriptor) (accessor: NativeCellAccessor) (paccessor: nativeint)  ->
        let fbget, fget, fset = 
                         { DeclaringType = tdesc
                           Verb          = ComposedVerb(SGet field, ComposedVerb(LGet, BGet)) },
                         { DeclaringType = tdesc
                           Verb          = ComposedVerb(SGet field, LGet) },
                         { DeclaringType = tdesc
                           Verb          = ComposedVerb(SGet field, LSet) }
        let [nfbget; nfget; nfset] = [fbget; fget; fset] |> List.map CompileFunction

        Assert.NotEqual(IntPtr.Zero, nfbget.CallSite)
        Assert.NotEqual(IntPtr.Zero, nfget.CallSite)
        Assert.NotEqual(IntPtr.Zero, nfset.CallSite)

        printfn "paccessor  = %X" paccessor
        printfn "pcell      = %X" accessor.CellPtr
        printfn "pushed     = %X" (CallHelper.GetPushedPtr(nfget.CallSite, paccessor, index))

        printfn "set"
        set nfset.CallSite paccessor                                 // setter invoke
        printfn "assert1"
        assert1 (CallHelper.GetPushedPtr(nfget.CallSite, paccessor, index)) // manual inspect
        printfn "assert2"
        assert2 nfbget.CallSite paccessor                            // getter inspect
        )

let _IntegerSLGetSet (cell: ICell) field (index: int32) (value: 'a) =
    _SLGetSet cell field index
        (fun site acc -> CallHelper.CallByVal(site, acc, index, value) )
        (fun p        -> Assert.Equal<'a>(value, NativePtr.read <| NativePtr.ofNativeInt<'a> p)) 
        (fun site acc -> Assert.Equal<'a>(value, CallHelper.CallByVal<'a>(site, acc, index)))

let _StringSLGetSet (cell: ICell) field (index: int32) (value: string) =
    let mutable _val = value
    use _pu16str     = fixed _val
    let lu16str      = value.Length * 2
    let pu16str      = AddTslHead (NativePtr.toNativeInt _pu16str) lu16str

    try
        _SLGetSet cell field index
            (fun site acc -> CallHelper.CallByPtr(site, acc, index, NativePtr.toNativeInt _pu16str))
            (fun (p: nativeint) -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), pu16str.ToPointer(), uint64 (lu16str + 4))))
            (fun site acc -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc, index)))
    finally
        Memory.free(pu16str.ToPointer())

[<Fact>]
let IntegerSLGetSet () =
    let mutable s2 = S2(0L, new System.Collections.Generic.List<int64>(seq [1L; 2L; 3L]), 123.456, 0)

    _IntegerSLGetSet s2 "f1" 0 4L
    _IntegerSLGetSet s2 "f1" 1 5L
    _IntegerSLGetSet s2 "f1" 2 6L

    _IntegerSLGetSet s2 "f1" 0 7L
    _IntegerSLGetSet s2 "f1" 1 8L
    _IntegerSLGetSet s2 "f1" 2 9L

[<Fact>]
let StringSLGetSet () = 
    let mutable s3 = S3(0L, 0, new System.Collections.Generic.List<string>(seq [""; ""]), 0)
    _StringSLGetSet s3 "f2" 0 "hello"
    _StringSLGetSet s3 "f2" 1 "world!"