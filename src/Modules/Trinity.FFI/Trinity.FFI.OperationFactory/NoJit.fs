namespace Trinity.FFI.OperationFactory

module NoJit = 
   open System
   type Path = string // Path implementation might be changed
   type Code = string
   type FunctionAddr = uint32
   type FunctionIdentity = string

   
   let ProjectGenerate (namespace': Path) 
                       (referencePaths: seq<Path>) 
                       (projectDirector: Path) 
                       (operationCodes: Code) : Map<FunctionIdentity, FunctionAddr> = 
        (**
        namespace'      : the namespace of generated c# proj.
        referencePath   : dll references of TSL extensions.
        projectDirector : the root dir of generated dll.
        operationCodes  : codes for data manipulations. => Target

        **) 
        raise (NotImplementedException())
    
   
    
    
    
   

  

