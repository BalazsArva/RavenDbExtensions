using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class LambdaExpressionProcessor : IExpressionProcessor
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

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression == null)
            {
                result = default;

                return false;
            }

            // TODO: Review this
            var expressionParameters = lambdaExpression.Parameters;
            var body = lambdaExpression.Body;

            result = ExpressionParser.CreateJsScriptFromExpression(body, parameters);

            return true;
        }
    }
}