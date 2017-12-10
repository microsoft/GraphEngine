using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    class ExpressionSerializer
    {
        [ThreadStatic]
        private static IExpressionSerializer s_serializer;
        private static Func<IExpressionSerializer> s_serializer_factory;

        private static void EnsureSerializer()
        {
            if (s_serializer == null)
            {
                s_serializer = s_serializer_factory();
            }
        }

        internal static string Serialize(System.Linq.Expressions.Expression pred)
        {
            EnsureSerializer();
            return s_serializer.Serialize(pred);
        }

        internal static Func<ICellAccessor, Action> DeserializeTraverseAction(string pred)
        {
            EnsureSerializer();
            return s_serializer.DeserializeTraverseAction(pred);
        }

        internal static Func<ICellAccessor, bool> DeserializeOriginPredicate(string pred)
        {
            EnsureSerializer();
            return s_serializer.DeserializeOriginPredicate(pred);
        }

        internal static void SetSerializerFactory(Func<IExpressionSerializer> func)
        {
            s_serializer_factory = func;
        }
    }
}
