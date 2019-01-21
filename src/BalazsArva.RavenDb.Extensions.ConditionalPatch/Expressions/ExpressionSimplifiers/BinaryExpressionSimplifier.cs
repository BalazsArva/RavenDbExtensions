using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class BinaryExpressionSimplifier : IExpressionSimplifier
    {
        private readonly IExpressionSimplifierPipeline _expressionSimplifierPipeline;

        public BinaryExpressionSimplifier(IExpressionSimplifierPipeline expressionSimplifierPipeline)
        {
            _expressionSimplifierPipeline = expressionSimplifierPipeline ?? throw new ArgumentNullException(nameof(expressionSimplifierPipeline));
        }

        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                var simplifiedLeftExpression = _expressionSimplifierPipeline.ProcessExpression(binaryExpression.Left);
                var simplifiedRightExpression = _expressionSimplifierPipeline.ProcessExpression(binaryExpression.Right);

                // Both operands could be resolved to a runtime value, perform the binary operation between them.
                if (simplifiedLeftExpression.NodeType == ExpressionType.Constant &&
                    simplifiedRightExpression.NodeType == ExpressionType.Constant)
                {
                    var convertExpression = Expression.Convert(binaryExpression, typeof(object));
                    var lambdaExpression = Expression.Lambda<Func<object>>(convertExpression);

                    var compiledLambdaExpression = lambdaExpression.Compile();
                    var value = compiledLambdaExpression();

                    result = Expression.Constant(value);

                    return true;
                }

                // One (or both) side could be simplified, but one (or both) of them is parameter-bound.
                // In this case, cannot evaluate to a runtime value, but can return a simpler expression.
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