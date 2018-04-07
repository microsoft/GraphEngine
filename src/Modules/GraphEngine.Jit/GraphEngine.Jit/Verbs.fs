namespace GraphEngine.Jit

open GraphEngine.Jit.TypeSystem
open System

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
    | LInlineGet of int // get value at a const index
    | LInlineSet of int // set value at a const index
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

type FunctionDescriptor = {
    DeclaringType : TypeDescriptor
    Verb          : Verb
}

module VerbTraits = 
    let private MemberType x name = x.DeclaringType.Members |> Seq.find(fun m -> m.Name = name ) |> (fun m -> m.Type)

    let Inputs (x: FunctionDescriptor) =
        let listElem = x.DeclaringType.ElementType |> Seq.tryHead
        match x.Verb with
        | BGet           -> seq []
        | BSet           -> seq [ x.DeclaringType ]

        | LGet           -> seq [ MakeFromType(typeof<int32>) ]
        | LSet           -> seq [ MakeFromType(typeof<int32>); listElem.Value ]
        | LInlineGet i   -> seq []
        | LInlineSet i   -> seq [ listElem.Value ]
        | LContains      -> seq [ listElem.Value ]
        | LCount         -> seq [ MakeFromType(typeof<int32>); listElem.Value ]

        | SGet name      -> seq []
        | SSet name      -> seq [ MemberType x name ]

        | GSGet t        -> seq [ MakeFromType(typeof<string>) ]
        | GSSet t        -> seq [ MakeFromType(typeof<string>); t ]

        | EAlloc         -> seq []
        | EFree          -> seq []
        | ENext          -> seq []
        | ECurrent       -> seq []

    let Output (x: FunctionDescriptor) =
        let listElem = x.DeclaringType.ElementType |> Seq.tryHead
        let tUnit = MakeFromType(typeof<Unit>)
        let tInt = MakeFromType(typeof<int32>)
        let tBool = MakeFromType(typeof<bool>)
        let tString = MakeFromType(typeof<string>)
        let tObject = MakeFromType(typeof<Object>)
        let tX = x.DeclaringType

        match x.Verb with
        | BGet           -> tX
        | BSet           -> tUnit

        | LGet           -> listElem.Value
        | LSet           -> tUnit
        | LInlineGet i   -> listElem.Value
        | LInlineSet i   -> tUnit
        | LContains      -> tBool
        | LCount         -> tInt

        | SGet name      -> MemberType x name
        | SSet name      -> tUnit

        | GSGet t        -> t
        | GSSet t        -> tUnit

        | EAlloc         -> tObject
        | EFree          -> tUnit
        | ENext          -> tBool
        | ECurrent       -> tObject // XXX strong type lost, consider add enumerator to TypeDescriptor

