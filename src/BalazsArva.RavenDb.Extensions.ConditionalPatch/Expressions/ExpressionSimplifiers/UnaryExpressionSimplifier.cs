using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class UnaryExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is UnaryExpression unaryExpression && ExpressionHelper.IsRuntimeObjectBoundExpression(unaryExpression))
            {
                result = Expression.Constant(RuntimeExpressionValueResolver.GetValue(unaryExpression));

                return true;
            }

            result = default;

            return false;
        }
    }
}