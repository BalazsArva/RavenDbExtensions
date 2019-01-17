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

                if (ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedTestExpression) &&
                    ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedIfTrueExpression) &&
                    ExpressionHelper.IsRuntimeObjectBoundExpression(simplifiedIfFalseExpression))
                {
                    var convertExpression = Expression.Convert(conditionalExpression, typeof(object));
                    var lambdaExpression = Expression.Lambda<Func<object>>(convertExpression);

                    var compiledLambdaExpression = lambdaExpression.Compile();
                    var value = compiledLambdaExpression();

                    result = Expression.Constant(value);

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