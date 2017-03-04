using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    class AccessorCodeTemplate
    {
        public static string GenerateFieldPropertiesCode(StructDescriptor structDesc, bool isReadOnly)
        {
            CodeWriter cw = new CodeWriter();
            #region field properties
            for (int cnt = 0; cnt < structDesc.Fields.Count; ++cnt)
            {
                Field field = structDesc.Fields[cnt];
                bool currentFieldIsOptional = structDesc.OptionalFieldSequenceMap.ContainsKey(field);

                #region for optional fields
                int optionalFieldSequence = -1;
                OptionalFieldCalculator opt = new OptionalFieldCalculator(structDesc.OptionalFieldSequenceMap.Count);
                if (currentFieldIsOptional) //XXX will not work on Big-endian machines
                {
                    optionalFieldSequence = structDesc.OptionalFieldSequenceMap[field];
                }
                #endregion

                string pushcode = @"byte* targetPtr = CellPtr;
" + GenerateFieldPushPointerCode(structDesc, cnt, "this");

                /*
                 * For all fields except Atom and Enum, we lay a corresponded private
                 * accessor field pre-constructed.
                 * When an user calls the property for the accessor,
                 * the private accessor is returned.
                 */

                string accessor_field_name = field.Name + "_Accessor_Field";
                string accessor_type_name = TSLCompiler.GetAccessorTypeName(isReadOnly, field.Type);

                if (!(field.Type is AtomType || field.Type is EnumType))
                    cw += accessor_type_name + " " + accessor_field_name + ";";

                //For an optional field with name n, we add property bool Contains_n to the struct accessor
                //With a public getter and a private setter
                #region Contains property
                if (currentFieldIsOptional)
                {
                    cw += @"
            ///<summary>
            ///Represents the presence of the optional field " + field.Name + @".
            ///</summary>
            public bool Contains_" + field.Name + @"
            {
                get
                {
                    unchecked
                    {
                        return (" + opt.GenerateReadBitExpression(optionalFieldSequence, "this.CellPtr") + @");
                    }
                }
                internal set
                {
                    unchecked
                    {
                        if(value)
                            " + opt.GenerateMaskOnCode(optionalFieldSequence, "this.CellPtr") + @"
                        else
                            " + opt.GenerateMaskOffCode(optionalFieldSequence, "this.CellPtr") + @"
                    }
                }
            }";
                }
                #endregion

                //For an optional field with name n, we add method void Remove_n to the struct accessor
                #region Remove interface
                if (currentFieldIsOptional)
                {
                    cw += @"
            ///<summary>
            ///Removes the optional field " + field.Name + @" from the object being operated.
            ///</summary>
            public unsafe void Remove_" + field.Name + @"()
            {
                if(!this.Contains_" + field.Name + @")
                {
                    throw new Exception(""Optional field " + field.Name + @" doesn't exist for current cell."");
                }
                this.Contains_" + field.Name + @" = false;
                " + pushcode + @"
                byte* startPtr = targetPtr;
                " + field.Type.GeneratePushPointerCode() + @"
                this.ResizeFunction(startPtr,0,(int)(startPtr - targetPtr));
            }";
                }
                #endregion

                /***
                 * For a format type, accessor can be readonly/ RW accessor
                 * readonly format accessors don't have "set" in their
                 * properties.
                 */
                #region Push pointer (targetPtr) in position. Generate getters/setters
                cw += @"
        ///<summary>
        ///Provides in-place access to the object field " + field.Name + @".
        ///</summary>
        public unsafe " + accessor_type_name + " " + field.Name + @"
        {
            get
            {
                ";
                if (currentFieldIsOptional)
                    cw += @"
                if(!this.Contains_" + field.Name + @")
                {
                    throw new Exception(""Optional field " + field.Name + @" doesn't exist for current cell."");
                }";
                cw += @"
                " + pushcode;
                #region Tip
                //Tip: to better read this piece of code,
                //Open one fold(region) a time. :-)
                //Some of them contains a setter inside.
                #endregion
                #region if (field.Type is StructFieldType || field.Type is DynamicArrayType || field.Type is ArrayType||Guid||DateTime)
                if (field.Type is StructFieldType || field.Type is ArrayType || field.Type is GuidType || field.Type is DateTimeType)
                {
                    cw += @"
                " + accessor_field_name + @".CellPtr = targetPtr;
                " + accessor_field_name + @".CellID = this.CellID;
                return " + accessor_field_name + @";
            }
            set
            {
                if ((object)value == null) throw new ArgumentNullException(""The assigned variable is null."");
";
                    cw += pushcode;
                    if (field.Type is ArrayType)
                    {
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                " + accessor_field_name + @".CellID = this.CellID;
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;
                    " + ImplicitOperatorCodeTemplate.ArraySetPropertiesCode(field, accessor_field_name, true) + @"
                }else
                {
                    " + ImplicitOperatorCodeTemplate.ArraySetPropertiesCode(field, accessor_field_name, false) + @"
                }";
                        }
                        #endregion
                        #region mandatory
                        else
                        {
                            cw += @"
                " + ImplicitOperatorCodeTemplate.ArraySetPropertiesCode(field, accessor_field_name, false);
                        }
                        #endregion
                    }
                    else if (field.Type is StructFieldType)
                    {
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                " + accessor_field_name + @".CellID = this.CellID;
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;"
                    + ImplicitOperatorCodeTemplate.FormatSetPropertiesCode(field, true) + @"
                }else
                {
                    " + ImplicitOperatorCodeTemplate.FormatSetPropertiesCode(field, false) + @"
                }";
                        }
                        #endregion
                        #region mandatory
                        else
                        {
                            cw += ImplicitOperatorCodeTemplate.FormatSetPropertiesCode(field, false);
                        }
                        #endregion
                    }
                    else if (field.Type is GuidType)
                    {
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                " + accessor_field_name + @".CellID = this.CellID;
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;
                    " + ImplicitOperatorCodeTemplate.GuidSetPropertiesCode(field, true) + @"
                }else
                {
                    " + ImplicitOperatorCodeTemplate.GuidSetPropertiesCode(field, false) + @"
                }";
                        }
                        #endregion
                        #region mandatory
                        else
                        {
                            cw += @"
                " + ImplicitOperatorCodeTemplate.GuidSetPropertiesCode(field, false);
                        }
                        #endregion

                    }
                    else if (field.Type is DateTimeType)
                    {
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                " + accessor_field_name + @".CellID = this.CellID;
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;
                    " + ImplicitOperatorCodeTemplate.DateTimeSetPropertiesCode(field, true) + @"
                }else
                {
                    " + ImplicitOperatorCodeTemplate.DateTimeSetPropertiesCode(field, false) + @"
                }";
                        }
                        #endregion
                        #region mandatory
                        else
                        {
                            cw += @"
                " + ImplicitOperatorCodeTemplate.DateTimeSetPropertiesCode(field, false);
                        }
                        #endregion
                    }
                    cw += @"
            }
        }
