using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchScriptBuilder : IPatchScriptBuilder
    {
        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public PatchScriptBuilder(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters)
        {
            var parsedConditionExpression = _expressionProcessorPipeline.ProcessExpression(condition, parameters);
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
    }
}