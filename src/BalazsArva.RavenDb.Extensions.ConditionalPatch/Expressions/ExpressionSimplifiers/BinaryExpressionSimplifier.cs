using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class BinaryExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                var simplifiedLeftExpression = ExpressionSimplifier.SimplifyExpression(binaryExpression.Left);
                var simplifiedRightExpression = ExpressionSimplifier.SimplifyExpression(binaryExpression.Right);

                if (ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedLeftExpression) &&
                    ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedRightExpression))
                {
                    var convertExpression = Expression.Convert(binaryExpression, typeof(object));
                    var lambdaExpression = Expression.Lambda<Func<object>>(convertExpression);

                    var compiledLambdaExpression = lambdaExpression.Compile();
                    var value = compiledLambdaExpression();

                    result = Expression.Constant(value);

                    return true;
                }

                // Only one of the two sides could be simplified - the expression as a whole still changed (in this case, the other side is parameter-bound).
                if (simplifiedLeftExpression != binaryExpression.Left || simplifiedRightExpression != binaryExpression.Right)
                {
                    result = Expression.MakeBinary(binaryExpression.NodeType, simplifiedLeftExpression, simplifiedRightExpression);

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}