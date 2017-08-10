// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network
{
    /// <summary>
    /// Specifies that a communication protocol group is associated with a communication schema.
    /// It is required that a communication protocol group is tagged with this attribute, so that the communication
    /// subsystem can retrive the schema of a protocol group and perform communication schema verification and module discovery.
    /// </summary>
    public class CommunicationSchemaAttribute : Attribute
    {
        /// <summary>
        /// The type of the associated communication schema.
        /// </summary>
        public Type CommunicationSchemaType { get; private set; }
        /// <summary>
        /// Constructs a CommunicationSchemaAttribute with a given communication schema type.
        /// </summary>
        /// <param name="schemaType">The type of the communication schema class. The schema class must implement <see cref="Trinity.Network.ICommunicationSchema"/>.</param>
        public CommunicationSchemaAttribute(Type schemaType)
        {
            this.CommunicationSchemaType = schemaType;
        }
    }

    /// <summary>
    /// Represents the communication schema associated with a TrinityServer or TrinityProxy.
    /// </summary>
    public interface ICommunicationSchema
    {
        /// <summary>
        /// Returns the descriptors of synchronous-request-no-response protocols.
        /// </summary>
        IEnumerable<IProtocolDescriptor> SynReqProtocolDescriptors { get; }
        /// <summary>
        /// Returns the descriptors of synchronous-request-with-response protocols.
        /// </summary>
        IEnumerable<IProtocolDescriptor> SynReqRspProtocolDescriptors { get; }
        /// <summary>
        /// Returns the descriptors of asynchronous-request-no-response protocols.
        /// </summary>
        IEnumerable<IProtocolDescriptor> AsynReqProtocolDescriptors { get; }
        /// <summary>
        /// Returns the name of the communication schema.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Returns the available http endpoint names.
        /// </summary>
        IEnumerable<string> HttpEndpointNames { get; }
    }

    internal class DefaultCommunicationSchema : ICommunicationSchema
    {
        public IEnumerable<IProtocolDescriptor> SynReqProtocolDescriptors
        {
            get { yield break; }
        }

        public IEnumerable<IProtocolDescriptor> SynReqRspProtocolDescriptors
        {
            get { yield break; }
        }

        public IEnumerable<IProtocolDescriptor> AsynReqProtocolDescriptors
        {
            get { yield break; }
        }

        public string Name
        {
            get { return DefaultCommunicationSchema.GetName(); }
        }

        public static string GetName()
        {
            return "DefaultCommunicationSchema"; 
        }

        public IEnumerable<string> HttpEndpointNames
        {
            get { yield break; }
        }
    }

    internal static class CommunicationSchemaSerializer
    {
        internal static string SerializeProtocols(ICommunicationSchema schema)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("[" + String.Join(",", schema.SynReqProtocolDescriptors.Select(_ => SerializeProtocol(_))) + "]");
            sb.Append("[" + String.Join(",", schema.SynReqRspProtocolDescriptors.Select(_ => SerializeProtocol(_))) + "]");
            sb.Append("[" + String.Join(",", schema.AsynReqProtocolDescriptors.Select(_ => SerializeProtocol(_))) + "]");
            sb.Append("}");
            return sb.ToString();
        }

        internal static string SerializeProtocol(IProtocolDescriptor protocol)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(String.Join("|", 
                protocol.Name,
                protocol.RequestSignature,
                protocol.ResponseSignature,
                protocol.Type.ToString()));
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents errors that occur when communication schema does not match.
    /// </summary>
    public class CommunicationSchemaNotMatchingException : Exception
    {
        internal CommunicationSchemaNotMatchingException(string schema_name, string schema_sig)
        {
            this.Data["name"]      = schema_name;
            this.Data["signature"] = schema_sig;
        }
    }
}
