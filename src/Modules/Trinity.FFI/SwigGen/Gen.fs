namespace SwigGen

open SwigGen

module TGEN = 
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open SwigGen.Command
    open SwigGen.Operator
    open GraphEngine.Jit.TypeSystem


    let manglingChar = '_'
            
    let mangling (name: string) = name.Replace(manglingChar, manglingChar + manglingChar)

    let MakeName (typeDesc :TypeDescriptor): string = raise (NotImplementedException())
    
    let make'operations (typeDesc: TypeDescriptor) : seq<string> = raise (NotImplementedException())
    
    let make'attribute'method  (memberDesc: MemberDescriptor) : string = raise (NotImplementedException()) 
    
    let make'list'method (typeDesc: TypeDescriptor) : string = raise (NotImplementedException())
        
    let define'structOrCell = Seq.collect make'attribute'method >> PString.str'concatBy "\n"

    let define'genericList = Seq.head >> make'list'method

    
    let Transpile(anyType : TypeDescriptor) : string = 
        match anyType.TypeCode with
            | STRUCT
            | CELL -> define'structOrCell anyType.Members
            | LIST   -> define'genericList anyType.ElementType
            | _      -> failwith "unexpected type"
        
    
    let rec TypeInfer(anyType: TypeDescriptor): seq<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
            | {TypeCode=LIST; ElementType = elemType} 
                 -> anyType >>> TypeInfer (Seq.head elemType)
            | {TypeCode=CELL; Members=members}
            | {TypeCode=STRUCT; Members=members} 
                 -> 
                    members
                    |> Seq.map (fun field -> field.Type)
                    |> fun tail -> anyType >>> tail
            | _  -> seq []
    
        
    
    let GenerateSrcCode(schema : IStorageSchema, 
                        generatedTypeNames: System.Collections.Generic.HashSet<string>) : string = 
        let partial f x y = f (x, y)

        let filterAndUpdate (typeDesc: TypeDescriptor) = 
            if typeDesc.TypeName |> generatedTypeNames.Contains
            then 
                false
            else
                generatedTypeNames.Add typeDesc.TypeName |> ignore
                true

        schema.CellDescriptors
                |> Seq.map (Builder.Make >> TypeInfer)
                |> Seq.collect (Seq.filter (fun t -> generatedTypeNames.Contains t.TypeName))
                |> Seq.map Transpile
                |> PString.str'concatBy "\n"

   