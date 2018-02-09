using System;

namespace Trinity.Client.TestProtocols
{
    public class TrinityClientTestModule : TrinityClientTestModuleBase
    {
        public override string GetModuleName() => "TrinityClientTestModule";

        public override void P1Handler(S1Reader request, S1Writer response)
        {
            Console.WriteLine($"P1Handler reached: {request.foo}, {request.bar}");
        }
    }
}
