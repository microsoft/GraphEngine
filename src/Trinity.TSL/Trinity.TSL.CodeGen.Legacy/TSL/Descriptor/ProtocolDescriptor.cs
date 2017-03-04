using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Network.Messaging;

namespace Trinity.TSL
{
    internal class ProtocolDescriptor : AbstractStruct
    {
        public TrinityMessageType ProtocolType;
        public StructDescriptor Request;
        public StructDescriptor Response;

        public ProtocolDescriptor(WrappedSyntaxNode node, SpecificationScript script)
        {
            Name = node.name;
            bool isASync = false;

            isASync = (node.data["protocolType"] == "asyn");
            string req_name, rsp_name;
            switch (node.data["requestType"])
            {
                case "void":
                    req_name = "VOID";
                    break;
                case "stream":
                    req_name = "STREAM";
                    break;
                default:
                    req_name = node.data["requestStruct"];
                    break;
            }
            switch (node.data["responseType"])
            {
                case "void":
                    rsp_name = "VOID";
                    break;
                case "stream":
                    rsp_name = "STREAM";
                    break;
                default:
                    rsp_name = node.data["responseStruct"];
                    break;
            }
            Request  = ReadMessageType(req_name, script);
            Response = ReadMessageType(rsp_name, script);

            if (isASync)
            {
                ProtocolType = TrinityMessageType.ASYNC;
            }
            else
            {
                if (Response == StructDescriptor.VOID)
                    ProtocolType = TrinityMessageType.SYNC;
                else
                    ProtocolType = TrinityMessageType.SYNC_WITH_RSP;
            }
        }

        private StructDescriptor ReadMessageType(string name, SpecificationScript script)
        {
            if (name == "VOID")
                return StructDescriptor.VOID;
            var ret = script.FindStructOrCellDescriptor(name);
            return ret;
        }

        internal string RegistrationLine(SpecificationScript script, string pg_name, ProtocolGroupType pg_type, ProtocolDescriptor protocol_desc)
        {
            if (pg_type == ProtocolGroupType.CommunicationModule)
            {
                string offset_var_name = "";
                switch(protocol_desc.ProtocolType)
                {
                    case TrinityMessageType.SYNC:
                        offset_var_name = "SynReqIdOffset";
                        break;
                    case TrinityMessageType.ASYNC:
                        offset_var_name = "AsynReqIdOffset";
                        break;
                    case TrinityMessageType.SYNC_WITH_RSP:
                        offset_var_name = "SynReqRspIdOffset";
                        break;
                }
                return @"MessageRegistry.RegisterMessageHandler((ushort)(this." + offset_var_name + " + (ushort)global::" + script.RootNamespace + ".TSL." + pg_type.ToString() + "." + pg_name + "." + SpecificationScript.CurrentScript.NamingPrefix + MsgType + @"MessageType." + Name + @"), _" + Name + @"Handler);
";
            }
            else
            {
                return @"MessageRegistry.RegisterMessageHandler((ushort)global::" + script.RootNamespace + ".TSL." + pg_type.ToString() + "." + pg_name + "." + SpecificationScript.CurrentScript.NamingPrefix + MsgType + @"MessageType." + Name + @", _" + Name + @"Handler);
";
            }
        }


        internal string InternalAsynRspBlock
        {
            get
            {
                return @"
        private unsafe void _" + Name + @"ResponseHandler(" + @"AsynRspArgs args)
        {
            fixed (byte* p = &args.Buffer[args.Offset])
            {
                " + Name + @"ResponseHandler( new " + Response.Name + @"Reader(p));
            }
        }";
            }
        }

        internal string PublicAsynRspBlock
        {
            get
            {
                return @"public abstract void " + Name + @"ResponseHandler(" + Response.Name + @"Reader msg);";
            }
        }

        internal string PublicAsynRspImplBlock
        {
            get
            {
                return @"public void " + Name + @"ResponseHandler(" + Response.Name + @"AsynRsp msg)
        {
            throw new NotImplementedException();
        }";
            }
        }


        internal string InternalHandlerBlock
        {
            get
            {
                if (
                    ProtocolType == TrinityMessageType.ASYNC ||
                    ProtocolType == TrinityMessageType.SYNC)
                {
                    if (Request != StructDescriptor.VOID)
                        return @"
        private unsafe void _" + Name + @"Handler(" + MsgType + @"Args args)
        {
            " + Name + @"Handler(new " + Request.Name + @"Reader(args.Buffer, args.Offset));
        }";
                    else
                        return @"
        private unsafe void _" + Name + @"Handler(" + MsgType + @"Args args)
        {
            " + Name + @"Handler();
        }";
                }
                else if (ProtocolType == TrinityMessageType.SYNC_WITH_RSP)
                {
                    if (Request != StructDescriptor.VOID)
                        return @"
        private unsafe void _" + Name + @"Handler(" + MsgType + @"Args args)
        {
            var rsp = new " + Response.Name + @"Writer();
            " + Name + @"Handler(new " + Request.Name + @"Reader(args.Buffer, args.Offset), rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage( rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }";
                    else
                        return @"
        private unsafe void _" + Name + @"Handler(" + MsgType + @"Args args)
        {
            var rsp = new " + Response.Name + @"Writer();
        " + Name + @"Handler( rsp );
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage( rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }";
                }
                return "";
            }
        }

        internal string PublicHandlerBlock
        {
            get
            {
                if (ProtocolType == TrinityMessageType.SYNC_WITH_RSP && Request == StructDescriptor.VOID)
                    return @"public abstract void " + Name + @"Handler(" + Response.Name + @"Writer response);";
                else if (ProtocolType == TrinityMessageType.SYNC_WITH_RSP && Request != StructDescriptor.VOID)
                    return @"public abstract void " + Name + @"Handler(" + Request.Name + @"Reader request, " + Response.Name + @"Writer response);";
                /* Else, SYNC or ASYNC, no RSP */
                else if (Request == StructDescriptor.VOID)
                    return @"public abstract void " + Name + @"Handler();";
                else
                    return @"public abstract void " + Name + @"Handler(" + Request.Name + @"Reader request);";
            }
        }

        internal string MsgType
        {
            get
            {
                switch (ProtocolType)
                {
                    case TrinityMessageType.SYNC:
                        return "SynReq";
                    case TrinityMessageType.SYNC_WITH_RSP:
                        return "SynReqRsp";
                    case TrinityMessageType.ASYNC:
                        return "AsynReq";
                    default:
                        return "";
                }
            }
        }

        internal string MsgTypeFullName
        {
            get
            {
                switch (ProtocolType)
                {
                    case TrinityMessageType.SYNC:
                        return "TrinityMessageType.SYNC";
                    case TrinityMessageType.SYNC_WITH_RSP:
                        return "TrinityMessageType.SYNC_WITH_RSP";
                    case TrinityMessageType.ASYNC:
                        return "TrinityMessageType.ASYNC";
                    default:
                        return "";
                }
            }
        }

        internal bool IsSyn
        {
            get
            {
                return ProtocolType == TrinityMessageType.SYNC || ProtocolType == TrinityMessageType.SYNC_WITH_RSP;
            }
        }

    }
}
