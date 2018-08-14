module Trinity.FFI.MetaGen.Factory

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
    | STRING -> "wchar_t*"
    | U8STRING -> "char*"
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

    let typeid = match tydesc with {TypeCode = CELL typeid} -> typeid | _ -> failwith "Only cell could be root object." 

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
                { info with pos_arg_types = "int64_t" :: pos_arg_types }
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
                [ "int32_t" ], ty_to_string elem_ty
            | LRemoveAt -> [ "int32_t" ], "bool"
            | LContains ->
                let elem_ty = get_elem_type()
                [ ty_to_string elem_ty ], "bool"
            | LAppend ->
                let elem_ty = get_elem_type()
                [ ty_to_string elem_ty ], null_type_string
            | LSet ->
                let elem_ty = get_elem_type()
                [ "int32_t"
                  ty_to_string elem_ty ], null_type_string
            | LInsertAt ->
                let elem_ty = get_elem_type()
                [ "int32_t"
                  ty_to_string elem_ty ], "bool"
            | (** argnum 0 *)
              BGet -> [], ty_to_string tydesc
            | LCount -> [], "int32_t"
            | SGet field ->
                let memb_ty = get_member_type field
                [], ty_to_string memb_ty // if primitive, for swig will do a more copy so without BGet is okay.
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
    let join (lst : string list) = String.Join(", ", lst)
    let parameters =
        [ for i in 1..(pos_arg_types.Length) -> sprintf "arg%d" i ]
    
    let (=>) a b = (a, b)
    if parameters.Length = 0 then 
        let private_fn_type = sprintf "%s (*)(void*)" ret_type
        let decl = sprintf "static %s %s(int64_t);" ret_type name_sig 
        
        let generator (addr: int64) = 
            let template = 
                sprintf  "
static {ret ty} (*_{fn name})(void*) = reinterpret_cast<{p fn type}>(0x%xll);
static {ret ty} {fn name}(int64_t cellId)
{{
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = {typeid};
    return _{fn name}(&acc);
}}                      " addr
             in PString.format 
                    template  
                    [
                     "ret ty"    => ret_type
                     "fn name"   => name_sig
                     "p fn type" => private_fn_type
                     "typeid"    => (sprintf "%u" typeid)
                    ]

        in (decl, generator)
    else
        let rev_typed_parameters =
            List.zip pos_arg_types parameters 
            |> List.rev 
            |> List.map (fun (ty_str, parameter) -> sprintf "%s %s" ty_str parameter)

        let args_string : string = join parameters

        // string formal of reversed typed arguemnts
        let rev_targs_string : string = join rev_typed_parameters
        
        let private_fn_type = sprintf "%s (*)(void*, %s)" ret_type <| join pos_arg_types
        let decl = sprintf "static %s %s(%s, int64_t);" ret_type name_sig <| join (List.rev pos_arg_types)
        let generator (addr: int64) = 
            let template = 
                sprintf  "
static {ret ty} (*_{fn name})(void*, {pos arg types}) = reinterpret_cast<{p fn type}>(0x%xll);
static {ret ty} {fn name}({rev targs string}, int64_t cellId)
{{
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = {typeid};
    return _{fn name}(&acc, {args string});
}}
                         " addr 
            
            PString.format 
                    template 
                     [
                        "ret ty"  => ret_type
                        "fn name" => name_sig
                        "pos arg types" => (join pos_arg_types)
                        "p fn type"     => private_fn_type
                        "rev targs string" => rev_targs_string
                        "args string" => args_string
                        "typeid" => (sprintf "%u" typeid)
                     ]
        in (decl, generator)

