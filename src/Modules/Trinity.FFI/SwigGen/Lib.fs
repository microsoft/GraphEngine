namespace SwigGen

open System.IO

module Command = 
    (** 
    A default implementation of Command Line Parser.
    **)
    let (|Prefix|_|) (p : string) (s : string) = 
        if s.StartsWith(p) then Some(s.Substring(p.Length))
        else None
    
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
