namespace SwigGen

open SwigGen

module TGEN = 
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open SwigGen.Command
    open SwigGen.Operator
            
        
        
    let Definition'Struct (struct': TypeDescriptor): string = 
        raise (NotImplementedException(""))
    
    let Definition'CellType(cellType': TypeDescriptor): string = 
        raise (NotImplementedException(""))
    
    let Definition'GenericList(list': TypeDescriptor): string = 
        raise (NotImplementedException(""))
    
    let Transpile(anyType : TypeDescriptor) : string = 
    
        match anyType.TypeCode with
            | STRUCT -> Definition'Struct anyType
            | LIST   -> Definition'GenericList anyType
            | CELL   -> Definition'CellType anyType
            | _      -> failwith "unknown typecode"
        
    
    let TypeInfer(anyType: TypeDescriptor): List<TypeDescriptor> = raise (NotImplementedException(""))
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
    
    
    let GenerateSrcCode(schema : IStorageSchema) : string = 
        let partial f x y = f (x, y)
        
        schema.CellDescriptors
                |> List.ofSeq
                |> List.map (Builder.Make >> TypeInfer)
                |> List.collect (List.map Transpile)
                |> partial System.String.Join "\n"

   