";
                }
                #endregion
                #region else if (field.Type is ListType || field.Type is StringType || field.Type is U8StringType)
                else if (field.Type is ListType || field.Type is StringType || field.Type is U8StringType)
                {
                    cw += @"
                " + accessor_field_name + @".CellPtr = targetPtr + 4;
                " + accessor_field_name + @".CellID = this.CellID;
                return " + accessor_field_name + @";
            }
";
                    if (!isReadOnly)
                    {
                        cw += @"
            set
            {
                if ((object)value == null) throw new ArgumentNullException(""The assigned variable is null."");
                ";
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                ";
                            cw += pushcode + @"
                " + accessor_field_name + @".CellID = this.CellID;
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;
                    " + ImplicitOperatorCodeTemplate.ContainerSetPropertiesCode(field.Type, accessor_field_name, true) + @"
                }else
                {
                    " + ImplicitOperatorCodeTemplate.ContainerSetPropertiesCode(field.Type, accessor_field_name, false) + @"
                }";
                        }
                        #endregion
                        #region mandatory
                        else
                        {
                            cw += pushcode + @"
                " + accessor_field_name + @".CellID = this.CellID;
                " + ImplicitOperatorCodeTemplate.ContainerSetPropertiesCode(field.Type, accessor_field_name, false);
                        }
                        #endregion
                        cw += @"                
            }
";
                    }
                    cw += @"
    }
";
                }
                #endregion
                #region else if (field.Type is AtomType||EnumType)
                else if (field.Type is AtomType || field.Type is EnumType)
                {
                    #region template
                    /*
                            public unsafe int WordID
                            {
                                get
                                {
                                    byte* targetPtr = CellPtr;
                                    return *(int*)(targetPtr);
                                }
                                set
                                {
                                    byte* targetPtr = CellPtr;
                                    *(int*)(targetPtr) = value;
                                }
                            }
                     */
                    #endregion
                    cw += @"
                return *(" + field.Type.Name + @"*)(targetPtr);
            }
";
                    if (!isReadOnly)
                    {
                        cw += @"
            set
            {
                ";
                        #region optional
                        if (currentFieldIsOptional)
                        {
                            cw += @"
                bool creatingOptionalField = (!this.Contains_" + field.Name + @");
                ";
                            cw += pushcode + @"
                if(creatingOptionalField)
                {
                    this.Contains_" + field.Name + @" = true;
                    targetPtr = this.ResizeFunction(targetPtr, 0, " + GetAtomOrEnumLength(field).ToString(CultureInfo.InvariantCulture) + @");
                }
                *(" + field.Type.Name + @"*)(targetPtr) = value;";
                        }
                        #endregion //optional
                        #region mandatory
                        else
                        {
                            cw += pushcode + @"
                *(" + field.Type.Name + @"*)(targetPtr) = value;";
                        }
                        #endregion //mandatory
                        cw += @"
            }
";
                    }
                    cw += @"
        }
