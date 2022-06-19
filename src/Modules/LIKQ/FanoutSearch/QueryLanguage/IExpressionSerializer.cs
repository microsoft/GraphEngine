using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    public interface IExpressionSerializer
    {
        string Serialize(System.Linq.Expressions.Expression pred);

        Func<ICellAccessor, Action> DeserializeTraverseAction(string pred);

        Func<ICellAccessor, bool> DeserializeOriginPredicate(string pred);
    }
}
