using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_server", "node->serverList")]
    [MAP_LIST("t_proxy", "node->proxyList")]
    [MAP_LIST("t_module", "node->moduleList")]
    [MAP_VAR("t_server", "")]
    [MAP_VAR("t_proxy", "")]
    [MAP_VAR("t_module", "")]
    [MAP_VAR("t_server_name", "name")]
    [MAP_VAR("t_proxy_name", "name")]
    [MAP_VAR("t_module_name", "name")]

    [FOREACH]
    [USE_LIST("t_server")]
    [MODULE_CALL("CommunicationClass", "$t_server")]
    [END]

    [FOREACH]
    [USE_LIST("t_proxy")]
    [MODULE_CALL("CommunicationClass", "$t_proxy")]
    [END]

    [FOREACH]
    [USE_LIST("t_module")]
    [MODULE_CALL("CommunicationClass", "$t_module")]
    [END]

    [MUTE]
    class protocol_place_holder { }
    /*MUTE_END*/

    /*FOREACH*/
    /*USE_LIST("t_server")*/
    namespace t_server_name
    {
        public static class MessagePassingExtension
        {
            /*MODULE_CALL("MessagePassingMethods", "$t_server")*/
        }
    }
    /*END*/

    /*FOREACH*/
    /*USE_LIST("t_proxy")*/
    namespace t_proxy_name
    {
        public static class MessagePassingExtension
        {
            /*MODULE_CALL("MessagePassingMethods", "$t_proxy")*/
        }
    }
    /*END*/

    /*FOREACH*/
    /*USE_LIST("t_module")*/
    namespace t_module_name
    {
        public static class MessagePassingExtension
        {
            /*MODULE_CALL("MessagePassingMethods", "$t_module")*/
        }
    }
    /*END*/

    /*MUTE*/
    namespace t_comm_name
    {
        public static class MessagePassingExtension
        {
            internal static Task t_protocol_nameAsync(IMessagePassingEndpoint messagePassingEndpoint)
            {
                throw new NotImplementedException();
            }

            internal static Task t_protocol_nameAsync(IMessagePassingEndpoint messagePassingEndpoint, t_protocol_requestWriter msg)
            {
                throw new NotImplementedException();
            }

            internal static Task<t_protocol_responseReader> t_protocol_name_2Async(IMessagePassingEndpoint messagePassingEndpoint)
            {
                throw new NotImplementedException();
            }

            internal static Task<t_protocol_responseReader> t_protocol_name_2Async(IMessagePassingEndpoint messagePassingEndpoint, t_protocol_requestWriter msg)
            {
                throw new NotImplementedException();
            }

            internal static Task<t_protocol_responseReader> t_protocol_name_3Async(IMessagePassingEndpoint messagePassingEndpoint)
            {
                throw new NotImplementedException();
            }

            internal static Task<t_protocol_responseReader> t_protocol_name_3Async(IMessagePassingEndpoint messagePassingEndpoint, t_protocol_requestWriter msg)
            {
                throw new NotImplementedException();
            }
        }
    }
    /*MUTE_END*/

    #region Legacy
    public static class LegacyMessagePassingExtension
    {
        [FOREACH]
        [USE_LIST("t_server")]
        [MODULE_CALL("LegacyMessagePassingMethods", "$t_server")]
        [END]

        [FOREACH]
        [USE_LIST("t_proxy")]
        [MODULE_CALL("LegacyMessagePassingMethods", "$t_proxy")]
        [END]

        [MUTE]
        class mpi_place_holder { }
        /*MUTE_END*/
    }

    [FOREACH]
    [USE_LIST("t_module")]
    public abstract partial class t_module_nameBase : CommunicationModule
    {
        [MODULE_CALL("LegacyMessagePassingMethods", "$t_module")]
        [MUTE]
        class mpi_place_holder { }
        /*MUTE_END*/
    }
    /*END*/
    #endregion
}
