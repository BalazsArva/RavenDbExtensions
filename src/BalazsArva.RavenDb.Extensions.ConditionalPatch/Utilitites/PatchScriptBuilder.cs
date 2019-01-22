using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Visitors;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchScriptBuilder : IPatchScriptBuilder
    {
        private const string DocumentParameterName = "this";

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public PatchScriptBuilder(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters)
        {
            var normalizedCondition = NormalizeCondition(condition);

            var parsedConditionExpression = _expressionProcessorPipeline.ProcessExpression(normalizedCondition, parameters);
            var assignmentScripts = new List<string>(propertyUpdates.Length);

            foreach (var propertyUpdate in propertyUpdates)
            {
                var assignmentExpression = Expression.Assign(
                    propertyUpdate.MemberSelector,
                    Expression.Constant(propertyUpdate.NewValue));

                assignmentScripts.Add(_expressionProcessorPipeline.ProcessExpression(assignmentExpression, parameters));
            }

            var scriptCondition = $"if ({parsedConditionExpression})";
            var scriptBody = string.Join(
                "\n",
                assignmentScripts.Select(script => $"\t{script};"));

            return string.Join("\n", scriptCondition, "{", scriptBody, "}");
        }

        private Expression<Func<TDocument, bool>> NormalizeCondition<TDocument>(Expression<Func<TDocument, bool>> condition)
        {
            condition = condition ?? (doc => true);

            var visitor = new ParameterRenamerExpressionVisitor(DocumentParameterName, condition.Parameters[0]);
            var transformedExpression = (Expression<Func<TDocument, bool>>)visitor.Visit(condition);

            return transformedExpression;
        }
    }
}