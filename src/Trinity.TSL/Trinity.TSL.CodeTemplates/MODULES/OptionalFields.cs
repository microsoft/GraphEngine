using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.TSL;

namespace t_Namespace
{
    unsafe class CellOrStruct : __meta
    {
        [MODULE_BEGIN]
        [TARGET("NStructBase")]
        [META_VAR("OptionalFieldCalculator", "optcalc", "OptionalFieldCalculator(node)")]
        [MAP_VAR("t_int", "%optcalc.headerLength")]
        [MAP_LIST("t_field", "node->fieldList")]
        [MAP_VAR("t_field", "")]
        [MAP_VAR("t_field_name", "name")]
        internal static string[] optional_field_names = null;
        ///<summary>
        ///Get an array of the names of all optional fields for object type t_struct_name.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if (optional_field_names == null)
                optional_field_names = new string[]
                {
                    /*FOREACH(",")*/
                    /*META("if(!$t_field->is_optional())continue;")*/
                    "t_field_name"
                    /*END*/
                };
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
                if (ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }

        internal unsafe byte[] GetOptionalFieldMap()
        {
            IF("%optcalc.headerLength > 0");
            byte [] bytes = new byte[t_int];
            Memory.Copy(m_ptr, 0, bytes, 0, t_int);
            return bytes;
            ELSE();
            return new byte[0];
            END();
        }

        [MODULE_END]
        byte* m_ptr;
    }
}
