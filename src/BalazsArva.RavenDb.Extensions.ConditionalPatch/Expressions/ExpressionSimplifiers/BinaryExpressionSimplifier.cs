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
                    if (TryHandleBinaryOperationBetweenNullableAndNonNullableOperands(binaryExpression, simplifiedLeftExpression, simplifiedRightExpression, out result))
                    {
                        return true;
                    }

                    result = Expression.MakeBinary(binaryExpression.NodeType, simplifiedLeftExpression, simplifiedRightExpression);
                    return true;
                }
            }

            result = default;
            return false;
        }

        private bool TryHandleBinaryOperationBetweenNullableAndNonNullableOperands(BinaryExpression binaryExpression, Expression simplifiedLeftExpression, Expression simplifiedRightExpression, out Expression result)
        {
            // We need to handle binary operations between Nullable and non-Nullable types specially for two reasons:
            // - Without it, in the main TrySimplifyExpression, the regular Expression.MakeBinary(...) call would construct the expression using the simplified operands (if they could be simplified). But if we
            //   do something like "doc => doc.SomeNullableLong > 0" then when wrapping the 0 into an Expression.Constant that call will automatically resolve the nullable value to its non-nullable counterpart
            //   since it is not null. From then on, the binary operation happens between a nullable and a non-nullable type and the binary operation might not be defined for such arguments (e.g. > is not defined
            //   for long? and long).
            // - Because we need to do the binary operation using C# semantics. In C#, "((int?)null) < ((int?)1)" is false, but in JS, null < 1 is true.
            var leftValueType = simplifiedLeftExpression.Type;
            if (leftValueType.IsGenericType)
            {
                leftValueType = leftValueType.GetGenericTypeDefinition();
            }

            var rightValueType = simplifiedRightExpression.Type;
            if (rightValueType.IsGenericType)
            {
                rightValueType = rightValueType.GetGenericTypeDefinition();
            }

            if (leftValueType == typeof(Nullable<>) && rightValueType != typeof(Nullable<>))
            {
                // We want something like:
                // this.NullableProp.HasValue ? (this.NullableProp.Value {operator} simplifiedRightExpression) as BinaryExpression.ResultType : null
                var conditionExpression = Expression.NotEqual(simplifiedLeftExpression, Expression.Constant(null));

                var nullableGetValueExpression = Expression.MakeMemberAccess(simplifiedLeftExpression, simplifiedLeftExpression.Type.GetProperty("Value"));
                var ifTrueExpression = Expression.Convert(
                    Expression.MakeBinary(
                        binaryExpression.NodeType,
                        nullableGetValueExpression,
                        simplifiedRightExpression),
                    binaryExpression.Type);

                var ifFalseExpression = Expression.Convert(Expression.Constant(binaryExpression.Type == typeof(bool) ? (object)false : null), binaryExpression.Type);

                var ternaryExpression = Expression.Condition(
                    conditionExpression,
                    ifTrueExpression,
                    ifFalseExpression);

                result = ternaryExpression;
                return true;
            }

            if (leftValueType != typeof(Nullable<>) && rightValueType == typeof(Nullable<>))
            {
                // We want something like:
                // this.NullableProp.HasValue ? (this.NullableProp.Value {operator} simplifiedRightExpression) as BinaryExpression.ResultType : null
                var conditionExpression = Expression.NotEqual(simplifiedRightExpression, Expression.Constant(null));

                var nullableGetValueExpression = Expression.MakeMemberAccess(simplifiedRightExpression, simplifiedRightExpression.Type.GetProperty("Value"));
                var ifTrueExpression = Expression.Convert(
                    Expression.MakeBinary(
                        binaryExpression.NodeType,
                        simplifiedLeftExpression,
                        nullableGetValueExpression),
                    binaryExpression.Type);

                var ifFalseExpression = Expression.Convert(Expression.Constant(binaryExpression.Type == typeof(bool) ? (object)false : null), binaryExpression.Type);

                var ternaryExpression = Expression.Condition(
                    conditionExpression,
                    ifTrueExpression,
                    ifFalseExpression);

                result = ternaryExpression;
                return true;
            }

            result = default;
            return false;
        }
    }
}