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

        <Description>
         After loading some TSL extensions, vast operation generators are generated
            in memory storage in case of users' applications.
        
         Once a user call an operation(exported lazily as a function), 
            a. if this operation has not been generated yet, 
                - we will build the corresponding operation generator and give the generated one(an operation) to that user.
                - goto b
                
            b. if this operation has already been generated,
                - call the generated operation as the user expected.
        </Description>
        
        <Params>
            namespace'      : the namespace of generated c# proj.
            referencePath   : dll references of current TSL extension.
            projectDirector : the root dir of generated dll.
            operationCodes  : codes for data manipulations. => Target
        </Params>
        
        **) 



        raise (NotImplementedException())
    
   
    
    
    
   

  

