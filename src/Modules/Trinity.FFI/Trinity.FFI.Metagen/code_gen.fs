module Trinity.FFI.MetaGen.code_gen

open FSharp.Data
open FSharp.NativeInterop
open GraphEngine.Jit.TypeSystem
open GraphEngine.Jit.Verbs
open System

type hashmap<'k, 'v> = System.Collections.Generic.(** argnum 1 *)
                                                  (** argnum 2 *)
                                                  (** argnum 0 *)
                                                  (** composed **)
                                                  // type: List_S
                                                  //Comp(LGet, Comp(SGet field, BGet))
                                                  //
                                                  //signature = List_S ^ "_" ^ LGet_S_SGet_field_Bet
                                                  //
                                                  //return : int
                                                  //argtypes: (*void, int)
                                                  //formal_args = (*void subject, int arg0)
                                                  //args = (subject, arg0)
                                                  //
                                                  //inner_call:
                                                  //    return static_cast<$return (*) ($argtypes)>(0x$addr) ($args);
                                                  Dictionary<'k, 'v>

let find (tb : ('k, 'v)hashmap) (k : 'k) : 'v option =
    if tb.ContainsKey(k) then Some(tb.[k])
    else None

type method_generator = nativeint -> string
type method_declaration = string

let mangling (name : string) = name.Replace("_", "__")

let rec ty_to_name (recursive_structure : bool) =
    function
    | { TypeCode = LIST; ElementType = elem } ->
        if recursive_structure then
            let elem = ty_to_name recursive_structure <| Seq.head elem
            in sprintf "list_%s" <| elem
        else "list"
    | { TypeCode = CELL _; TypeName = name } -> sprintf "cell_%s" <| mangling name
    | { TypeCode = STRUCT; TypeName = name } -> sprintf "struct_%s" <| mangling name
    | { TypeName = name } -> name.ToLower()

let null_type_string = "void"

let ty_to_string tydesc =
    match tydesc.TypeCode with
    | NULL -> null_type_string
    | U8 -> "uint8_t"
    | U16 -> "uint16_t"
    | U32 -> "uint32_t"
    | U64 -> "uint64_t"
    | I8 -> "int8_t"
    | I16 -> "int16_t"
    | I32 -> "int32_t"
    | I64 -> "int64_t"
    | F32 -> "float"
    | F64 -> "double"
    | BOOL -> "bool"
    | CHAR -> "char"
    | STRING -> "char*"
    | U8STRING -> "wchar*"
    | _ -> "void*"

let chaining_verb_to_name (verb : Verb) =
    match verb with
    | ComposedVerb(l, r) -> sprintf "compose_%A_%A" l r
    | SGet field -> sprintf "SGet_%s" field
    | SSet field -> sprintf "SSet_%s" field
    | _ -> sprintf "%A" verb

let rec count_verb_arg (verb : Verb) : int =
    match verb with
    | ComposedVerb(l, r) -> count_verb_arg l + count_verb_arg r
    | SSet _ | BSet | LGet | LRemoveAt | LAppend -> 1
    | LSet | LInsertAt -> 2
    | BGet | LCount | SGet _ -> 0
    | _ -> raise (NotImplementedException())

type FuncInfo =
    { name_sig : string
      pos_arg_types : string list
      ret_type : string }

let code_gen (tb : (string * string, FuncInfo) hashmap) (tydesc : TypeDescriptor) (verb : Verb) : method_declaration * method_generator =
    let no_recur_name = ty_to_name false

    let rec collect (tydesc : TypeDescriptor) verb : FuncInfo =
        let mutable name_sig : string = no_recur_name tydesc
        let get_elem_type = fun () -> Seq.head <| tydesc.ElementType

        let get_member_type =
            fun (field : string) ->
                let memb = tydesc.Members |> Seq.find (fun it -> it.Name = field)
                memb.Type
        match find tb (tydesc.QualifiedName, verb.ToString()) with
        | Some v -> raise (NotImplementedException())
        | _ ->
            match verb with
            | SSet field ->
                let memb_ty = get_member_type field
                [ ty_to_string memb_ty ], null_type_string
            | BSet ->
                let arg_type = ty_to_string tydesc
                [ arg_type ], null_type_string
            | LGet ->
                let elem_ty = get_elem_type()
                [ "int" ], ty_to_string elem_ty
            | LRemoveAt ->
                let elem_ty = get_elem_type()
                [ "int" ], "bool"
            | LAppend ->
                let elem_ty = get_elem_type()
                [ ty_to_string elem_ty ], null_type_string
            | LSet ->
                let elem_ty = get_elem_type()
                [ "int"
                  ty_to_string elem_ty ], null_type_string
            | LInsertAt ->
                let elem_ty = get_elem_type()
                [ "int"
                  ty_to_string elem_ty ], "bool"
            | BGet -> [], ty_to_string tydesc
            | LCount -> [], "int"
            | SGet field ->
                let memb_ty = get_member_type field
                [], ty_to_string memb_ty
            | ComposedVerb(l, r) ->
                match l with
                | SGet field ->
                    let memb_ty = get_member_type field
                    let { name_sig = append_name_sig; pos_arg_types = pos_arg_types; ret_type = ret_type } =
                        collect memb_ty r
                    name_sig <- sprintf "%s_%s" name_sig append_name_sig
                    pos_arg_types, ret_type
                | LGet ->
                    let elem_ty = get_elem_type()
                    let { name_sig = append_name_sig; pos_arg_types = pos_arg_types; ret_type = ret_type } =
                        collect elem_ty r
                    name_sig <- sprintf "%s_%s" name_sig append_name_sig
                    "int" :: pos_arg_types, ret_type
                | _ -> failwith "Only SGet/LGet requires method chaining composition."
            | BNew ->
                [], ty_to_string tydesc
            | _ -> raise (NotImplementedException())
            |> function
            | pos_arg_types, ret_type ->
                let result =
                    { name_sig = name_sig
                      pos_arg_types = pos_arg_types
                      ret_type = ret_type }
                tb.[(tydesc.QualifiedName, verb.ToString())] <- result
                result
    in
    let {name_sig = name_sig; pos_arg_types = pos_arg_types; ret_type = ret_type} = collect tydesc verb in
    let pos_arg_types = if verb = BNew then pos_arg_types else "*void"::pos_arg_types in
    let join (lst: string list) = String.Join(", ", lst) in

    let parameters = [for i in 1..(pos_arg_types.Length) -> sprintf "arg%d" i] in
    let typed_parameters =
                    List.zip pos_arg_types parameters
                    |> List.map (fun (parameter, ty_str) -> sprintf "%s %s" ty_str parameter)

    let types_string: string = join pos_arg_types
    let args_string: string = join parameters in
    let typed_args_string: string = join typed_parameters in
    let function_type_string = sprintf "%s (*)(%s)" ret_type types_string in
    let decl = sprintf "static %s %s(%s);" ret_type name_sig types_string in
    let generator id =
        sprintf "
        static %s %s(%s){
            return static_cast<%s>(0x%xll)(%s);
        }
        " ret_type name_sig typed_args_string function_type_string id args_string
    in (decl, generator)
