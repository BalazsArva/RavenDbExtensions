using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class MemberExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is MemberExpression memberExpression && ExpressionHelper.IsRuntimeObjectBoundExpression(memberExpression))
            {
                result = Expression.Constant(RuntimeExpressionValueResolver.GetValue(memberExpression));

                return true;
            }

            result = default;

            return false;
        }
    }
}