using System;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class LambdaExpressionProcessor : IExpressionProcessor
    {
        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public LambdaExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

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

            var parameterList = string.Join(", ", lambdaExpression.Parameters.Select(p => p.Name));
            var body = _expressionProcessorPipeline.ProcessExpression(lambdaExpression.Body, parameters);

            // TODO: Consider whether "return" is always correct (technically we can return void stuff in JS but what if
            result = $"function({parameterList}) {{ return {body}; }}";

            return true;
        }
    }
}