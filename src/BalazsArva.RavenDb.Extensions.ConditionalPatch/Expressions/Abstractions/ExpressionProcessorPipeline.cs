using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public class ExpressionProcessorPipeline : IExpressionProcessorPipeline
    {
        private readonly IEnumerable<IExpressionProcessor> _expressionProcessors;
        private readonly IExpressionSimplifierPipeline _expressionSimplifierPipeline;

        public ExpressionProcessorPipeline(IExpressionSimplifierPipeline expressionSimplifierPipeline)
        {
            _expressionSimplifierPipeline = expressionSimplifierPipeline ?? throw new ArgumentNullException(nameof(expressionSimplifierPipeline));
            _expressionProcessors = new List<IExpressionProcessor>
            {
                new ConstantExpressionProcessor(),
                new LambdaExpressionProcessor(this),
                new BinaryExpressionProcessor(this),
                new UnaryExpressionProcessor(this),
                new ConditionalExpressionProcessor(this),
                new ParameterExpressionProcessor(),
                new MemberExpressionProcessor(this),
                new MethodCallExpressionProcessor(this)
            };
        }

        public string ProcessExpression(Expression expression, ScriptParameterDictionary parameters)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var simplifiedExpression = _expressionSimplifierPipeline.ProcessExpression(expression);
            foreach (var processor in _expressionProcessors)
            {
                if (processor.TryProcess(simplifiedExpression, parameters, out var result))
                {
                    return result;
                }
            }

            throw new NotSupportedException($"Cannot handle expression of type {expression.GetType().FullName}.");
        }
    }
}