";
                }
                #endregion
                #endregion//getter / setter
            }
            #endregion
            return cw.ToString();
        }

        private static int GetAtomOrEnumLength(Field field)
        {
            if (field.Type is AtomType)
                return (field.Type as AtomType).Length;
            else
                //TODO enum length
                return 1;
        }

        internal static string GenerateOptionalFieldMap(StructDescriptor structDesc)
        {

            StringBuilder optional_fields = new StringBuilder();

            for (int i = 0; i < structDesc.Fields.Count; i++)
            {
                if (structDesc.Fields[i].Modifiers.Contains(Modifier.Optional))
                {
                    optional_fields.AppendFormat("\"{0}\"", structDesc.Fields[i].Name);
                    if (i != (structDesc.Fields.Count - 1))
                        optional_fields.Append(", ");
                }
            }

            string OptionalFields = @"
        internal static string[] optional_field_names;
        ///<summary>
        ///Get an array of the names of all optional fields for object type " + structDesc.Name + @".
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if(optional_field_names == null)
                optional_field_names = new string[] {" + optional_fields + @"};
            return optional_field_names;   
        }

        ///<summary>
        ///Get a list of the names of available optional fields in the object being operated by this accessor.
        ///</summary>
        internal List<string> GetNotNullOptionalFields()
        {
            List<string> list = new List<string>();
            BitArray ba = new BitArray(GetOptionalFieldMap());
            string[] optional_fields = GetOptionalFieldNames();
            for (int i = 0; i < ba.Count; i++)
            {
                if(ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }
";


            OptionalFieldCalculator opt = new OptionalFieldCalculator(structDesc.OptionalFieldSequenceMap.Count);
            if (opt.headerLength > 0)
            {
                return OptionalFields + @"
        internal unsafe byte[] GetOptionalFieldMap()
        {
            byte [] bytes = new byte[" + opt.headerLength + @"];
            Memory.Copy(CellPtr, 0, bytes, 0, " + opt.headerLength + @");
            return bytes;
        }

        internal static int OptionalFieldBitMapLength
        {
            get
            {
                return " + opt.headerLength + @";
            }
        }
";
            }
            else
            {
                return OptionalFields + @"
        internal unsafe byte[] GetOptionalFieldMap()
        {
            return new byte[0];
        }
";
            }
        }

        internal static string GenerateFieldPushPointerCode(StructDescriptor structDesc, int cnt, string cellName)
        {
            string indent = "            ";
            int pushnum = 0;
            CodeWriter ret = new CodeWriter();
            OptionalFieldCalculator opt = new OptionalFieldCalculator(structDesc.OptionalFieldSequenceMap.Count);
            #region Push pointer
            if (structDesc.OptionalFieldSequenceMap.Count != 0)//Contains optional fields.
            {
                ret += indent + "targetPtr += " + opt.headerLength + ";\r\n";
            }
            pushnum = 0;
            for (int i = 0; i < cnt; i++)//Push through fields before current one
            {
                bool pushFieldIsOptional = structDesc.OptionalFieldSequenceMap.ContainsKey(structDesc.Fields[i]);
                #region OptionalFieldHeader
                if (pushFieldIsOptional)
                {
                    if (structDesc.Fields[i].Type is DynamicFieldType)
                    {
                        if (pushnum != 0)
                            ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                        pushnum = 0;
                    }
                    ret += @"
                if(" + cellName + @".Contains_" + structDesc.Fields[i].Name + @")
                {
";
                }
                #endregion
                if (structDesc.Fields[i].Type is FixedFieldType)
                {
                    if (pushFieldIsOptional)//Do not include optional field in a batch fixed push
                        ret += indent + "targetPtr += " + (structDesc.Fields[i].Type as FixedFieldType).Length.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                    else
                        pushnum += (structDesc.Fields[i].Type as FixedFieldType).Length;
                }
                else
                {
                    if (!pushFieldIsOptional)
                    {
                        if (pushnum != 0)
                            ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
                        pushnum = 0;
                    }
                    ret += indent + (structDesc.Fields[i].Type as DynamicFieldType).GeneratePushPointerCode();
                }
                #region OptionalFieldFooter
                if (pushFieldIsOptional)
                {
                    ret += @"
                }
";
                }
                #endregion
            }
            if (pushnum != 0)
                ret += indent + "targetPtr += " + pushnum.ToString(CultureInfo.InvariantCulture) + ";\r\n";
            return ret;
            #endregion

        }
    }
}