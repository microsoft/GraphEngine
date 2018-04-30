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
open System.Runtime.InteropServices.ComTypes

let _AllocAccessor allocsize = 
    let p = Memory.malloc (uint64 allocsize) |> IntPtr
    {
        CellPtr    = p
        CellId     = 0L
        Size       = allocsize
        EntryIndex = 0
        Type       = 0us
        IsMalloc   = 1uy
        IsCell     = 0uy
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
        IsMalloc   = 1uy
        IsCell     = 0uy
    }

let _Compile (tdesc: TypeDescriptor) (vs: Verb list) =
    let fs = vs |> List.map (fun v -> {DeclaringType = tdesc; Verb = v} |> CompileFunction )
    for f in fs do
        Assert.NotEqual(IntPtr.Zero, f.CallSite)
    fs

let _AllocAccessorWithBNew (tdesc: TypeDescriptor) =
    let [bnew] = _Compile tdesc [BNew]
    let mutable accessor: NativeCellAccessor = {
        CellPtr = IntPtr.Zero
        CellId = 0L
        Type = 0us
        EntryIndex = -1
        Size = 0
        IsMalloc = 0uy
        IsCell = 1uy
    }
    let paccessor = &&accessor |> NativePtr.toNativeInt

    let ret = CallHelper.CallByVal<int32>(bnew.CallSite, paccessor)
    if ret <> 0 then
        failwith "nomem"
    accessor

let _BTest (getaccessor) (fn) =
    let mutable accessor = getaccessor()
    let paccessor        = &&accessor |> NativePtr.toNativeInt
    try
        fn accessor paccessor
    finally
        Memory.free(accessor.CellPtr.ToPointer())

let _Set (value: 'a) (site: IntPtr) (acc: IntPtr) = CallHelper.CallByVal(site, acc, value)

//  !Assume accessor.CellPtr is malloc'ed
let _BGetSet(tdesc: TypeDescriptor) (getaccessor) (set) (assert1) (assert2) =

    let [nfget; nfset] = _Compile tdesc [BGet; BSet] 

    _BTest getaccessor (fun accessor paccessor ->
        set nfset.CallSite paccessor     // setter invoke
        assert1 accessor.CellPtr         // manual inspect
        assert2 nfget.CallSite paccessor)// getter inspect 

let _AtomAssignTest (value: 'a) fn =
    fn  (_Set value)
        (fun p        -> Assert.Equal<'a>(value, NativePtr.read <| NativePtr.ofNativeInt<'a> p)) 
        (fun site acc -> Assert.Equal<'a>(value, CallHelper.CallByVal<'a>(site, acc)))

let _AtomBGetSet (tdesc: TypeDescriptor) (value: 'a) =
    _AtomAssignTest value <| _BGetSet tdesc (fun () -> _AllocAccessor sizeof<'a>)
    _AtomAssignTest value <| _BGetSet tdesc (fun () -> _AllocAccessorWithBNew tdesc)

// string setter expect "real" string
// string getter return "real" string
// but the accessor should contain a "tsl string"

let _StrTest (value: string) (strlen: int) (prawstr: nativeint) (ptslstr: nativeint) fn =
    fn
        (fun ()            -> _AllocAccessorWithHeader strlen )
        (fun site acc      -> CallHelper.CallByPtr(site, acc, prawstr))
        (fun (p:nativeint) -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), ptslstr.ToPointer(), uint64 (strlen + 4))))
        (fun site acc      -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc)))

let _StringBGetSet (tdesc: TypeDescriptor) (value: string) =
    let mutable _val = value
    use _pu16str     = fixed _val
    let lu16str      = value.Length * 2
    let pu16str      = AddTslHead (NativePtr.toNativeInt _pu16str) lu16str

    try
        _StrTest value lu16str (NativePtr.toNativeInt _pu16str) pu16str <| _BGetSet tdesc
    finally
        Memory.free(pu16str.ToPointer())

let _U8StrTest (value: string) (strlen: int) (prawstr: nativeint) (ptslstr: nativeint) fn =
    let mutable _val = value
    use _pu16str     = fixed _val

    fn
        (fun ()            -> _AllocAccessorWithHeader strlen )
        (fun site acc      -> CallHelper.CallByPtr(site, acc, (NativePtr.toNativeInt _pu16str)))
        (fun (p:nativeint) -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), ptslstr.ToPointer(), uint64 (strlen + 4))))
        (fun site acc      -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc)))


