namespace Trinity.FFI.Metagen

module CodeGen = 
   open Trinity.FFI.Metagen.CommonForRender
   open Trinity.FFI.Metagen.Operator
   open GraphEngine.Jit.Verbs
   open GraphEngine.Jit.TypeSystem
   open GraphEngine.Jit.JitCompiler
   open System
   type Path = string // Path implementation might be changed
   type Code = string
   type FunctionAddr = uint32
   type FunctionIdentity = string

   type Name = string

    type FunctionId = string
        
    type TypeStr = string

    type ManglingChar = char

    type FunctionDecl = string

   //let ProjectGenerate (namespace': Path) 
   //                    (referencePaths: seq<Path>) 
   //                    (projectDirector: Path) 
   //                    (operationCodes: Code) : Map<FunctionIdentity, FunctionAddr> = 
        

   //     (**

   //     <Description>
   //      After loading some TSL extensions, vast operation generators are generated
   //         in memory storage in case of users' applications.
        
   //      Once a user call an operation(exported lazily as a function), 
   //         a. if this operation has not been generated yet, 
   //             - we will build the corresponding operation generator and give the generated one(an operation) to that user.
   //             - goto b
                
   //         b. if this operation has already been generated,
   //             - call the generated operation as the user expected.
   //     </Description>
        
   //     <Params>
   //         namespace'      : the namespace of generated c# proj.
   //         referencePath   : dll references of current TSL extension.
   //         projectDirector : the root dir of generated dll.
   //         operationCodes  : codes for data manipulations. => Target
   //     </Params>
        
   //     **) 
        //raise (NotImplementedException())

   let generateSwigFile
         (manglingCode : ManglingChar)
         (name'maker   : ManglingChar   -> TypeDescriptor -> Name) 
         (spec         : seq<TypeDescriptor *  seq<FunctionDescriptor*(FunctionDecl * (FunctionId -> Code))>>) = 

        let use_newobj = new System.Collections.Generic.List<string>()

        let rec reducer (seqs) (definitions: list<FunctionDecl>) (sources: list<Code>) = 

            match seqs with 
            | (subject: TypeDescriptor, fields: seq<FunctionDescriptor*(FunctionDecl * (FunctionId -> Code))>) :: tail ->

                let (defs, srcs) = fields |> Seq.map (fun (toCompile, (fnDecl, codeMaker)) -> 
                                                //let concrete_verb = 
                                                //    match toCompile.Verb with
                                                //    | SGet x -> ComposedVerb(SGet x, BGet)
                                                //    | SSet x -> ComposedVerb(SSet x, BSet)
                                                //    | x      -> x

                                                sprintf "0x%xll" ((CompileFunction toCompile).CallSite.ToInt64())
                                                |> codeMaker
                                                |> fun it -> (fnDecl, it))
                                       |> List.ofSeq
                                       |> List.unzip

                let extended'definitions = definitions|> List.append defs
                let extended'sources     = sources    |> List.append srcs
            
                let subject'name = name'maker manglingCode subject
            
                if subject.TypeCode = CELL
                then 
                   let func_name = sprintf "Use%c%s" manglingCode subject'name
                   let decl_head = sprintf "CellAccessor* %s(int64_t cellid, int32_t options)" func_name
                   use_newobj.Add("%newobject " + func_name + ";")

                   let src  = sprintf "
%s
{
    CellAccessor* accessor = new CellAccessor();
    accessor->cellId = cellid;
    auto errCode = LockCell(*accessor, options);
    if (errCode)
    throw errCode;
    return accessor;
}
                               " decl_head

                   reducer tail ((sprintf "%s;" decl_head)::extended'definitions) (src::extended'sources)
                else
                   reducer tail extended'definitions extended'sources
            | _     ->
                List.rev definitions, List.rev sources
        
        let (decls, srcs) = reducer (List.ofSeq spec) [] []
        
        fun (moduleName: Name) -> 
            "

%module {moduleName}
%include <stdint.i>
%{{
#include \"swig_accessor.h\"
#include \"CellAccessor.h\"
#define SWIG_FILE_WITH_INIT
{source}
%}}
{decl}
{use_newobj}
            " 
            |> fun template -> 
                PString.format template
                               [
                                "moduleName" ->> moduleName
                                "source"     ->> (srcs  |> PString.str'concatBy "\n" )
                                "decl"       ->> (decls |> PString.str'concatBy "\n")
                                "use_newobj" ->> (use_newobj |> PString.str'concatBy "\n")
                               ]
    
   
    
    
    
   

  

