using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class RuntimeObjectBoundMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly Type StringType = typeof(string);

        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            if (!(expression is MethodCallExpression methodCallExpression) || !ExpressionHelper.IsRuntimeObjectBoundExpression(methodCallExpression))
            {
                result = default;

                return false;
            }

            var expressionValue = RuntimeExpressionValueResolver.GetValue(methodCallExpression);
            var parameterKey = parameters.AddNext(expressionValue);

            result = $"args.{parameterKey}";

            return true;
        }
    }
}