let _U8StringBGetSet (tdesc: TypeDescriptor) (value: string) = 
    let _pu8str      = ToUtf8 value
    let lu8str       = strlen _pu8str
    let pu8str       = AddTslHead _pu8str lu8str

    try 
        _U8StrTest value lu8str _pu8str pu8str <| _BGetSet tdesc
    finally
        Memory.free(_pu8str.ToPointer())
        Memory.free(pu8str.ToPointer())


[<Fact>]
let IntegerBGetBSet () =
    _AtomBGetSet (``TypeDescriptor allocation`` "U8") 255uy
    _AtomBGetSet (``TypeDescriptor allocation`` "I8") -71y

    _AtomBGetSet (``TypeDescriptor allocation`` "U16") 46131us
    _AtomBGetSet (``TypeDescriptor allocation`` "I16") -516s

    _AtomBGetSet (``TypeDescriptor allocation`` "U32") 123u
    _AtomBGetSet (``TypeDescriptor allocation`` "I32") 5108346

    _AtomBGetSet (``TypeDescriptor allocation`` "U64") 4968173491UL
    _AtomBGetSet (``TypeDescriptor allocation`` "I64") 6924560298457134L

[<Fact>]
let FloatBGetBSet () =
    _AtomBGetSet (``TypeDescriptor allocation`` "F32") 3.14f
    _AtomBGetSet (``TypeDescriptor allocation`` "F64") 12345678.90123

[<Fact>]
let StringBGetBSet () =
    _StringBGetSet (``TypeDescriptor allocation`` "STRING") "hello"
    _StringBGetSet (``TypeDescriptor allocation`` "STRING") "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
    _StringBGetSet (``TypeDescriptor allocation`` "STRING") "UTF8中文测试"

    _U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "hello"
    _U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
    _U8StringBGetSet (``TypeDescriptor allocation`` "U8STRING") "UTF8中文测试"

let Dump size p =
    printfn "========="
    printfn "Size: %X (%A)" size size
    printfn "Content:"
    printf "      00 01 02 03  04 05 06 07   08 09 0A 0B  0C 0D 0E 0F"
    for i in 0..(size-1) do
        if i % 16 = 0 then
            printfn ""
            printf "%02X " i
        if i % 8 = 0 then printf " "
        if i % 4 = 0 then printf " "
        printf " %02X" (NativePtr.read (NativePtr.add p i))
    printfn ""
    printfn "Content End"
    printfn "========="

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
        IsMalloc = 0uy
        IsCell = 1uy
    }

    let mutable p: nativeptr<byte> = NativePtr.ofNativeInt (nativeint 0)

    Global.LocalStorage.SaveGenericCell(cell) |> ignore
    let mutable arr = [| 0uy |]
    Global.LocalStorage.LoadCell(0L, &arr) |> ignore

    Global.LocalStorage.GetLockedCellInfo(0L, &accessor.Size, &accessor.Type, &p, &accessor.EntryIndex) |> ignore
    try
        Dump accessor.Size p
        accessor.CellPtr <- NativePtr.toNativeInt p
        let paccessor = &&accessor |> NativePtr.toNativeInt
        action tdesc accessor paccessor
    finally
        Global.LocalStorage.ReleaseCellLock(0L, accessor.EntryIndex)

let _SGetSet (cell: ICell) field (set) (assert1) (assert2) =

    _CellGetSet cell (fun (tdesc: TypeDescriptor) (accessor: NativeCellAccessor) (paccessor: nativeint)  ->

        let [nfbget; nfget; nfset] = _Compile tdesc [ ComposedVerb(SGet field, BGet)
                                                      SGet field 
                                                      SSet field ]

        printfn "paccessor  = %X" paccessor
        printfn "pcell      = %X" accessor.CellPtr
        printfn "pushed     = %X" (CallHelper.GetPushedPtr(nfget.CallSite, paccessor))

        printfn "set"
        set nfset.CallSite paccessor                                 // setter invoke
        printfn "assert1"
        let p = CallHelper.GetPushedPtr(nfget.CallSite, paccessor)
        assert1 (p) // manual inspect
        printfn "assert2"
        assert2 nfbget.CallSite paccessor                            // getter inspect
        )

