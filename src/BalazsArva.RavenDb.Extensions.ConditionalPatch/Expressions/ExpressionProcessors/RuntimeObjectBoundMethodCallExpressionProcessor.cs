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
                var expressionValue = RuntimeExpressionValueResolver.GetValue(methodCallExpression);

                result = ConstantValueConverter.ConvertToJson(expressionValue);

                return true;
            }

            result = default;

            return false;
        }
    }
}