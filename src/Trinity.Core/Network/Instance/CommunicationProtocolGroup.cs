// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Trinity.Network
{
    internal delegate Task HttpRequestDispatcher(HttpListenerContext context, string rawUrl, string handlerName);
    /// <summary>
    /// Represents a group of communication protocols.
    /// </summary>
    public abstract class CommunicationProtocolGroup : ICommunicationModuleRegistry
    {
        #region Fields
        /// <summary>
        /// Holds registered module types.
        /// </summary>
        protected HashSet<Type> m_RegisteredModuleTypes = new HashSet<Type>();
        /// <summary>
        /// Raised when the host communication instance is started.
        /// </summary>
        public event Func<Task> Started = delegate { return Task.CompletedTask; };
        #endregion
        internal Task _RaiseStartedEventAsync()
        {
            return Started();
        }

        internal abstract CommunicationInstance GetCommunicationInstance();

        /// <summary>
        /// Register user-defined message handlers.
        /// </summary>
        protected abstract void RegisterMessageHandler();

        /// <summary>
        /// Handles requests on the root endpoint.
        /// </summary>
        /// <param name="ctx">A <see cref="HttpListenerContext"/> object.</param>
        protected abstract Task RootHttpHandlerAsync(HttpListenerContext ctx);

        /// <summary>
        /// Dispatches a HTTP request to a non-root handler.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="endpointName">The name of the endpoint.</param>
        /// <param name="url"></param>
        protected abstract Task DispatchHttpRequestAsync(HttpListenerContext ctx, string endpointName, string url);

        #region Public
        /// <summary>
        /// Retrieves the communication schema associated with the current instance.
        /// </summary>
        /// <returns>A communication schema.</returns>
        public ICommunicationSchema GetCommunicationSchema()
        {
            CommunicationSchemaAttribute schema_attr = this
                .GetType()
                .GetCustomAttributes(typeof(CommunicationSchemaAttribute), inherit: true)
                .FirstOrDefault() as CommunicationSchemaAttribute;

            if (schema_attr == null) return new DefaultCommunicationSchema();
            try { return schema_attr.CommunicationSchemaType.GetConstructor(new Type[] { }).Invoke(new object[] { }) as ICommunicationSchema; }
            catch { return new DefaultCommunicationSchema(); }
        }

        /// <summary>
        /// Retrieves all registered communication module types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRegisteredCommunicationModuleTypes()
        {
            return m_RegisteredModuleTypes.ToList();
        }

        /// <inheritdoc />
        public T GetCommunicationModule<T>() where T : CommunicationModule
        {
            return GetCommunicationInstance()._GetCommunicationModule_impl<T>();
        }

        /// <summary>
        /// Registers a communication module type. The registered type will be instantiated when the hosting communication instance starts up.
        /// Duplicated registrations of the same module are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the communication module.</typeparam
        public void RegisterCommunicationModule<T>() where T : CommunicationModule
        {
            m_RegisteredModuleTypes.Add(typeof(T));
        }

        #endregion
    }
}
