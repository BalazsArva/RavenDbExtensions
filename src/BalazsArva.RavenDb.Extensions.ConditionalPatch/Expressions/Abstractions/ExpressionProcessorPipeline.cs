using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public static class ExpressionProcessorPipeline
    {
        private static readonly IEnumerable<IExpressionProcessor> expressionProcessors;

        static ExpressionProcessorPipeline()
        {
            expressionProcessors = new List<IExpressionProcessor>
            {
                new ConstantExpressionProcessor(),
                new LambdaExpressionProcessor(),
                new BinaryExpressionProcessor(),
                new UnaryExpressionProcessor(),
                new ConditionalExpressionProcessor(),
                new ParameterExpressionProcessor(),
                new ParameterBoundMemberExpressionProcessor(),
                new ParameterBoundMethodCallExpressionProcessor()
            };
        }

        public static string ProcessExpression(Expression expression, ScriptParameterDictionary parameters)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            foreach (var processor in expressionProcessors)
            {
                if (processor.TryProcess(expression, parameters, out var result))
                {
                    return result;
                }
            }

            throw new NotSupportedException($"Cannot handle expression of type {expression.GetType().FullName}.");
        }
    }
}