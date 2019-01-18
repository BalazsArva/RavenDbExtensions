using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ConstantExpressionProcessor : IExpressionProcessor
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

            if (expression is ConstantExpression constantExpression)
            {
                var parameterKey = parameters.AddNext(constantExpression.Value);

                result = $"args.{parameterKey}";

                return true;
            }

            result = default;

            return false;
        }
    }
}