let code_gen (module_name) (tsl_specs : (TypeDescriptor * Verb list) list) =
    let tb = hashmap()  (** for caching *)

    let generate = single_method'code_gen tb
    let ty_recur_naming = ty_to_name true
   

    let (decls, defs) =
        [
        for (ty, verb_lst) in tsl_specs do
            // for specific types
            let ty_name = ty_recur_naming ty

            match ty.TypeCode with
            | CELL typeid ->
            for verb in verb_lst do
                let (decl, generator) = generate ty verb
                let native_fn =
                    CompileFunction { DeclaringType = ty; Verb = verb }

                let addr = native_fn.CallSite.ToInt64()

                yield (decl, generator addr)

            let lock_cell_decl = sprintf "static void use_%s(int64_t, int32_t);" ty_name
            let lock_cell_body =
                    sprintf "\n
static std::function<int32_t(void*)> _use_%s = _%s_BNew;
static void use_%s(int64_t cellId, int32_t options)
{
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = %u;

    auto errCode = LockCell(acc, options, _use_%s);
    if (TrinityErrorCode_FAILED(errCode))
        throw errCode;    
}                       " ty_name ty_name ty_name typeid ty_name

            yield (lock_cell_decl, lock_cell_body)

            let load_cell_decl = sprintf "static void load_%s(int64_t, char** trinity_loaded_content_buff, int* trinity_loaded_content_len);" ty_name
            let load_cell_body =
                    sprintf "\n
static void load_%s(int64_t cellId, char** trinity_loaded_content_buff, int *trinity_loaded_content_len)
{
    CellAccessor acc;

    acc.cellId = cellId;
    acc.type = %u;

    auto errCode = LoadCell(acc);
    if (TrinityErrorCode_FAILED(errCode))
        throw errCode;
   
    *trinity_loaded_content_buff = reinterpret_cast<char*>(acc.cellPtr);
    *trinity_loaded_content_len = acc.size;
}                       " ty_name typeid
            yield (load_cell_decl, load_cell_body)

            let valued_initializer_decl = sprintf "static void use_%s_with_data(int64_t, char*);" ty_name 
            let valued_initializer_body = 
                    sprintf  "
static void use_%s_with_data(int64_t cellId, char* content)
{
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = %u;
      
    std::function<int32_t(void*)> caller = [&content](void* acc_ptr){
        auto errCode = _%s_BNew(acc_ptr);

        if (TrinityErrorCode_FAILED(errCode))
            return errCode;

        _%s_BSet(acc_ptr, reinterpret_cast<void*>(content));

        return 0;
    };

    auto errCode = LockCell(acc, _CreateNewOnCellNotFound, caller);

    if (TrinityErrorCode_FAILED(errCode))
        throw errCode;
}              
                    " ty_name typeid ty_name ty_name
            yield (valued_initializer_decl, valued_initializer_body)

            let unlock_decl = sprintf "static void unlock_%s(int64_t);" ty_name
            let unlock_body =
                     sprintf "
static void unlock_%s(int64_t cellId)
{ 
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = %u;
    UnlockCell(acc); 
}
                        " ty_name typeid
            yield (unlock_decl, unlock_body)
            let save_cell_decl = sprintf "static void save_%s(int64_t, char*);" ty_name;
            let save_cell_body = 
                    sprintf "
static void save_%s(int64_t cellId, char* content)
{
    CellAccessor acc;
    acc.cellId = cellId;
    acc.type = %u;
    acc.cellPtr = reinterpret_cast<int64_t>(content);
    auto errCode = SaveCell(acc);
    if(TrinityErrorCode_FAILED(errCode))
        throw errCode;
}
                        " ty_name typeid
            yield (save_cell_decl, save_cell_body)    

            
            
                
            | _ -> ()
        
        // not for specific types
 
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
%include <std_wstring.i>
%include <cstring.i>
%cstring_output_allocate_size(char **trinity_loaded_content_buff, int *trinity_loaded_content_len, TRINITY_JUSTPASS);
%begin %{{
#define TRINITY_JUSTPASS 

#include \"swig_accessor.h\"
#include \"CellAccessor.h\"
#include \"stdio.h\"
#define TrinityErrorCode_FAILED(x) (x != TrinityErrorCode::E_SUCCESS)
const int32_t _CreateNewOnCellNotFound = 2;

#define SWIG_FILE_WITH_INIT
#define SWIG_PYTHON_STRICT_BYTE_CHAR
{decl}
{source}
%}}
{decl}
        "
    in PString.format swig_template
                       ["moduleName" => module_name
                        "source"     => (defs           |> PString.str'concatBy "\n" )
                        "decl"       => (decls          |> PString.str'concatBy "\n")
                        ]

