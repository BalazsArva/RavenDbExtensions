using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class ConditionalExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is ConditionalExpression conditionalExpression)
            {
                var simplifiedTestExpression = ExpressionSimplifier.SimplifyExpression(conditionalExpression.Test);
                var simplifiedIfTrueExpression = ExpressionSimplifier.SimplifyExpression(conditionalExpression.IfTrue);
                var simplifiedIfFalseExpression = ExpressionSimplifier.SimplifyExpression(conditionalExpression.IfFalse);

                // If the condition and both of the branches can be evaluated, then evaluate and return the result as a constant expression.
                if (ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedTestExpression) &&
                    ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedIfTrueExpression) &&
                    ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedIfFalseExpression))
                {
                    // Wrap the expression in a lambda, compile and call it to get the evaluated result.
                    var convertExpression = Expression.Convert(conditionalExpression, typeof(object));
                    var lambdaExpression = Expression.Lambda<Func<object>>(convertExpression);

                    var compiledLambdaExpression = lambdaExpression.Compile();
                    var value = compiledLambdaExpression();

                    result = Expression.Constant(value);

                    return true;
                }

                // If the True and False branches resolve to the same value, replace the ?: expression with a constant expression with the value of either side.
                if (simplifiedIfTrueExpression is ConstantExpression ifTrueExpression &&
                    simplifiedIfFalseExpression is ConstantExpression ifFalseExpression &&
                    Equals(ifTrueExpression.Value, ifFalseExpression.Value))
                {
                    result = Expression.Constant(ifTrueExpression.Value);

                    return true;
                }

                // The condition is runtime resolvable - evaluate it and return the appropriate "branch"
                if (simplifiedTestExpression is ConstantExpression constantTestExpression)
                {
                    var conditionValue = (bool)constantTestExpression.Value;

                    result = conditionValue
                        ? simplifiedIfTrueExpression
                        : simplifiedIfFalseExpression;

                    return true;
                }

                if (simplifiedTestExpression != conditionalExpression.Test ||
                    simplifiedIfTrueExpression != conditionalExpression.IfTrue ||
                    simplifiedIfFalseExpression != conditionalExpression.IfFalse)
                {
                    result = conditionalExpression.Update(simplifiedTestExpression, simplifiedIfTrueExpression, simplifiedIfFalseExpression);

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}