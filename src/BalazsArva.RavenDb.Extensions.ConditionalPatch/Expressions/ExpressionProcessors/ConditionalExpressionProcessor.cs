using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ConditionalExpressionProcessor : IExpressionProcessor
    {
        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public ConditionalExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
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

            if (expression is ConditionalExpression conditionalExpression)
            {
                var test = _expressionProcessorPipeline.ProcessExpression(conditionalExpression.Test, parameters);
                var ifTrue = _expressionProcessorPipeline.ProcessExpression(conditionalExpression.IfTrue, parameters);
                var ifFalse = _expressionProcessorPipeline.ProcessExpression(conditionalExpression.IfFalse, parameters);

                result = $"({test} ? {ifTrue} : {ifFalse})";

                return true;
            }

            result = default;

            return false;
        }
    }
}