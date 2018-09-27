using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Client.TestProtocols.Impl
{
    public class TrinityClientTestModule : TrinityClientTestModuleBase
    {
        public override string GetModuleName() => "TrinityClientTestModule";

        public override void P1Handler(S1Reader request, S1Writer response)
        {
            Console.WriteLine($"P1Handler reached: {request.foo}, {request.bar}");
            Console.WriteLine(Global.CloudStorage.LoadC1(request.bar));
            Global.CloudStorage.SaveC1(request.bar + 1, request.foo, request.bar);

            response.foo = request.foo;
            response.bar = request.bar + 1;
        }
    }
}
