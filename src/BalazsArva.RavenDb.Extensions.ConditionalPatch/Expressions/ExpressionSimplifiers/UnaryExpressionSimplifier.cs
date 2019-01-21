using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class UnaryExpressionSimplifier : IExpressionSimplifier
    {
        private static readonly Type objectType = typeof(object);

        private readonly IExpressionSimplifierPipeline _expressionSimplifierPipeline;

        public UnaryExpressionSimplifier(IExpressionSimplifierPipeline expressionSimplifierPipeline)
        {
            _expressionSimplifierPipeline = expressionSimplifierPipeline ?? throw new ArgumentNullException(nameof(expressionSimplifierPipeline));
        }

        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is UnaryExpression unaryExpression)
            {
                var simplifiedOperand = _expressionSimplifierPipeline.ProcessExpression(unaryExpression.Operand);

                // Runtime-resolvable value
                if (simplifiedOperand.NodeType == ExpressionType.Constant)
                {
                    Expression convertExpression;

                    // TODO: Write test for this case!
                    // If we encounter a conversion, we need to perform the original conversion first before forcing every type into Object.
                    // If we did something like "10 (int) < 10 (long)" we would get an error without this when simplifying the binary operation
                    // because the compiler generates a conversion because of the operator but here we would discard it if we didn't include the
                    // Expression.Convert(..., unaryExpression.Type) expression.
                    if (unaryExpression.NodeType == ExpressionType.Convert || unaryExpression.NodeType == ExpressionType.ConvertChecked)
                    {
                        convertExpression = Expression.Convert(
                            Expression.Convert(simplifiedOperand, unaryExpression.Type),
                            objectType);
                    }
                    else
                    {
                        convertExpression = Expression.Convert(simplifiedOperand, objectType);
                    }

                    var valueProviderLambdaExpression = Expression.Lambda<Func<object>>(convertExpression);

                    var valueProvider = valueProviderLambdaExpression.Compile();
                    var resolvedValue = valueProvider();

                    result = Expression.Constant(resolvedValue);

                    return true;
                }

                var updatedUnaryExpression = unaryExpression.Update(simplifiedOperand);
                if (updatedUnaryExpression != unaryExpression)
                {
                    result = updatedUnaryExpression;

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}