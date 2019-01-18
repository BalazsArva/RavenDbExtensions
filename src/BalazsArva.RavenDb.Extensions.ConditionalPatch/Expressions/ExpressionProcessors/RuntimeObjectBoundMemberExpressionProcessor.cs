using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class RuntimeObjectBoundMemberExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!(expression is MemberExpression memberExpression) || !ExpressionHelper.IsRuntimeObjectBoundExpression(memberExpression))
            {
                result = default;

                return false;
            }

            var expressionValue = RuntimeExpressionValueResolver.GetValue(memberExpression);
            var parameterKey = parameters.AddNext(expressionValue);

            result = $"args.{parameterKey}";

            return true;
        }
    }
}