let _IntegerSGetSet (cell: ICell) field (value: 'a) =
    _SGetSet cell field
        (fun site acc -> CallHelper.CallByVal(site, acc, value) )
        (fun p        -> Assert.Equal<'a>(value, NativePtr.read <| NativePtr.ofNativeInt<'a> p)) 
        (fun site acc -> Assert.Equal<'a>(value, CallHelper.CallByVal<'a>(site, acc)))

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
    //_StringSGetSet s1 "f2" "hello"
    //_StringSGetSet s1 "f2" "world"

    s1 <- S1(0L, 56256, "yargaaiawrguaw", 56892651)
    _StringSGetSet s1 "f2" "hello"
    _StringSGetSet s1 "f2" "world"

let _SLGetSet (cell: ICell) field index (set) (assert1) (assert2) =

    _CellGetSet cell (fun (tdesc: TypeDescriptor) (accessor: NativeCellAccessor) (paccessor: nativeint)  ->

        let [nfbget; nfget; nfset] = _Compile tdesc [ ComposedVerb(SGet field, ComposedVerb(LGet, BGet))
                                                      ComposedVerb(SGet field, LGet)
                                                      ComposedVerb(SGet field, LSet) ]

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
            (fun site acc       -> CallHelper.CallByPtr(site, acc, index, NativePtr.toNativeInt _pu16str))
            (fun (p: nativeint) -> Assert.Equal(0, Memory.memcmp(p.ToPointer(), pu16str.ToPointer(), uint64 (lu16str + 4))))
            (fun site acc       -> Assert.Equal(value, CallHelper.CallByVal<string>(site, acc, index)))
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

let _SLCount (cell: ICell) field (assert_len) =

    _CellGetSet cell (fun (tdesc: TypeDescriptor) (accessor: NativeCellAccessor) (paccessor: nativeint)  ->
        let fcnt = { DeclaringType = tdesc
                     Verb          = ComposedVerb(SGet field, LCount) }
        let nfcnt = CompileFunction fcnt

        Assert.NotEqual(IntPtr.Zero, nfcnt.CallSite)

        printfn "paccessor  = %X" paccessor
        printfn "pcell      = %X" accessor.CellPtr

        printfn "length assertion"
        Assert.Equal(assert_len, (CallHelper.CallByVal<int>(nfcnt.CallSite, paccessor)))
        )

[<Fact>]
let IntegerSLCount () =
    let mutable s2 = S2(0L, new System.Collections.Generic.List<int64>(seq [1L; 2L; 3L]), 123.456, 0)
    _SLCount s2 "f1" 3

    s2 <- S2(0L, new System.Collections.Generic.List<int64>(seq [1L; 2L; 3L; 5L; 8L]), 2333.33333, 0)
    _SLCount s2 "f1" 5

let _ListOpsTest
    (index : int32)
    (verbs : Verb list)
    (get   : nativeint -> nativeint -> int32 -> 'b)
    (insert: nativeint -> nativeint -> int32 -> bool) 
    (append: nativeint -> nativeint -> unit) 
    (remove: nativeint -> nativeint -> int32 -> bool)
    (inspect: 'b -> bool)
    (cmp: 'b -> 'b -> bool)
    (tdesc: TypeDescriptor)
    (accessor: NativeCellAccessor)
    (paccessor: nativeint) =
        let [fget; finsert; fcnt; fappend; fremove] = _Compile tdesc verbs

        printfn "paccessor  = %X" paccessor
        printfn "pcell      = %X" accessor.CellPtr
        printfn "index      = %d" index

        let mutable _accessor = NativePtr.read (NativePtr.ofNativeInt<NativeCellAccessor> paccessor)
        let mutable _size = _accessor.Size

        Dump _size (NativePtr.ofNativeInt<byte> _accessor.CellPtr)

        let len1 = CallHelper.CallByVal<int32>(fcnt.CallSite, paccessor)
        printfn "list len = %d" len1

        printfn "e[0] = %A" (CallHelper.CallByVal<'b>(fget.CallSite, paccessor, 0))
        printfn "e[1] = %A" (CallHelper.CallByVal<'b>(fget.CallSite, paccessor, 1))
        printfn "e[2] = %A" (CallHelper.CallByVal<'b>(fget.CallSite, paccessor, 2))
        //printfn "e[3] = %A" (CallHelper.CallByVal<string>(fget.CallSite, paccessor, 3))
        //printfn "e[4] = %A" (CallHelper.CallByVal<string>(fget.CallSite, paccessor, 4))


        let e = if len1 = 0 
                then Unchecked.defaultof<'b> 
                else printfn "get original"
                     let v = get fget.CallSite paccessor index
                     printfn "Original = %A" v
                     v

        printfn "insert"
        Assert.True(insert finsert.CallSite paccessor index)

        _accessor <- NativePtr.read (NativePtr.ofNativeInt<NativeCellAccessor> paccessor)
        _size <- _accessor.Size
        Dump _size (NativePtr.ofNativeInt<byte> _accessor.CellPtr)

        printfn "inspect insert"
        let v = get fget.CallSite paccessor index
        printfn "v = %A" v
        Assert.True(inspect <| v)
        if len1 <> 0 
        then printfn "cmp"
             Assert.True(cmp e (get fget.CallSite paccessor (index + 1)))

        printfn "append"
        append fappend.CallSite paccessor

        _accessor <- NativePtr.read (NativePtr.ofNativeInt<NativeCellAccessor> paccessor)
        _size <- _accessor.Size
        Dump _size (NativePtr.ofNativeInt<byte> _accessor.CellPtr)


        let len2 = CallHelper.CallByVal<int32>(fcnt.CallSite, paccessor)
        Assert.Equal(len1 + 2, len2)

        _accessor <- NativePtr.read (NativePtr.ofNativeInt<NativeCellAccessor> paccessor)
        _size <- _accessor.Size
        Dump _size (NativePtr.ofNativeInt<byte> _accessor.CellPtr)

        printfn "inspect append"
        Assert.True(inspect <| get fget.CallSite paccessor (len1 + 1))
        
        printfn "remove at end"
        Assert.True(remove fremove.CallSite paccessor (len1 + 1))

        let len3 = CallHelper.CallByVal<int32>(fcnt.CallSite, paccessor)
        Assert.Equal(len1 + 1, len3)

        printfn "remove at index"
        Assert.True(remove fremove.CallSite paccessor index)

        let len4 = CallHelper.CallByVal<int32>(fcnt.CallSite, paccessor)
        Assert.Equal(len1, len4)

        if len1 <> 0 
        then printfn "cmp again"
             Assert.True(cmp e (get fget.CallSite paccessor index))
        
let _SLOps cell field index get insert append remove inspect cmp = 
    let verbs = [ ComposedVerb(SGet field, ComposedVerb(LGet, BGet))
                  ComposedVerb(SGet field, LInsertAt) 
                  ComposedVerb(SGet field, LCount)
                  ComposedVerb(SGet field, LAppend)
                  ComposedVerb(SGet field, LRemoveAt) ]
    let test = _ListOpsTest index verbs
                            get insert append remove inspect cmp
    _CellGetSet cell test

let inline __SLOps (cell: ICell) field (index: int32) (value: 'a) =
    _SLOps cell field index
        (fun site acc i -> CallHelper.CallByVal<'a>(site, acc, i) )            // SLGet
        (fun site acc i -> CallHelper.CallByValBool<'a>(site, acc, i, value) ) // LInsert
        (fun site acc   -> CallHelper.CallByVal<'a>(site, acc, value) )        // LAppend
        (fun site acc i -> CallHelper.CallByValBool<'a>(site, acc, i) )        // LRemove
        (fun v          -> v = value )                                         // inspect
        (fun a b        -> a = b)

[<Fact>]
let IntegerSLOps () =
    let mutable s2 = S2(0L, new System.Collections.Generic.List<int64>(seq [1L; 2L; 3L]), 123.456, 0)

    __SLOps s2 "f1" 0 4L
    __SLOps s2 "f1" 1 5L
    __SLOps s2 "f1" 2 6L

    __SLOps s2 "f1" 0 7L
    __SLOps s2 "f1" 1 8L
    __SLOps s2 "f1" 2 9L

[<Fact>]
let FloatSLOps () =
    let mutable s4 = S4(0L, "S4 header", new System.Collections.Generic.List<float32>(seq [1.0f; 2.0f; 3.0f]), 123)

    __SLOps s4 "f2" 0 4.0f
    __SLOps s4 "f2" 1 5.0f
    __SLOps s4 "f2" 2 6.0f

    __SLOps s4 "f2" 0 7.0f
    __SLOps s4 "f2" 1 8.0f
    __SLOps s4 "f2" 2 9.0f

[<Fact>]
let StringSLOps () =
    let mutable s3 = S3(0L, 0xDEADBEEF, new System.Collections.Generic.List<string>(seq ["hello"; "world"; "people"]), 0x78563412)

    __SLOps s3 "f2" 0 "Trinity"
    __SLOps s3 "f2" 1 "Graph"
    __SLOps s3 "f2" 2 "Engine"

    __SLOps s3 "f2" 0 "Gets"
    __SLOps s3 "f2" 1 "JIT"
    __SLOps s3 "f2" 2 "超能力"


let _BCompare (tdesc: TypeDescriptor) (getaccessor) (small: 'a) (mid: 'a) (large: 'a) =
    printfn "BCompare %A: %A - %A - %A" typeof<'a> small mid large
    let [set; cmp; lt; le; gt; ge] = _Compile tdesc [BSet; BCmp; BLt; BLe; BGt; BGe] 
    printfn "Functions compiled"
    _BTest getaccessor (fun accessor paccessor ->
        let test = fun expect vv nf -> 
            printfn "Test %A %A" expect vv
            Assert.Equal(expect, CallHelper.CallCmp<'a>(nf.CallSite, paccessor, vv))

        printfn "Set small"
        CallHelper.CallByVal<'a>(set.CallSite, paccessor, small)

        test -1 large  cmp
        test -1 mid    cmp
        test 0  small  cmp

        test 1  large  lt
        test 1  mid    lt
        test 0  small  lt

        test 1  large  le
        test 1  mid    le
        test 1  small  le

        test 0  large  gt
        test 0  mid    gt
        test 0  small  gt

        test 0  large  ge
        test 0  mid    ge
        test 1  small  ge

        printfn "Set mid"
        CallHelper.CallByVal<'a>(set.CallSite, paccessor, mid)

        test -1 large  cmp
        test 0  mid    cmp
        test 1  small  cmp

        test 1  large  lt
        test 0  mid    lt
        test 0  small  lt

        test 1  large  le
        test 1  mid    le
        test 0  small  le

        test 0  large  gt
        test 0  mid    gt
        test 1  small  gt

        test 0  large  ge
        test 1  mid    ge
        test 1  small  ge

        printfn "Set large"
        CallHelper.CallByVal<'a>(set.CallSite, paccessor, large)

        test 0  large  cmp
        test 1  mid    cmp
        test 1  small  cmp

        test 0  large  lt
        test 0  mid    lt
        test 0  small  lt

        test 1  large  le
        test 0  mid    le
        test 0  small  le

        test 0  large  gt
        test 1  mid    gt
        test 1  small  gt

        test 1  large  ge
        test 1  mid    ge
        test 1  small  ge
        )

let inline _NumericBCompare (tdesc: TypeDescriptor) (value: 'a) (delta: 'a) = 
     _BCompare tdesc (fun () -> _AllocAccessor sizeof<'a>) (value-delta) (value) (value+delta)

[<Fact>]
let IntegerCompare() =
    let t_int32   = MakeFromType typeof<int32>
    let t_uint32  = MakeFromType typeof<uint32>
    let t_float32 = MakeFromType typeof<float32>
    let t_float   = MakeFromType typeof<float>

    let inline disturb (d: 'a) (fn: 'a -> unit) = fun () -> fn d 
    let tests = List.collect id [
        [  _NumericBCompare t_int32 0
           _NumericBCompare t_int32 405923845
           _NumericBCompare t_int32 -20514305
        ] |> List.map (disturb 1)
        [ _NumericBCompare t_uint32 1u
          _NumericBCompare t_uint32 49401345u
          _NumericBCompare t_uint32 90071u
        ] |> List.map (disturb 1u)
        [ _NumericBCompare t_float32 0.0f
          _NumericBCompare t_float32 3.14f
          _NumericBCompare t_float32 -45103.04352f
        ] |> List.map (disturb 1.0f)
        [ _NumericBCompare t_float 0.0
          _NumericBCompare t_float 2.54160571
          _NumericBCompare t_float -62039145.51201235
        ] |> List.map (disturb 1.0)
    ] 

    for t in tests do
        t()