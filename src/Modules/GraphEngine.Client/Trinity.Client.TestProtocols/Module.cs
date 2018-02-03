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

        public override void P1Handler(P1RequestReader request)
        {
            Console.WriteLine($"P1Handler reached: {request.foo}, {request.bar}");
        }
    }
}
