using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors.Linq
{
    public class LinqExtensionMethodCallExpressionProcessor : IExpressionProcessor<MethodCallExpression>
    {
        private readonly IEnumerable<IExpressionProcessor<MethodCallExpression>> expressionProcessors;

        public LinqExtensionMethodCallExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            if (expressionProcessorPipeline == null)
            {
                throw new ArgumentNullException(nameof(expressionProcessorPipeline));
            }

            expressionProcessors = new List<IExpressionProcessor<MethodCallExpression>>
            {
                new LinqAnyExtensionMethodCallExpressionProcessor(expressionProcessorPipeline)
            };
        }

        public bool TryProcess(MethodCallExpression methodCallExpression, ScriptParameterDictionary parameters, out string result)
        {
            if (methodCallExpression == null)
            {
                throw new ArgumentNullException(nameof(methodCallExpression));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (methodCallExpression.Method.DeclaringType != typeof(Enumerable))
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