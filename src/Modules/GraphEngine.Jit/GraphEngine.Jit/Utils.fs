module Utils

open Microsoft.FSharp.Reflection

let ParseCase<'a> (value: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun x -> x.Name = value) with
    | [| case |] -> (FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _          -> failwith "Cannot parse union case"

let ToStringCase (value: 'a) = 
    let caseInfo, _ = FSharpValue.GetUnionFields(value, typeof<'a>)
    caseInfo.Name