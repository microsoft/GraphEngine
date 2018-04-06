namespace SwigGen

open SwigGen

module TGEN = 
    open GraphEngine.Jit.TypeSystem
    open System
    open Trinity.Storage
    open SwigGen.Command
    open SwigGen.Operator
    let manglingChar = "蛤"
            
    let mangling (name: string) = name.Replace(manglingChar, manglingChar + manglingChar)

    let MakeName (typeDesc :TypeDescriptor): string = 
        raise (NotImplementedException())

    let makeMethod ()

        
    let Definition'Struct (struct': TypeDescriptor): string = 
        PString.format "
void {TypeName}{manglingChar}{FieldName}{manglingChar}Get_{Attrs}(void* struct_ptr, void* &data){{
        
        
}}
    

"            (
                Map [ 
                    
                    "manglingChar" ->> manglingChar
                    "1" ->> "2"
                ])
    
    let Definition'CellType(cellType': TypeDescriptor): string = 
        raise (NotImplementedException(""))
    
    let Definition'GenericList(list': TypeDescriptor): string = 
        raise (NotImplementedException(""))
    
    let Transpile(anyType : TypeDescriptor) : string = 
    
        match anyType.TypeCode with
            | STRUCT -> Definition'Struct anyType
            | LIST   -> Definition'GenericList anyType
            | CELL   -> Definition'CellType anyType
            | _      -> failwith "unexpected type"
        
    
    let TypeInfer(anyType: TypeDescriptor): List<TypeDescriptor> = 
    (** inference out the descriptors of struct types and generic list types in a cell descriptor.**)
        match anyType with 
            | {TypeCode=LIST; ElementType = elemType} 
                 -> raise (NotImplementedException())
            | {TypeCode=CELL; Members=members}
            | {TypeCode=STRUCT; Members=members} 
                 -> 
                    members
                    |> List.ofSeq
                    |> List.map (fun field -> field.Type)
                    |> fun tail -> anyType::tail
            | _  -> failwith "unexpected type"
    
        
    
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
                |> List.ofSeq
                |> List.map (Builder.Make >> TypeInfer)
                |> List.collect (List.filter (fun t -> generatedTypeNames.Contains t.TypeName))
                |> List.map Transpile
                |> partial System.String.Join "\n"

   