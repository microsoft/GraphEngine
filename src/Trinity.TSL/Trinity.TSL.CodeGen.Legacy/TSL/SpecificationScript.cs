using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Trinity.TSL
{
    public class SpecificationScript
    {
        internal static SpecificationScript CurrentScript = null;

        private string name_prefix = "";
        internal string NamingPrefix
        {
            get
            {
                return name_prefix;
            }
            set
            {
                name_prefix = value;
            }
        }

        internal List<StructDescriptor>          StructDescriptors   = new List<StructDescriptor>();
        internal List<StructDescriptor>          CellDescriptors     = new List<StructDescriptor>();

        internal List<ProtocolDescriptor>        ProtocolDescriptors = new List<ProtocolDescriptor>();
        internal List<ProtocolGroupDescriptor>   ServerDescriptors   = new List<ProtocolGroupDescriptor>();
        internal List<ProtocolGroupDescriptor>   ProxyDescriptors    = new List<ProtocolGroupDescriptor>();
        internal List<ProtocolGroupDescriptor>   ModuleDescriptors   = new List<ProtocolGroupDescriptor>();

        internal List<EnumDescriptor>            EnumDescriptors     = new List<EnumDescriptor>();


        internal List<TrinitySettingsDescriptor> TrinitySettings     = new List<TrinitySettingsDescriptor>();


        //TODO
        internal List<string> ReferencedAssemblyFileNames = new List<string>();
        //internal Dictionary<string, string> Macros = new Dictionary<string, string>();
        //internal List<EnumDescriptor> EnumDescriptors = new List<EnumDescriptor>();

        //For Graph Element Annotator to location fields
        internal string RootNamespace;
        internal Guid GUID;
        internal Dictionary<string, string> TrinitySettingKVTable = new Dictionary<string, string>();

        // Top level node, equivalent to NTSL
        public SpecificationScript(bool needReorderTSL = false, bool annotateGraphElements = false)
        {
            GUID = Guid.Empty;
            RootNamespace = "Trinity";

            //Sort the structs and cells so that they are in the same order as the user specified
            StructDescriptors.Sort((a, b) => a.top_level_index.CompareTo(b.top_level_index));
            CellDescriptors.Sort((a, b) => a.top_level_index.CompareTo(b.top_level_index));

            //case TokenId.Using:
            //    HandleReferences(l);
            //    break;

            //case TokenId.Namespace:
            //    HandleNamespace(l);
            //    break;

            //case TokenId.Sharp:
            //    HandleMacro(l);
            //    break;



            //if (needReorderTSL)
            //{
            //    l.WriteBackReorderedTSL();
            //}
            //else
            //{
            //    ReferencedAssemblyFileNames = new HashSet<string>(ReferencedAssemblyFileNames).ToList();
            //}

            foreach (TrinitySettingsDescriptor settings_desc in TrinitySettings)
            {
                string guid;
                if (settings_desc.KVTable.TryGetValue("GUID", out guid))
                {
                    if (!Guid.TryParse(guid, out this.GUID))
                    {
                        this.GUID = Guid.Empty;
                    }
                }

                foreach (var k in settings_desc.KVTable.Keys)
                {
                    TrinitySettingKVTable[k] = settings_desc.KVTable[k];
                }
            }

            //Console.WriteLine(RootNamespace);
        }

        //private void HandleNamespace(Lexer l)
        //{
        //    RootNamespace = l.ReadUntil(TokenId.Semicolon).Trim();
        //    if (l.ReadToken() != TokenId.Semicolon)
        //        CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
        //}
        //private void HandleReferences(Lexer l)
        //{
        //    string filename = l.ReadUntil(TokenId.Semicolon);
        //    ReferencedAssemblyFileNames.Add(filename);
        //    l.ReadToken();
        //}

        internal StructDescriptor GetStructDescByName(string name)
        {
            foreach (var desc in StructDescriptors)
            {
                if (desc.Name.Equals(name))
                    return desc;
            }

            foreach (var desc in CellDescriptors)
            {
                if (desc.Name.Equals(name))
                    return desc;
            }

            return null;
        }

        internal Field GetFieldByAttribute(StructDescriptor cell, string key, string value)
        {
            foreach (var f in cell.Fields)
            {
                string attribute_value;
                if (f.Attribute.AttributeValuePairs.TryGetValue(key, out attribute_value))
                {
                    if (attribute_value.Equals(value))
                    {
                        return f;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return Format
        /// </summary>
        /// <param name="descName"></param>
        /// <returns></returns>
        internal StructDescriptor FindStructDescriptor(string descName)
        {
            foreach (StructDescriptor fd in StructDescriptors)
                if (fd.Name == descName)
                    return fd;
            return null;
        }

        internal StructDescriptor FindStructOrCellDescriptor(string descName)
        {
            foreach (StructDescriptor fd in StructDescriptors)
                if (fd.Name == descName)
                    return fd;
            foreach (StructDescriptor fd in CellDescriptors)
                if (fd.Name == descName)
                    return fd;
            return null;
        }

        internal EnumDescriptor FindEnumDescriptor(string descName)
        {
            foreach (var desc in EnumDescriptors)
            {
                if (desc.Name == descName)
                    return desc;
            }
            return null;
        }

        internal ProtocolDescriptor FindProtocolDescriptor(string p)
        {
            foreach (var pDesc in ProtocolDescriptors)
                if (pDesc.Name == p)
                    return pDesc;
            return null;
        }

    }

    internal class Field
    {
        internal FieldType Type;
        public string Name;
        public List<Modifier> Modifiers;
        public string DefaultValueString;
        public AttributeDescriptor Attribute = new AttributeDescriptor();

        public Field() { }
        public Field(WrappedSyntaxNode node, SpecificationScript script)
        {
            Name = node.name;
            WrappedSyntaxTreeHelper.ReadAttributes(Attribute, node);
            ReadModifiers(node);
            foreach (var fieldType in node.children)
            {
                if (fieldType.type == "NFieldType")
                {
                    Type = ReadFieldType(fieldType, script);
                    AtomType at = Type as AtomType;
                    if (at != null && at.Name == "bit")
                    {
                        CompilerError.Throw("Currently bit as atom not supported");
                    }
                }
            }
            if (Type == null)
                CompilerError.Throw("FieldType not found!");
        }

        private void ReadModifiers(WrappedSyntaxNode node)
        {
            Modifiers = new List<Modifier>();
            foreach (var mod in node.modifierList)
            {
                switch (mod)
                {
                    case "optional":
                        Modifiers.Add(Modifier.Optional);
                        break;
                }
            }
        }

        internal FieldType ReadFieldType(WrappedSyntaxNode node, SpecificationScript script)
        {
            StructDescriptor struct_desc = null;
            EnumDescriptor enum_desc = null;
            FieldType ft;
            AtomType at;
            switch (node.data["fieldType"])
            {
                case "atom":
                    return ReadAtomFieldType(node);
                case "struct":
                    struct_desc = script.FindStructDescriptor(node.data["structName"]);
                    if (struct_desc.IsFixed())
                        return new FixedStructFieldType(struct_desc);
                    else
                        return new DynamicStructFieldType(struct_desc);
                case "enum":
                    enum_desc = script.FindEnumDescriptor(node.data["enumName"]);
                    return new EnumType(enum_desc);
                //return EnumStru
                case "list":
                    ft = ReadFieldType(node.children[0], script);
                    return new ListType(ft);
                case "array":
                    ft = ReadFieldType(node.children[0], script);
                    at = ft as AtomType;
                    return new ArrayType(ft as FixedFieldType, node.arrayDimensionList);
                default:
                    CompilerError.Throw("Internal error 303");
                    break;
            }
            return null;
        }

        private FieldType ReadAtomFieldType(WrappedSyntaxNode node)
        {
            switch (node.data["atomType"])
            {
                case "DateTime":
                    return new DateTimeType();
                case "Guid":
                    return new GuidType();
                case "string":
                    return new StringType();
                case "u8string":
                    return new U8StringType();
                case "???":
                    CompilerError.Throw("Unrecognized atomType");
                    return null;
                default:
                    return new AtomType(node.data["atomType"]);
            }
        }
    }

    public enum Modifier
    {
        Extern,
        Optional,
        Invisible,
    }
}
