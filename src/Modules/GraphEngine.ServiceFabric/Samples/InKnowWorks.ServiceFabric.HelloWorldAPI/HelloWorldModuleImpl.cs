using System;

namespace InKnowWorks.ServiceFabric.HelloWorldAPI
{
    [Trinity.Extension.AutoRegisteredCommunicationModule]
    public class HelloWorldModuleImpl : HelloWorkdAPIModuleBase
    {

        public override string GetModuleName()
        {
            return "HelloWorldModuleImpl";
        }

        public override void HelloWorldProtocolHandler(HelloWorldRequestReader request, HelloWorldResponseWriter response)
        {
            throw new NotImplementedException();
        }
    }
}
