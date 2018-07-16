module Trinity.FFI.MetaGen.code_gen

open GraphEngine.Jit.JitCompiler
open GraphEngine.Jit.TypeSystem
open GraphEngine.Jit.Verbs
open System
open Trinity.FFI.Metagen.PString
open Trinity.FFI.Metagen

type hashmap<'k, 'v> = System.Collections.Generic.Dictionary<'k, 'v>

let find (tb : ('k, 'v)hashmap) (k : 'k) : 'v option =
    if tb.ContainsKey(k) then Some(tb.[k])
    else None

type method_generator = Int64 -> string

type method_declaration = string

type method_code = string

let mangling (name : string) = name.Replace("_", "__")

let rec ty_to_name (recursive_structure : bool) =
    function
    | { TypeCode = LIST; ElementType = elem } ->
        if recursive_structure then
            let elem = ty_to_name recursive_structure <| Seq.head elem
            sprintf "list_%s" <| elem
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

type FuncInfo =
    { name_sig : string
      pos_arg_types : string list
      ret_type : string
      verb_str_lst : string list }

let single_method'code_gen (tb : ((string * string), FuncInfo)hashmap) (tydesc : TypeDescriptor) (verb : Verb) : method_declaration * method_generator =
    let no_recur_name = ty_to_name false
    let recur_name = ty_to_name true

    let rec collect (tydesc : TypeDescriptor) verb : FuncInfo =

        let name_sig : string = no_recur_name tydesc

        let mutable name_sig_lsts : string list option = None

        let get_elem_type = fun () -> Seq.head <| tydesc.ElementType

        let get_member_type =
            fun (field : string) ->
                let memb = tydesc.Members |> Seq.find (fun it -> it.Name = field)
                memb.Type

        match find tb (tydesc.QualifiedName, verb.ToString()) with
        | Some looked_up -> looked_up
        | _ ->
        match verb with
        | ComposedVerb(l, r) ->
            (** composed *)
            match l, r with
            | _, BGet -> collect tydesc BGet
            | l, BSet -> collect tydesc BSet
            | SGet field, _ ->
                let memb_ty = get_member_type field
                collect memb_ty r
            | LGet, _ ->
                let elem_ty = get_elem_type()
                let { pos_arg_types = pos_arg_types } as info = collect elem_ty r
                { info with pos_arg_types = "int" :: pos_arg_types }
            | _ -> failwith "Only SGet/LGet requires method chaining composition."
            |> function
            | { verb_str_lst = verb_str_lst; pos_arg_types = pos_arg_types; ret_type = ret_type } ->
                let verb_str = chaining_verb_to_name l
                verb_str :: verb_str_lst, pos_arg_types, ret_type
        | _ ->
            (** in the final node of a method chain *)
            match verb with
            (** argnum 1 *)
            | SSet field ->
                let memb_ty = get_member_type field
                [ ty_to_string memb_ty ], null_type_string
            | BSet ->
                let arg_type = ty_to_string tydesc
                [ arg_type ], null_type_string
            | LGet ->
                let elem_ty = get_elem_type()
                [ "int" ], ty_to_string elem_ty
            | LRemoveAt -> [ "int" ], "bool"
            | LContains ->
                let elem_ty = get_elem_type()
                [ ty_to_string elem_ty ], "bool"
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
            | (** argnum 0 *)
              BGet -> [], ty_to_string tydesc
            | LCount -> [], "int"
            | SGet field ->
                let memb_ty = get_member_type field
                [], ty_to_string memb_ty
            (** BNew takes no subject argument. **)
            | BNew -> [], ty_to_string tydesc
            | info -> failwith <| sprintf "NotImplemented verb %A on %s" info tydesc.TypeName
            |> function
            | pos_arg_types, ret_type ->
                let verb_str = chaining_verb_to_name verb
                [ verb_str ], pos_arg_types, ret_type
        |> function
        | verb_str_lst, pos_arg_types, ret_type ->
            let result =
                { name_sig = recur_name tydesc
                  verb_str_lst = verb_str_lst
                  pos_arg_types = pos_arg_types
                  ret_type = ret_type }
            tb.[(tydesc.QualifiedName, verb.ToString())] <- result
            result
    in
    let { name_sig = name_sig; verb_str_lst = verb_str_lst; pos_arg_types = pos_arg_types; ret_type = ret_type } = collect tydesc verb
    let name_sig = sprintf "%s_%s" name_sig <| String.Join("_", verb_str_lst)
    let pos_arg_types = "void*" :: pos_arg_types

    let join (lst : string list) = String.Join(", ", lst)

    let parameters =
        [ for i in 1..(pos_arg_types.Length) -> sprintf "arg%d" i ]

    let typed_parameters =
        List.zip pos_arg_types parameters |> List.map (fun (parameter, ty_str) -> sprintf "%s %s" ty_str parameter)
    let types_string : string = join pos_arg_types
    let args_string : string = join parameters
    let typed_args_string : string = join typed_parameters
    let function_type_string = sprintf "%s (*)(%s)" ret_type types_string
    let decl = sprintf "static %s %s(%s);" ret_type name_sig types_string
    let generator addr =
        sprintf ("static %s %s(%s){\nreturn static_cast<%s>(0x%xll)(%s);\n}") ret_type name_sig typed_args_string function_type_string addr args_string
    in (decl, generator)

let code_gen (module_name) (tsl_specs : (TypeDescriptor * Verb list) list) =
    let tb = hashmap()
    let generate = single_method'code_gen tb
    let ty_recur_naming = ty_to_name true
    let (decls, defs) = 
        [ 
        for (ty, verb_lst) in tsl_specs do
            let ty_name = ty_recur_naming ty 
            
            for verb in verb_lst do
                let (decl, generator) = generate ty verb

                let native_fn =
                    CompileFunction { DeclaringType = ty; Verb = verb }

                let addr = native_fn.CallSite.ToInt64()

                yield (decl, generator addr)

            let initializer_decl = sprintf "static void* create_%s();" ty_name 
            let initializer_body = 
                sprintf "\n
static void* create_%s(){ 
    CellAccessor* accessor = new CellAccessor();
    auto errCode = %s_BNew(accessor);
    if(errCode) 
        throw errCode;
    return accessor;
}
                " ty_name ty_name
            yield (initializer_decl, initializer_body)

        let lock_cell_decl = "void* lock_cell(int64_t, int32_t);"
        let lock_cell_body = 
            sprintf "\n
void* UseCell(int64_t cellid, int32_t options)
{
    CellAccessor* accessor = new CellAccessor();
    accessor->cellId = cellid;
    auto errCode = LockCell(*accessor, options);
    if (errCode)
    throw errCode;
    return accessor;
}
            "
        yield (lock_cell_decl, lock_cell_body)

        ] |> List.unzip
        
    let (=>) a b = (a, b) 
    let swig_template = 
        "
%module {moduleName}
%include <stdint.i>
%{{
#include \"swig_accessor.h\"
#include \"CellAccessor.h\"
#define SWIG_FILE_WITH_INIT
{decl}
{source}
%}}
{decl}
        "
    in PString.format swig_template 
                       ["moduleName" => module_name
                        "source"     => (defs       |> PString.str'concatBy "\n" )
                        "decl"       => (decls      |> PString.str'concatBy "\n")
                        ]
    
    

