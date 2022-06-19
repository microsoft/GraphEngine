using FanoutSearch;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System;
using Trinity.Storage;

namespace FanoutSearch
{
    class JsonExpressionSerializer : IExpressionSerializer
    {
        private JsonSerializer m_serializer = null;
        private NodeFactory m_factory = null;

        public JsonExpressionSerializer()
        {
            m_serializer = new JsonSerializer();
            m_serializer.AddKnownType(typeof(FanoutSearch.Action));
            m_factory = new NodeFactory(new FactorySettings { });
        }

        public string Serialize(System.Linq.Expressions.Expression pred)
        {
            return pred.ToJson(m_factory, m_serializer);
        }

        public Func<ICellAccessor, FanoutSearch.Action> DeserializeTraverseAction(string pred)
        {
            var func_exp = m_serializer.Deserialize<LambdaExpressionNode>(pred).ToExpression<Func<ICellAccessor, FanoutSearch.Action>>();
            return func_exp.Compile();
        }

        public Func<ICellAccessor, bool> DeserializeOriginPredicate(string pred)
        {
            var func_exp = m_serializer.Deserialize<LambdaExpressionNode>(pred).ToExpression<Func<ICellAccessor, bool>>();
            return func_exp.Compile();
        }

    }
}
