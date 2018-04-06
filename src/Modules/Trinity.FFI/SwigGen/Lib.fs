namespace SwigGen

open System.IO


module Operator = 
    let (->>) (key: string) (value: 'T) = (key, value.ToString())
    let (>>>) (head: 'T) (tail: seq<'T>) =  Seq.append (head |> Seq.singleton) tail
    let (<^.^^>) (left: 'T) (right: 'G) = (left, right)
    let (<^^.^>) (right: 'T) (left: 'G) = (left, right)

module PString = 
    (** python like string utilities **)
    open System
    
    let (|Prefix|_|) (p : string) (s : string) = 
        if s.StartsWith(p) then Some(s.Substring(p.Length))
        else None
    
    type StrOrChr = 
        | Chr of char
        | Str of string
    
    let str'concat (xs : seq<'T>) = String.Join("", xs)

    let str'concatBy (sep: string) (xs: seq<'T>) = String.Join(sep, xs)
    
    let rev'concat (xs : seq<'T>) = 
        xs
        |> Seq.rev
        |> str'concat
    
    let rec format'root (template : List<char>) (result : List<string>) (kvpairs : Map<string, string>) 
            (cache : List<char>) : string = 
        match template with
        | '{'  :: '{' :: tail -> format'root tail result kvpairs ('{' :: cache)
        | '{'  ::        tail -> 
            if cache.IsEmpty 
            then 
                format'render tail result kvpairs []
            else 
                format'render tail ((cache |> rev'concat) :: result) kvpairs []
        | '\\' :: chr :: tail -> format'root tail result kvpairs (chr :: cache)
        | '}'  :: '}' :: tail -> format'root tail result kvpairs ('}' :: cache)
        | '}'  :: _           -> failwith "missing pair parentheses."
        | chr  ::        tail -> format'root tail result kvpairs (chr :: cache)
        | [] -> 
            if cache.IsEmpty 
            then 
                result |> rev'concat
            else 
                (cache |> rev'concat) :: result |> rev'concat
    
    and format'render (chrs : List<char>) (result : List<string>) (kvpairs : Map<string, string>) (name : List<char>) = 
        match chrs with
        | '}' :: tail -> 
            if name.IsEmpty 
            then 
                failwith "empty name cannot be render."
            else 
                name
                |> rev'concat
                |> kvpairs.TryFind
                |> (fun some -> 
                if some.IsNone 
                then 
                    failwith "unexpected keyword args"
                else 
                    some.Value)
                    |> (fun value -> format'root tail (value :: result) kvpairs [])
                    
        | chr :: tail -> format'render tail result kvpairs (chr :: name)
        | _           -> failwith "unsolved error."
    
    let format (template : string) (kvpairs : Map<string, string>) : string = 
        format'root (template.ToCharArray() |> List.ofArray) [] kvpairs []

module Command = 
    (** 
    A default implementation of Command Line Parser.
    **)
    open PString
    
    type Argument = 
        | Optional of string
        | KeyValue of string * string
        | Varadic of List<string>
    
    type ArgInfo = 
        | On
        | Off
        | Varargs of List<string>
        | Value of string
    
    let rec extract (args : List<string>) : List<Argument> = 
        match args with
        | (Prefix "--" head) :: tail -> optionalExtract (head :: tail)
        | (Prefix "-" head) :: tail -> keyValueExtract (head :: tail)
        | head :: tail -> varadicExtract tail (Varadic [ head ])
        | [] -> []
    
    and optionalExtract (head :: tail) : List<Argument> = (Optional head) :: (extract tail)
    
    (** 
        The pattern must match `head::tail` 
        because `optionalExtract` is called within only this case.
    **)

    and keyValueExtract args : List<Argument> = 
        match args with
        | key :: value :: tail -> (KeyValue(key, value)) :: (extract tail)
        | _ -> failwith "unrecognized keyvalue argument"
    
    and varadicExtract (args : List<string>) ((Varadic lst) as varargs) : List<Argument> = 
        (** 
        Just like `optionalExtract`,
        the pattern must match `Varadic lst`. It's only called in this case. 
    **)
        match args with
        | (Prefix "--" head) :: tail -> varargs :: (optionalExtract (head :: tail))
        | (Prefix "-" head) :: tail -> varargs :: (keyValueExtract (head :: tail))
        | arg :: tail -> varadicExtract tail (Varadic(arg :: lst))
        | [] -> [ varargs ]
        | _ -> failwith "unknown exception"
    
    let parse (args : string []) : Map<string, ArgInfo> = 
        Map.ofList (List.map (fun arg -> 
                        match arg with
                        | Optional str -> (str, On)
                        | KeyValue(k, v) -> (k, Value v)
                        | Varadic many -> ("varadic", Varargs many)) (extract (args |> List.ofArray)))

module IO = 
    let exists filename = filename |> File.Exists
    let write filename content = File.WriteAllText(filename, content)
    let readlines filename = File.ReadAllLines(filename)
    let read filename = File.ReadAllText(filename)


//
//module Main = 
//
//    [<EntryPoint>]
//    let main (argv: string[]) = 
//        let myMap = Map.ofList [("1", "number1") ; ("2", "number2"); ("qwe???", "红可太秀了")]
//        let s = PString.format "{1} some some some; {qwe???} some some some \\{ {2}  {{{1}}} some " myMap
//        System.Console.WriteLine(s) 
//        0












