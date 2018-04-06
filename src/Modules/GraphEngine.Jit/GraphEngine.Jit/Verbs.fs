namespace GraphEngine.Jit

open GraphEngine.Jit.TypeSystem
open GraphEngine.Jit.TypeSystem.Builder
open System

//  !Note, only BGet should generate code to extract data from Trinity;
//   Other getters should only be interpreted as "getting the accessor of a type"
//   For example, let t denote a type descriptor of a struct.
//   SGet t "field" will yield the accessor of the field type, and another BGet on
//   the type should then yield the real value.
//   Further getters can be applied to the result to build up complex getter without
//   actually getting the whole value out into the runtime.
type BasicVerb  = BGet 
                | BSet

type ListVerb   = LInlineGet of int 
                | LInlineSet of int
                | LGet 
                | LSet
                | LContains
                | LCount

type StructVerb = SGet of string
                | SSet of string

type GenericStructVerb = 
                | GSGet of TypeDescriptor
                | GSSet of TypeDescriptor

type EnumeratorVerb =
                | EAlloc
                | EFree
                | ENext
                | ECurrent

type Verb       = Basic of BasicVerb
                | List of ListVerb
                | Struct of StructVerb
                | GStruct of GenericStructVerb
                | Enum of EnumeratorVerb

type FunctionDescriptor = {
    DeclaringType : TypeDescriptor
    Verb          : Verb
}

module VerbTraits = 
    let private MemberType x name = x.DeclaringType.Members |> Seq.find(fun m -> m.Name = name ) |> (fun m -> m.Type)

    let Inputs (x: FunctionDescriptor) =
        let listElem = x.DeclaringType.ElementType |> Seq.tryHead
        match x.Verb with
        | Basic BGet           -> seq []
        | Basic BSet           -> seq [ x.DeclaringType ]

        | List  LGet           -> seq [ MakeFromType(typeof<int32>) ]
        | List  LSet           -> seq [ MakeFromType(typeof<int32>); listElem.Value ]
        | List  (LInlineGet i) -> seq []
        | List  (LInlineSet i) -> seq [ listElem.Value ]
        | List  LContains      -> seq [ listElem.Value ]
        | List  LCount         -> seq [ MakeFromType(typeof<int32>); listElem.Value ]

        | Struct (SGet name)   -> seq []
        | Struct (SSet name)   -> seq [ MemberType x name ]

        | GStruct (GSGet t)    -> seq [ MakeFromType(typeof<string>) ]
        | GStruct (GSSet t)    -> seq [ MakeFromType(typeof<string>); t ]

        | Enum  EAlloc         -> seq []
        | Enum  EFree          -> seq []
        | Enum  ENext          -> seq []
        | Enum  ECurrent       -> seq []

    let Output (x: FunctionDescriptor) =
        let listElem = x.DeclaringType.ElementType |> Seq.tryHead
        let tUnit = MakeFromType(typeof<Unit>)
        let tInt = MakeFromType(typeof<int32>)
        let tBool = MakeFromType(typeof<bool>)
        let tString = MakeFromType(typeof<string>)
        let tObject = MakeFromType(typeof<Object>)
        let tX = x.DeclaringType

        match x.Verb with
        | Basic BGet           -> tX
        | Basic BSet           -> tUnit

        | List  LGet           -> listElem.Value
        | List  LSet           -> tUnit
        | List  (LInlineGet i) -> listElem.Value
        | List  (LInlineSet i) -> tUnit
        | List  LContains      -> tBool
        | List  LCount         -> tInt

        | Struct (SGet name)   -> MemberType x name
        | Struct (SSet name)   -> tUnit

        | GStruct (GSGet t)    -> t
        | GStruct (GSSet t)    -> tUnit

        | Enum  EAlloc         -> tObject
        | Enum  EFree          -> tUnit
        | Enum  ENext          -> tBool
        | Enum  ECurrent       -> tObject // XXX strong type lost, consider add enumerator to TypeDescriptor

    let GetFunctions (t: TypeDescriptor) = seq {
        yield ()
    }
