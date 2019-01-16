using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class RuntimeObjectBoundMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly Type StringType = typeof(string);

        public bool TryProcess(Expression expression, out string result)
        {
            if (!(expression is MethodCallExpression methodCallExpression))
            {
                result = default;

                return false;
            }

            if (ExpressionHelper.IsRuntimeObjectBoundExpression(methodCallExpression))
            {
                var value = RuntimeExpressionValueResolver.GetValue(methodCallExpression);

                // There is a logic in converting common types to JS representations in the constant expression processor. So create a dummy constant expression to convert the value.
                // TODO: Refactor that logic elsewhere so we don't have to create a dummy constant expression.
                result = ExpressionProcessorPipeline.GetScriptFromConditionExpression(Expression.Constant(value));

                return true;
            }

            result = default;

            return false;
        }
    }
}