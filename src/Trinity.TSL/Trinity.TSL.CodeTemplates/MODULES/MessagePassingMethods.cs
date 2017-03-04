#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using t_Namespace.MODULES;
using Trinity;
using Trinity.Network;
using Trinity.Network.Http;
using Trinity.TSL;
using Trinity.TSL.Lib;

namespace t_Namespace
{
    /// <summary>
    /// This module generates message passing methods.
    /// TODO
    /// </summary>
    public static class MessagePassingExtensions
    {
        [MODULE_BEGIN]
        [TARGET("NProtocolGroup")]
        [MAP_VAR("t_instance", "node")]
        [MAP_VAR("t_base_class_name", "context->m_arguments[0]")]
        [MAP_VAR("t_instance_name", "node->name")]
        [META_VAR("NProtocol*", "protocol")]
        [META_VAR("std::string", "method_name")]
        [MAP_VAR("t_method_name", "%method_name")]

        [MAP_LIST("t_protocol_list", "protocolList", MemberOf = "t_instance")]
        [MAP_VAR("t_protocol_name", "name", MemberOf = "t_protocol_list")]

        [FOREACH]
        [USE_LIST("t_protocol_list")]
        [META("%protocol = tsl->find_protocol($t_protocol_name);")]



        [META("%method_name = $t_protocol_name + \"To\" + $t_instance_name")]
        public unsafe static t_return_type t_method_name(this Trinity.Storage.MemoryCloud storage, int t_instance_nameId)
        {
            /*MUTE*/
            return null;
            /*MUTE_END*/
        }
        [END]//FOREACH

        [MODULE_END]
        #region mute
        [MUTE]
        public class t_return_type{}
        /*MUTE_END*/
        #endregion
    }
}
