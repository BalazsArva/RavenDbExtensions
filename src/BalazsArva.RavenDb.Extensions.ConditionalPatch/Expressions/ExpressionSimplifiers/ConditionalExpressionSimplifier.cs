using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class ConditionalExpressionSimplifier : IExpressionSimplifier
    {
        private readonly IExpressionSimplifierPipeline _expressionSimplifierPipeline;

        public ConditionalExpressionSimplifier(IExpressionSimplifierPipeline expressionSimplifierPipeline)
        {
            _expressionSimplifierPipeline = expressionSimplifierPipeline ?? throw new ArgumentNullException(nameof(expressionSimplifierPipeline));
        }

        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is ConditionalExpression conditionalExpression)
            {
                var simplifiedTestExpression = _expressionSimplifierPipeline.ProcessExpression(conditionalExpression.Test);
                var simplifiedIfTrueExpression = _expressionSimplifierPipeline.ProcessExpression(conditionalExpression.IfTrue);
                var simplifiedIfFalseExpression = _expressionSimplifierPipeline.ProcessExpression(conditionalExpression.IfFalse);

                // If the condition and both of the branches could be evaluated, then evaluate and return the result as a constant expression.
                if (simplifiedTestExpression is ConstantExpression constantTestExpression &&
                    simplifiedIfTrueExpression is ConstantExpression constantIfTrueExpression &&
                    simplifiedIfFalseExpression is ConstantExpression constantIfFalseExpression)
                {
                    var evaluationResult = (bool)constantTestExpression.Value
                        ? constantIfTrueExpression.Value
                        : constantIfFalseExpression.Value;

                    result = Expression.Constant(evaluationResult);

                    return true;
                }

                // If the True and False branches resolve to the same value, replace the ?: expression with a constant expression with the value of either side.
                if (simplifiedIfTrueExpression is ConstantExpression ifTrueExpression &&
                    simplifiedIfFalseExpression is ConstantExpression ifFalseExpression &&
                    ifTrueExpression.Value.Equals(ifFalseExpression.Value))
                {
                    result = Expression.Constant(ifTrueExpression.Value);

                    return true;
                }

                // The condition is runtime resolvable - evaluate it and return the appropriate "branch"
                if (simplifiedTestExpression is ConstantExpression constantTestExpression2)
                {
                    result = (bool)constantTestExpression2.Value
                        ? simplifiedIfTrueExpression
                        : simplifiedIfFalseExpression;

                    return true;
                }

                var updatedConditionalExpression = conditionalExpression.Update(simplifiedTestExpression, simplifiedIfTrueExpression, simplifiedIfFalseExpression);
                if (updatedConditionalExpression != conditionalExpression)
                {
                    result = updatedConditionalExpression;

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}