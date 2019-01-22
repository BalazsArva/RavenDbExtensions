using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Visitors;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchScriptConditionBuilder : IPatchScriptConditionBuilder
    {
        private const string DocumentParameterName = "this";

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public PatchScriptConditionBuilder(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public string CreateScriptCondition<TDocument>(Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var normalizedConditionBody = GetNormalizedConditionBody(condition);

            return _expressionProcessorPipeline.ProcessExpression(normalizedConditionBody, parameters);
        }

        private Expression GetNormalizedConditionBody<TDocument>(Expression<Func<TDocument, bool>> condition)
        {
            var visitor = new ParameterRenamerExpressionVisitor(DocumentParameterName, condition.Parameters[0]);
            var transformedExpression = (Expression<Func<TDocument, bool>>)visitor.Visit(condition);

            return transformedExpression.Body;
        }
    }
}