module GraphEngine.Jit.Verbs

open System
open TypeSystem

//  !Note, only BGet should generate code to extract data from Trinity;
//   Other getters should only be interpreted as "getting the accessor of a type"
//   For example, let t denote a type descriptor of a struct.
//   SGet t "field" will yield the accessor of the field type, and another BGet on
//   the type should then yield the real value.
//   Further getters can be applied to the result to build up complex getter without
//   actually getting the whole value out into the runtime.

type Verb =
    (** BasicVerb **)
    | BGet
    | BSet
    (** ListVerb **)
    | LInlineGet of int // get value at a constant index
    | LInlineSet of int // set value at a constant index
    | LGet
    | LSet
    | LContains
    | LCount
    (** StructVerb **)
    | SGet of string
    | SSet of string
    (** GenericStructVerb **)
    | GSGet of TypeDescriptor
    | GSSet of TypeDescriptor
    (** EnumeratorVerb **)
    | EAlloc
    | EFree
    | ENext
    | ECurrent
    (** ComposedVerb **)
    | ComposedVerb of array<Verb>

type FunctionDescriptor = {
    DeclaringType : TypeDescriptor
    Verb          : Verb
}

type FunctionSignature(fdesc: FunctionDescriptor) =

    let listElem = fdesc.DeclaringType.ElementType |> Seq.tryHead
    let tUnit = MakeFromType(typeof<Unit>)
    let tInt = MakeFromType(typeof<int32>)
    let tBool = MakeFromType(typeof<bool>)
    let tString = MakeFromType(typeof<string>)
    let tX = fdesc.DeclaringType

    member private x.MemberType name = fdesc.DeclaringType.Members |> Seq.find(fun m -> m.Name = name ) |> (fun m -> m.Type)

    member x.Input : seq<TypeDescriptor> = 
        let listElem = fdesc.DeclaringType.ElementType |> Seq.tryHead
        match fdesc.Verb with
        | BGet           -> seq []
        | BSet           -> seq [ fdesc.DeclaringType ]

        | LGet           -> seq [ MakeFromType(typeof<int32>) ]
        | LSet           -> seq [ MakeFromType(typeof<int32>); listElem.Value ]
        | LInlineGet _   -> seq []
        | LInlineSet _   -> seq [ listElem.Value ]
        | LContains      -> seq [ listElem.Value ]
        | LCount         -> seq [ MakeFromType(typeof<int32>); listElem.Value ]

        | SGet _         -> seq []
        | SSet name      -> seq [ x.MemberType name ]

        | GSGet _        -> seq [ MakeFromType(typeof<string>) ]
        | GSSet t        -> seq [ MakeFromType(typeof<string>); t ]

        | EAlloc         -> seq []
        | EFree          -> seq []
        | ENext          -> seq []
        | ECurrent       -> seq []
    member x.Output: TypeDescriptor =

        match fdesc.Verb with
        | BGet           -> tX
        | BSet           -> tUnit

        | LGet           -> listElem.Value
        | LSet           -> tUnit
        | LInlineGet i   -> listElem.Value
        | LInlineSet i   -> tUnit
        | LContains      -> tBool
        | LCount         -> tInt

        | SGet name      -> x.MemberType name
        | SSet _         -> tUnit

        | GSGet t        -> t
        | GSSet _        -> tUnit

        | EAlloc         -> tUnit // XXX
        | EFree          -> tUnit
        | ENext          -> tBool
        | ECurrent       -> tUnit // XXX

        | ComposedVerb _ -> failwith "notimplemented"


