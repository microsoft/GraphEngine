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

        let rec reducer (seqs) (definitions: list<FunctionDecl>) (sources: list<Code>) = 

            match seqs with 
            | (subject: TypeDescriptor, fields: seq<FunctionDescriptor*(FunctionDecl * (FunctionId -> Code))>) :: tail ->

                let (defs, srcs) = fields |> Seq.map (fun (toCompile, (fnDecl, codeMaker)) -> 
                                                (** Callsite.ToInt? **)
                                                CompileFunction(toCompile).CallSite.ToInt64().ToString()
                                                |> codeMaker
                                                |> fun it -> (fnDecl, it))
                                       |> List.ofSeq
                                       |> List.unzip

                let extended'definitions = definitions|> List.append defs
                let extended'sources     = sources    |> List.append srcs
            
                let subject'name = name'maker manglingCode subject
            
                if subject.TypeCode = CELL
                then 
                   let decl = sprintf "CellAccessor Use%c%s(cellid_t cellid, CellAccessOptions options);" manglingCode subject'name

                   let src  = sprintf "
%s
{
    CellAccessor accessor;
    auto errCode = LockCell(accessor, options);
    if (errCode)
    throw errCode;
    return accessor;
}
                               " decl

                   reducer tail (decl::extended'definitions) (src::extended'sources)
                else
                   reducer tail extended'definitions extended'sources
            | _     ->
                List.rev definitions, List.rev sources
        
        let (decls, srcs) = reducer (List.ofSeq spec) [] []
        
        fun (moduleName: Name) -> 
            "
%module {moduleName}
%{{
#include \"GraphEngine.Jit.Native.h\"
#include \"CellAccessor.h\"
#define SWIG_FILE_WITH_INIT
{source}
%}}
{delc}
            " 
            |> fun template -> 
                PString.format template
                               [
                                "moduleName" ->> moduleName
                                "source"     ->> (srcs  |> PString.str'concatBy "\n" )
                                "delc"       ->> (decls |> PString.str'concatBy "\n")
                               ]
    
   
    
    
    
   

  

