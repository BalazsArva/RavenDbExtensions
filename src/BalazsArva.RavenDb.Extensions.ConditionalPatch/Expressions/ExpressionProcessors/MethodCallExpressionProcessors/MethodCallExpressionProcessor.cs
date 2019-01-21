using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class MethodCallExpressionProcessor : IExpressionProcessor
    {
        private readonly IEnumerable<IExpressionProcessor<MethodCallExpression>> expressionProcessors;

        public MethodCallExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            if (expressionProcessorPipeline == null)
            {
                throw new ArgumentNullException(nameof(expressionProcessorPipeline));
            }

            // TODO: Consider LINQ extension methods as well!
            expressionProcessors = new List<IExpressionProcessor<MethodCallExpression>>
            {
                new ObjectMethodCallExpressionProcessor(expressionProcessorPipeline),
                new IntegralTypesMethodCallExpressionProcessor(expressionProcessorPipeline),
                new StringMethodCallExpressionProcessor(expressionProcessorPipeline)
            };
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

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
            {
                result = default;
                return false;
            }

            foreach (var processor in expressionProcessors)
            {
                if (processor.TryProcess(methodCallExpression, parameters, out result))
                {
                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}