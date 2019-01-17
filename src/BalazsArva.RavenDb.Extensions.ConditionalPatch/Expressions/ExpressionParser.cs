using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class ExpressionParser
    {
        public static string CreateJsScriptFromExpression(Expression expression, ScriptParameterDictionary parameters)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return ExpressionProcessorPipeline.ProcessExpression(
                ExpressionSimplifierPipeline.ProcessExpression(expression),
                parameters);
        }
    }
}