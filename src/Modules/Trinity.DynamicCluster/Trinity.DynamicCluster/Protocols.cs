#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
namespace Trinity.DynamicCluster
{
    
    public abstract partial class DynamicClusterBase : CommunicationModule
    {
        protected override void RegisterMessageHandler()
        {
            
            {
                
                MessageRegistry.RegisterMessageHandler((ushort)(this.SynReqRspIdOffset + (ushort)global::Trinity.DynamicCluster.TSL.CommunicationModule.DynamicCluster.SynReqRspMessageType.QueryChunkedRemoteStorageInformation), _QueryChunkedRemoteStorageInformationHandler);
                
            }
            
            {
                
                MessageRegistry.RegisterMessageHandler((ushort)(this.AsynReqIdOffset + (ushort)global::Trinity.DynamicCluster.TSL.CommunicationModule.DynamicCluster.AsynReqMessageType.MotivateRemoteStorageOnLeaving), _MotivateRemoteStorageOnLeavingHandler);
                
            }
            
        }
        
        private unsafe void _QueryChunkedRemoteStorageInformationHandler(SynReqRspArgs args)
        {
            var rsp = new _QueryChunkedRemoteStorageInformationReusltWriter();
            QueryChunkedRemoteStorageInformationHandler(rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }
        public abstract void QueryChunkedRemoteStorageInformationHandler(_QueryChunkedRemoteStorageInformationReusltWriter response);
        
        private unsafe void _MotivateRemoteStorageOnLeavingHandler(AsynReqArgs args)
        {
            MotivateRemoteStorageOnLeavingHandler(new _MotivateRemoteStorageOnLeavingRequestReader(args.Buffer, args.Offset));
            
        }
        
        public abstract void MotivateRemoteStorageOnLeavingHandler(_MotivateRemoteStorageOnLeavingRequestReader request);
        
    }
    
    public static class MessagePassingExtension
    {
        #region Server
        
        #endregion
        #region Proxy
        
        #endregion
        
    }
    #region Module
    
    public abstract partial class DynamicClusterBase : CommunicationModule
    {
        
        #region prototype definition template variables
        
        #endregion
        
        public unsafe _QueryChunkedRemoteStorageInformationReusltReader QueryChunkedRemoteStorageInformation( int moduleId)
        {
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);
            try
            {
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.SYNC_WITH_RSP ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::Trinity.DynamicCluster.TSL.CommunicationModule.DynamicCluster.SynReqRspMessageType.QueryChunkedRemoteStorageInformation;
                TrinityResponse response;
                this.SendMessage(moduleId, bufferPtr, TrinityProtocol.MsgHeader, out response);
                return new _QueryChunkedRemoteStorageInformationReusltReader(response.Buffer, response.Offset);
            }
            finally { Memory.free(bufferPtr); }
        }
        
        #region prototype definition template variables
        
        #endregion
        
        public unsafe void MotivateRemoteStorageOnLeaving( int moduleId, _MotivateRemoteStorageOnLeavingRequestWriter msg)
        {
            byte* bufferPtr = msg.buffer;
            try
            {
                *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.ASYNC ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::Trinity.DynamicCluster.TSL.CommunicationModule.DynamicCluster.AsynReqMessageType.MotivateRemoteStorageOnLeaving;
                this.SendMessage(moduleId, bufferPtr, msg.Length + TrinityProtocol.MsgHeader);
            }
            finally { }
        }
        
    }
    
    #endregion
    
}

#pragma warning restore 162,168,649,660,661,1522
