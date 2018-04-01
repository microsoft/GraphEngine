namespace SwigGen

open SwigGen

module TGEN = 
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    
    let Transpile(anyType : TypeDescriptor) : string = raise (NotImplementedException(""))
    
    let TypeInfer(anyType: TypeDescriptor): List<TypeDescriptor> = raise (NotImplementedException(""))
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
    
    
    let GenerateSrcCode(schema : IStorageSchema) : string = 
        let partial f x y = f (x, y)
        
        schema.CellDescriptors
                |> List.ofSeq
                |> List.map Builder.Make
                |> List.map TypeInfer
                |> List.collect (function it -> it)
                |> List.map Transpile
                |> partial System.String.Join "\n"

   