using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors;

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
                new RuntimeObjectBoundMemberExpressionProcessor(),
                new RuntimeObjectBoundMethodCallExpressionProcessor(),
                new ParameterBoundMethodCallExpressionProcessor()
            };
        }

        public static string GetScriptFromConditionExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            foreach (var processor in expressionProcessors)
            {
                if (processor.TryProcess(expression, out var result))
                {
                    return result;
                }
            }

            throw new NotSupportedException($"Cannot handle expression of type {expression.GetType().FullName}.");
        }
    }
}