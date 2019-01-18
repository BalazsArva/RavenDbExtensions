using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ParameterExpressionProcessor : IExpressionProcessor
    {
        private const string ParameterReference = "this";

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

            // TODO: Consider cases when there can be multiple parameters
            if (expression is ParameterExpression)
            {
                result = ParameterReference;

                return true;
            }

            result = default;

            return false;
        }
    }
}