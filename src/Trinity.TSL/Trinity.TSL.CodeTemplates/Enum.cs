using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trinity;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_enum","node->enumList")]
    [MAP_VAR("t_enum_name", "name")]
    [MAP_LIST("t_type", "enumEntryList", MemberOf = "t_enum")]
    [MAP_VAR("t_type_name", "name")]
    [MAP_VAR("t_type_value", "value")]
    [FOREACH]
    /// <summary>
    /// Represents the enum type t_enum_name defined in the TSL.
    /// </summary>
    public enum t_enum_name : byte
    {
        [FOREACH(",")]
        t_type_name = 9/*META_OUTPUT($t_type_value)*/
        /*END*/
    }
    /*END*/
}
