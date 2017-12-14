using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Wcf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Client
{
/// <summary>
/// An <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ICommunicationClientFactory`1" /> that uses
/// Windows Communication Foundation to create <see cref="T:Microsoft.ServiceFabric.Services.Communication.Wcf.Client.WcfCommunicationClient`1" />
/// to communicate with stateless and stateful services that are using 
/// <see cref="T:Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime.WcfCommunicationListener`1" />.
/// </summary>
/// <typeparam name="TServiceContract">WCF based service contract</typeparam>
public class WcfCommunicationClientFactory<TServiceContract> : CommunicationClientFactoryBase<WcfCommunicationClient<TServiceContract>> where TServiceContract : class
{
[CompilerGenerated]
[Serializable]
private sealed class <>c
{
public static readonly WcfCommunicationClientFactory<TServiceContract>.<>c <>9 = new WcfCommunicationClientFactory<TServiceContract>.<>c();

public static Func<Exception, bool> <>9__4_0;

internal bool <CreateClientAsync>b__4_0(Exception x)
{
return x is TimeoutException;
}
}

private readonly Binding clientBinding;

private readonly object callbackObject;

/// <summary>
/// Constructs a factory to create clients using WCF to communicate with the services.
/// </summary>
/// <param name="clientBinding">
/// WCF binding to use for the client. If the client binding is not specified or null,
/// a default client binding is created using 
/// <see cref="M:Microsoft.ServiceFabric.Services.Communication.Wcf.WcfUtility.CreateTcpClientBinding(System.Int64,System.TimeSpan,System.TimeSpan)" /> method 
/// which creates a <see cref="T:System.ServiceModel.NetTcpBinding" /> with no security.
/// </param>
/// <param name="exceptionHandlers">
/// Exception handlers to handle the exceptions encountered in communicating with the service.
/// </param>
/// <param name="servicePartitionResolver">
/// Service partition resolver to resolve the service endpoints. If not specified, a default 
/// service partition resolver returned by <see cref="M:Microsoft.ServiceFabric.Services.Client.ServicePartitionResolver.GetDefault" /> is used.
/// </param>
/// <param name="traceId">
/// Id to use in diagnostics traces from this component.
/// </param>
/// <param name="callback">
/// The callback client that receives the callbacks from the service.
/// </param>
public WcfCommunicationClientFactory(Binding clientBinding = null, IEnumerable<IExceptionHandler> exceptionHandlers = null, IServicePartitionResolver servicePartitionResolver = null, string traceId = null, object callback = null) : base(servicePartitionResolver, WcfCommunicationClientFactory<TServiceContract>.GetExceptionHandlers(exceptionHandlers), traceId)
{
if (clientBinding == null)
{
clientBinding = WcfUtility.DefaultTcpClientBinding;
}
this.clientBinding = clientBinding;
this.callbackObject = callback;
}

/// <summary>
/// Creates WCF communication clients to communicate over the given channel.
/// </summary>
/// <param name="channel">Service contract based WCF channel.</param>
/// <returns>The communication client that was created</returns>
protected virtual WcfCommunicationClient<TServiceContract> CreateWcfCommunicationClient(TServiceContract channel)
{
return new WcfCommunicationClient<TServiceContract>(channel);
}

/// <summary>
/// Creates a communication client for the given endpoint address.
/// </summary>
/// <param name="endpoint">Endpoint address where the service is listening</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>The communication client that was created</returns>
protected override async Task<WcfCommunicationClient<TServiceContract>> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
{
EndpointAddress endpointAddress = new EndpointAddress(endpoint);
TServiceContract tServiceContract = (this.callbackObject != null) ? DuplexChannelFactory<TServiceContract>.CreateChannel(this.callbackObject, this.clientBinding, endpointAddress) : ChannelFactory<TServiceContract>.CreateChannel(this.clientBinding, endpointAddress);
IClientChannel clientChannel = (IClientChannel)((object)tServiceContract);
Exception ex = null;
try
{
Task task = Task.Factory.FromAsync(clientChannel.BeginOpen(this.clientBinding.OpenTimeout, null, null), new Action<IAsyncResult>(clientChannel.EndOpen));
if (await Task.WhenAny(new Task[]
{
task,
Task.Delay(this.clientBinding.OpenTimeout, cancellationToken)
}) != task)
{
clientChannel.Abort();
throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, SR.ErrorCommunicationClientOpenTimeout, new object[]
{
this.clientBinding.OpenTimeout
}));
}
if (task.Exception != null)
{
throw task.Exception;
}
task = null;
}
catch (AggregateException var_4_1B2)
{
Func<Exception, bool> arg_1D5_1;
if ((arg_1D5_1 = WcfCommunicationClientFactory<TServiceContract>.<>c.<>9__4_0) == null)
{
arg_1D5_1 = (WcfCommunicationClientFactory<TServiceContract>.<>c.<>9__4_0 = new Func<Exception, bool>(WcfCommunicationClientFactory<TServiceContract>.<>c.<>9.<CreateClientAsync>b__4_0));
}
var_4_1B2.Handle(arg_1D5_1);
ex = var_4_1B2;
}
catch (TimeoutException ex)
{
}
if (ex != null)
{
throw new EndpointNotFoundException(ex.Message, ex);
}
clientChannel.OperationTimeout = this.clientBinding.ReceiveTimeout;
return this.CreateWcfCommunicationClient(tServiceContract);
}

/// <summary>
/// Returns true if the client is still valid. Connection oriented transports can use this method to indicate that the client is no longer
/// connected to the service.
/// </summary>
/// <param name="client">WCF communication client</param>
/// <returns>true if the client is valid, false otherwise</returns>
protected override bool ValidateClient(WcfCommunicationClient<TServiceContract> client)
{
return client.ClientChannel.State == CommunicationState.Opened;
}

/// <summary>
/// Returns true if the client is still valid and connected to the endpoint specified in the parameter.
/// </summary>
/// <param name="endpoint">endpoint string</param>
/// <param name="client">WCF communication client</param>
/// <returns>true if the client is valid, false otherwise</returns>
protected override bool ValidateClient(string endpoint, WcfCommunicationClient<TServiceContract> client)
{
IClientChannel clientChannel = client.ClientChannel;
return clientChannel.State == CommunicationState.Opened && clientChannel.RemoteAddress.Uri.Equals(new Uri(endpoint));
}

/// <summary>
/// Aborts the given client
/// </summary>
/// <param name="client">Communication client</param>
protected override void AbortClient(WcfCommunicationClient<TServiceContract> client)
{
client.ClientChannel.Abort();
}

private static IEnumerable<IExceptionHandler> GetExceptionHandlers(IEnumerable<IExceptionHandler> exceptionHandlers)
{
List<IExceptionHandler> list = new List<IExceptionHandler>();
if (exceptionHandlers != null)
{
list.AddRange(exceptionHandlers);
}
list.Add(new WcfExceptionHandler());
return list;
}
}
}