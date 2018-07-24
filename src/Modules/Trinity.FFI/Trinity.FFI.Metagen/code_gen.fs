
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
    | { TypeCode = code} -> code.ToString()

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
    | U8STRING -> "wchar_t*"
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
                [], "void*" // SGet always get 
            (** BNew takes no subject argument. **)
            | BNew -> [], "int32_t"
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

    // reversed
    let rev_typed_parameters =
        List.zip pos_arg_types parameters |> List.rev |> List.map (fun (ty_str, parameter) -> sprintf "%s %s" ty_str parameter)
    let args_string : string = join parameters
    let typed_args_string : string = join rev_typed_parameters
    let private_fn_type = sprintf "%s (*)(%s)" ret_type <| join pos_arg_types
    let decl = sprintf "static %s %s(%s);" ret_type name_sig <| join (List.rev pos_arg_types)
    let generator addr =
        sprintf ("static %s %s(%s){\nreturn reinterpret_cast<%s>(0x%xll)(%s);\n}") ret_type name_sig typed_args_string private_fn_type addr args_string
    in (decl, generator)

let code_gen (json_cons_fn_ptr: int64) (module_name) (tsl_specs : (TypeDescriptor * Verb list) list) =
    let tb = hashmap()  (** for caching *)

    let generate = single_method'code_gen tb
    let ty_recur_naming = ty_to_name true
    let (decls, defs) =
        [
        let json_cons_method_body = 
            sprintf "
static void (*json_cons)(char*, char*, int64_t&, int64_t&) = reinterpret_cast<void(*)(char*, char*, int64_t&, int64_t&)>(0x%ull);
                    " json_cons_fn_ptr
        yield ("", json_cons_method_body)
        for (ty, verb_lst) in tsl_specs do
            
            // for specific types
            let ty_name = ty_recur_naming ty

            for verb in verb_lst do
                let (decl, generator) = generate ty verb
                let native_fn =
                    CompileFunction { DeclaringType = ty; Verb = verb }

                let addr = native_fn.CallSite.ToInt64()

                yield (decl, generator addr)
            
            match ty.TypeCode with
            | CELL typeid ->
                // for cells
                let lock_cell_from_existed_decl = sprintf "static void reuse_%s(void*, int32_t);" ty_name
                let lock_cell_from_existed_body =
                    sprintf "\n
static void reuse_%s(void* subject, int32_t options)
{
    auto accessor = static_cast<CellAccessor*>(subject);
    accessor -> Release();
    auto errCode = LockCell(*accessor, options, %s_BNew);
    if (errCode)
        throw errCode;
}                       " ty_name ty_name
                yield (lock_cell_from_existed_decl, lock_cell_from_existed_body)

                let lock_cell_decl = sprintf "static void* use_%s(int64_t, int32_t);" ty_name
                let lock_cell_body =
                    sprintf "\n
static void* use_%s(int64_t cellid, int32_t options)
{
    CellAccessor* accessor = new CellAccessor();
    accessor -> cellId = cellid;
    accessor -> type = %u;
    auto errCode = LockCell(*accessor, options, %s_BNew);
    if (errCode)
        throw errCode;
    return accessor;
}                       " ty_name typeid ty_name
                yield (lock_cell_decl, lock_cell_body)

                let load_cell_decl = sprintf "static void* load_%s(int64_t cellid);" ty_name
                let load_cell_body =
                    sprintf "\n
static void* load_%s(int64_t cellid)
{
    CellAccessor* accessor = new CellAccessor();
    accessor -> cellId = cellid;
    accessor -> type = %u;
    auto errCode = LoadCell(*accessor);
    if (errCode)
        throw errCode;
    return accessor;
}                       " ty_name typeid
                yield (load_cell_decl, load_cell_body)
            | _ -> ()

            let default_initializer_decl = sprintf "static void* create_%s();" ty_name
            let default_initializer_body =
                sprintf "\n
static void* create_%s()
{
    CellAccessor* accessor = new CellAccessor();
    auto errCode = %s_BNew(accessor);
    if(errCode)
        throw errCode;
    return accessor;
}                " ty_name ty_name
            yield (default_initializer_decl, default_initializer_body)

            let valued_initializer_decl = sprintf "static void* create_%s_with_data(char*);" ty_name 
            let valued_initializer_body = 
                sprintf  "
 static void* create_%s_with_data(char* content)
 {
    CellAccessor* accessor = static_cast<CellAccessor*>(create_%s());
    json_cons(\"%s\", content, accessor -> cellId, accessor -> cellPtr);
    return accessor;
 }              
                " ty_name ty_name ty.TypeName
            yield (valued_initializer_decl, valued_initializer_body)
        
        // not for specific types
        
        let basic_ref_get_decl = "static void* Unbox(void*);"
        let basic_ref_get_body = "\n
static void* Unbox(void* object)
{
    return cast_object(object);
}
          "
        yield (basic_ref_get_decl, basic_ref_get_body)

        let unlock_decl = "static void unlock(void*);"
        let unlock_body =
             "
static void unlock(void* subject)
{ 
    auto accessor = static_cast<CellAccessor*>(subject);
    UnlockCell(*accessor); 
}
                "
        yield (unlock_decl, unlock_body)

        let save_cell_decl = "static void save_cell(void*);";
        let save_cell_body = "
static void save_cell(void* subject)
{
auto accessor = static_cast<CellAccessor*>(subject);
auto errCode = SaveCell(*accessor);
if(errCode)
    throw errCode;
}
            "
        yield (save_cell_decl, save_cell_body)
        // TODO:
        //   1. load and save cell. load cell might not require cellAcc.type(done)
        //   2. link json deserialization method to init data
        //   3. python method binding of above unfinished items(done).

        ] |> List.unzip

    let (=>) a b = (a, b)
    let swig_template =
        "
%module {moduleName}
%include <stdint.i>
%{{
#include \"swig_accessor.h\"
#include \"CellAccessor.h\"
#include \"stdio.h\"
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

