using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal interface FixedFieldType : FieldType
    {
        int Length { get; }
    }

    internal class FixedStructFieldType : StructFieldType, FixedFieldType
    {
        public FixedStructFieldType(StructDescriptor format)
        {
            if (!format.IsFixed())
                CompilerError.Throw(CompilerErrorType.FixedFormatExpected);
            this.descriptor = format;
        }
        public int Length
        {
            get
            {
                int ret = 0;
                foreach (Field f in descriptor.Fields)
                {
                    ret += (f.Type as FixedFieldType).Length;
                }
                return ret;
            }
        }
        public override string GeneratePushPointerCode()
        {
            return "targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
            ";
        }
    }

    internal class ArrayType : FixedFieldType
    {
        public int Length
        {
            get
            {
                int ret = ElementType.Length;
                foreach (var l in lengths)
                    ret *= l;
                return ret;
            }
        }
        public int[] lengths;
        public FixedFieldType ElementType;

        public ArrayType(FixedFieldType elementType, List<int> dimLength)
        {
            this.ElementType = elementType;
            this.lengths = dimLength.ToArray();
        }

        public string Name
        {
            get
            {
                string ret = ElementType.Name + "Array";
                foreach (int length in lengths)
                    ret += "_" + length.ToString(CultureInfo.InvariantCulture);
                return ret;
            }
        }

        public string CSharpName
        {
            get
            {
                string ret = ElementType.CSharpName + "[";
                if (lengths.Length > 0)
                    foreach (int length in lengths)
                        //ret += length.ToString(CultureInfo.InvariantCulture) + ",";
                        ret += ",";
                ret = ret.Remove(ret.Length - 1);
                ret += "]";
                return ret;
            }
        }


        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            string total_length_string = Length.ToString(CultureInfo.InvariantCulture);
            if (OnlyPushPointer)
            {
                return "targetPtr += " + total_length_string + @";
";
            }

            string pointer_string = "storedPtr_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string iterator_name = "iterator_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string ret = "";
            ret += @"
                if(" + VarName + @"!= null){
                    if(" + VarName + @".Rank != " + lengths.Length.ToString(CultureInfo.InvariantCulture) + @") throw new IndexOutOfRangeException(""The assigned array'storage Rank mismatch."");
                    if(";
            for (int i = 0; i < lengths.Length; i++)
                ret += VarName + @".GetLength(" + i.ToString(CultureInfo.InvariantCulture) + @") != " + lengths[i].ToString(CultureInfo.InvariantCulture) + " || ";
            ret = ret.TrimEnd(" || ".ToCharArray()) + @") throw new IndexOutOfRangeException(""The assigned array'storage dimension mismatch."");
                ";

            if (TSLCompiler.IsValueType(ElementType))
            {
                ret += @"
                    fixed(" + ElementType.Name + "* " + pointer_string + " = " + VarName + @")
                    {
                        Memory.memcpy(targetPtr," + pointer_string + ",(ulong)(" + total_length_string + @"));
                    }
                    ";
                ret += "targetPtr += " + total_length_string + @";
";
            }
            else//We can't get the pointer to the array, so we have to traverse through the array and use elementType'storage assign code.
            {
                ret += @"
                    foreach( var " + iterator_name + @" in " + VarName + @")
                    {
                        " + ElementType.GenerateAssignCodeForConstructor(iterator_name, currentLevel + 1, OnlyPushPointer) + @"
                    }";
            }
            ret += @"
                }
                else
                {
                    ";
            if (TSLCompiler.IsValueType(ElementType))
                ret += @"Memory.memset(targetPtr,0,(ulong)(" + total_length_string + @"));
                    ";
            ret += "targetPtr += " + total_length_string + @";
                }
                ";
            return ret;
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
        }
    }
    enum TokenId
    {
        MapSymbol,
        Comma,
        Semicolon,
        BlockBegin,
        BlockEnd,
        TypeListBegin,
        TypeListEnd,
        LSquare,
        RSquare,
        Equal,
        Dot,
        Colon,
        Using,
        Include,

        ListType,
        StringType,
        DateTimeType,
        GuidType,

        FixedModifier,
        ElasticModifier,
        ExternModifier,
        OptionalModifier,

        StructDefinition,
        CellDefinition,
        ServerDefinition,
        ProxyDefinition,
        ProtocolDefinition,
        IndexDefinition,

        SyncRPC,
        AsyncRPC,
        Type,
        Request,
        Response,
        VOID,
        RAW,

        BitType,
        ByteType,
        SByteType,
        CharType,
        BoolType,
        ShortType,
        UShortType,
        IntType,
        UIntType,
        LongType,
        ULongType,
        FloatType,
        DoubleType,
        DecimalType,

        NameString,
        Integer,

        DoubleSlash,
        SlashStar,
        StarSlash,
        LF,
        Sharp,

        Layout,
        Seq_Layout,
        Auto_Layout,

        Enum,

        // Struct Attributes
        EntityList,
        RelationalTable,
        PartitionBy,
        SpecFile,
        Entities,
        ConnectionString,
        SqlServer,

        // Cell Field Attributes
        PrimaryKey,
        ReferencedCell,
        Column,
        Association,
        Invisible,
        TrinitySettings,
        RunningMode,
        Embedded,
        Distributed,
        IndexConnString,
        TQL,
        OFF,
        ON,
        RDF,
        Freebase,
        TSLProfile,
        TrinityMM,
        ExtensionSuffixChar,
        Namespace,
    }
    internal class AtomType : FixedFieldType
    {
        internal TokenId AtomSymbol;
        string TypeName;
        int TypeLength;
        internal static Dictionary<TokenId, string> NameTable = new Dictionary<TokenId, string>();
        internal static Dictionary<TokenId, int> LengthTable  = new Dictionary<TokenId, int>();

        static AtomType()
        {
            NameTable[TokenId.BitType]       = "bit";
            NameTable[TokenId.ByteType]      = "byte";
            NameTable[TokenId.SByteType]     = "sbyte";
            NameTable[TokenId.BoolType]      = "bool";
            NameTable[TokenId.CharType]      = "char";
            NameTable[TokenId.ShortType]     = "short";
            NameTable[TokenId.UShortType]    = "ushort";
            NameTable[TokenId.IntType]       = "int";
            NameTable[TokenId.UIntType]      = "uint";
            NameTable[TokenId.LongType]      = "long";
            NameTable[TokenId.ULongType]     = "ulong";
            NameTable[TokenId.FloatType]     = "float";
            NameTable[TokenId.DoubleType]    = "double";
            NameTable[TokenId.DecimalType]   = "decimal";

            LengthTable[TokenId.BitType]     = 1;//TODO
            LengthTable[TokenId.ByteType]    = 1;
            LengthTable[TokenId.SByteType]   = 1;
            LengthTable[TokenId.BoolType]    = 1;
            LengthTable[TokenId.CharType]    = 2;
            LengthTable[TokenId.ShortType]   = 2;
            LengthTable[TokenId.UShortType]  = 2;
            LengthTable[TokenId.IntType]     = 4;
            LengthTable[TokenId.UIntType]    = 4;
            LengthTable[TokenId.LongType]    = 8;
            LengthTable[TokenId.ULongType]   = 8;
            LengthTable[TokenId.FloatType]   = 4;
            LengthTable[TokenId.DoubleType]  = 8;
            LengthTable[TokenId.DecimalType] = 16;
        }

        public AtomType(TokenId TypeSymbol)
        {
            TypeName = NameTable[TypeSymbol];
            TypeLength = LengthTable[TypeSymbol];
            AtomSymbol = TypeSymbol;
        }

        public AtomType(string typeName)
        {
            foreach (var kvp in NameTable)
            {
                if (kvp.Value == typeName)
                {
                    TypeName   = NameTable[kvp.Key];
                    TypeLength = LengthTable[kvp.Key];
                    AtomSymbol = kvp.Key;
                    return;
                }
            }
            CompilerError.Throw("Unrecognized atomType in AtomType constructor: " + typeName);
        }

        public int Length
        {
            get { return TypeLength; }
        }

        public string Name
        {
            get
            {
                return TypeName;
            }
        }

        public string CSharpName
        {
            get
            {
                return TypeName;
            }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            if (OnlyPushPointer)
                return "targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
            return "*(" + TypeName + "*)targetPtr = " + VarName + ";" + @"
            targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
        }
    }

    internal class GuidType : FixedFieldType
    {
        public int Length
        {
            get { return 16; }
        }

        public string Name
        {
            get { return "GuidAccessor"; }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            if (OnlyPushPointer)
                return "targetPtr += 16;";
            return @"
                if(" + VarName + @"!= null)
                {
                    byte[] tmpGuid = " + VarName + @".ToByteArray();
                    fixed(byte* tmpGuidPtr = tmpGuid)
                    {
                    Memory.Copy(tmpGuidPtr, targetPtr, 16);                    
                    }
                    targetPtr += 16;
                }
                else
                {
                    Memory.memset(targetPtr, 0, 16);                    
                    targetPtr+=16;
                }
                
";
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr += 16;";
        }

        public string CSharpName
        {
            get { return "Guid"; }
        }
    }

    internal class EnumType : FixedFieldType
    {
        internal EnumDescriptor descriptor;
        public EnumType(EnumDescriptor desc)
        {
            this.descriptor = desc;
        }

        public int Length
        {
            get { return sizeof(byte); }
        }

        public string Name
        {
            get { return descriptor.Name; }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            if (OnlyPushPointer)
                return "targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
            return "*(" + Name + "*)targetPtr = " + VarName + ";" + @"
            targetPtr += " + Length.ToString(CultureInfo.InvariantCulture) + @";
";
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr++;";
        }

        public string CSharpName
        {
            get { return Name; }
        }
    }

    internal class DateTimeType : FixedFieldType
    {
        public int Length
        {
            get { return 8; }
        }

        public string Name
        {
            get { return "DateTimeAccessor"; }
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            if (OnlyPushPointer)
                return "targetPtr += 8;";
            return @"
                if(" + VarName + @"!= null)
                {
                    *(long*)targetPtr = " + VarName + @".ToBinary();
                    targetPtr += 8;
                }
                else
                {
                 *(long*)targetPtr = 0;
                    targetPtr+=8;
                }
";
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr += 8;";
        }

        public string CSharpName
        {
            get { return "DateTime"; }
        }
    }
}
