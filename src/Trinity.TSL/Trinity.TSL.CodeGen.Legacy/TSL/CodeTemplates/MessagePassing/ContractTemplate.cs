using Trinity.TSL;
using Trinity.Network.Messaging;
using Trinity.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Trinity.TSL
{
    internal static class ContractTemplate
    {
        /***********************
         Example:
          TrinityResponse response;
          storage.SendMessageToServer(
              serverId,
              msg.buffer,
              msg.Length + TrinityProtocol.MsgHeader, out response);
         ************************/
        private static string GenerateSendMessageMethodCall(
            ProtocolGroupDescriptor proto_group,
            string id,
            string buffer,
            string length,
            bool withResponse)
        {
            string header = "";
            string footer = "";
            string sendMessageMethod = "";

            if (withResponse)
            {
                header = @"
            TrinityResponse response;";
                footer = @", out response";
            }


            switch (proto_group.Type)
            {
                case ProtocolGroupType.TrinityServer:
                    sendMessageMethod = "storage.SendMessageToServer";
                    break;
                case ProtocolGroupType.TrinityProxy:
                    sendMessageMethod = "storage.SendMessageToProxy";
                    break;
                case ProtocolGroupType.CommunicationModule:
                    sendMessageMethod = "this.SendMessage";
                    break;
            }

            return header + @"
            " + sendMessageMethod + String.Format(CultureInfo.InvariantCulture, @"(
                {0},
                {1},
                {2}{3});", id, buffer, length, footer);
        }

        private static string GenerateMessagePassingMethod(SpecificationScript script, ProtocolGroupDescriptor proto_group, ProtocolDescriptor proto_desc)
        {
            CodeWriter cw = new CodeWriter();

            string pg_name = proto_group.Name;
            bool has_request  = (proto_desc.Request != StructDescriptor.VOID);
            bool has_response = (proto_desc.ProtocolType == TrinityMessageType.SYNC_WITH_RSP);

            string return_stmt;
            string method_modifier;
            string method_return_type;
            string method_name;

            string bufferPtr_assign_stmt;
            string bufferPtr_free_stmt;

            string instance_id;
            string buffer_len_expr;
            string arg_extension_method_target;
            string arg_message_writer;

            switch (proto_group.Type)
            {
                case ProtocolGroupType.CommunicationModule:
                    method_modifier             = "";
                    instance_id                 = "moduleId";
                    method_name                 = proto_desc.Name;
                    arg_extension_method_target = "";
                    break;
                case ProtocolGroupType.TrinityProxy:
                    method_modifier             = "static";
                    instance_id                 = "proxyId";
                    method_name                 = proto_desc.Name + @"To" + pg_name;
                    arg_extension_method_target = "this Trinity.Storage.MemoryCloud storage, ";
                    break;
                case ProtocolGroupType.TrinityServer:
                    method_modifier             = "static";
                    instance_id                 = "serverId";
                    method_name                 = proto_desc.Name + @"To" + pg_name;
                    arg_extension_method_target = "this Trinity.Storage.MemoryCloud storage, ";
                    break;
                default:
                    throw new Exception("Internal error T5015");
            }

            if (has_response)
            {
                return_stmt        = "return new " + proto_desc.Response.Name + @"Reader(response.Buffer, response.Offset);";
                method_return_type = proto_desc.Response.Name + @"Reader";
            }
            else
            {
                return_stmt        = "";
                method_return_type = "void";
            }

            if (has_request)
            {
                bufferPtr_assign_stmt = @"
            byte* bufferPtr = msg.buffer;";
                bufferPtr_free_stmt   = "";
                buffer_len_expr       =  "msg.Length + ";
                arg_message_writer    = ", " + proto_desc.Request.Name + @"Writer msg";
            }
            else
            {
                bufferPtr_assign_stmt = @"
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);";
                bufferPtr_free_stmt   = @"
            Memory.free(bufferPtr);";
                buffer_len_expr       = "";
                arg_message_writer    = "";
            }

            cw += @"
        public unsafe " + method_modifier + " " + method_return_type + " " + method_name + "(" + arg_extension_method_target + @"int " + instance_id + arg_message_writer + @")
        {" + 
                  bufferPtr_assign_stmt + @"
            try {
            *(int*)(bufferPtr) = " + buffer_len_expr + @"TrinityProtocol.TrinityMsgHeader;
            *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte) TrinityMessageType." + proto_desc.ProtocolType + @";
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::" + script.RootNamespace + ".TSL." + proto_group.Type + "." + pg_name + "." +
                  SpecificationScript.CurrentScript.NamingPrefix + proto_desc.MsgType + "MessageType." + proto_desc.Name + @";" +
                  GenerateSendMessageMethodCall(proto_group, instance_id, "bufferPtr", buffer_len_expr + "TrinityProtocol.MsgHeader", withResponse: has_response) + 
                  return_stmt + @"
            } finally {" + bufferPtr_free_stmt + @" }
        }
";
            return cw;
        }

        internal static string GenerateCode()
        {
            CodeWriter cw = new CodeWriter();
            SpecificationScript script = SpecificationScript.CurrentScript;

            // Server and proxy message passing interfaces are bound to MemoryCloud.
            cw += @"
    public static class " + script.NamingPrefix + @"MessagePassingExtension
    {" +
            String.Join("\r\n",
                Enumerable
                .Concat(script.ServerDescriptors, script.ProxyDescriptors)
                .SelectMany(pg => pg.ProtocolDescriptors.Select(proto => GenerateMessagePassingMethod(script, pg, proto)))) + @"
    }
";

            // Module MPIs are bound to their own classes
            foreach (var module in script.ModuleDescriptors)
            {
                cw += @"
    public abstract partial class " + module.Name + @"Base : CommunicationModule
    {" + 
            String.Join("\r\n",
                module.ProtocolDescriptors.Select(proto => GenerateMessagePassingMethod(script, module, proto))) + @"
    }
";
            }

            return cw;
        }
    }
}
