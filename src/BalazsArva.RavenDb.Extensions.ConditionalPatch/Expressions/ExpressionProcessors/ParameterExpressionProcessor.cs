using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ParameterExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            // TODO: Consider cases when there can be multiple parameters
            if (expression is ParameterExpression)
            {
                result = "this";

                return true;
            }

            result = default;

            return false;
        }
    }
}