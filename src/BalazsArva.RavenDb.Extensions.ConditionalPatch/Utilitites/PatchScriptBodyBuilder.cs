using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchScriptBodyBuilder : IPatchScriptBodyBuilder
    {
        private const string DocumentParameterName = "this";

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public PatchScriptBodyBuilder(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public string CreateScriptBody(PropertyUpdateDescriptor[] propertyUpdates, ScriptParameterDictionary parameters)
        {
            var assignmentScripts = new List<string>(propertyUpdates.Length);

            foreach (var propertyUpdate in propertyUpdates)
            {
                var transformedMemberSelector = GetNormalizedExpression(propertyUpdate.MemberSelector);

                var assignmentExpression = Expression.Assign(
                    transformedMemberSelector,
                    Expression.Constant(propertyUpdate.NewValue));

                assignmentScripts.Add(_expressionProcessorPipeline.ProcessExpression(assignmentExpression, parameters));
            }

            return string.Join(
                "\n",
                assignmentScripts.Select(script => $"\t{script};"));
        }

        private Expression GetNormalizedExpression(MemberExpression memberExpression)
        {
            if (memberExpression.Expression is ParameterExpression parameterExpression)
            {
                return memberExpression.Update(Expression.Parameter(parameterExpression.Type, DocumentParameterName));
            }
            else if (memberExpression.Expression is MemberExpression parentMemberExpression)
            {
                return memberExpression.Update(GetNormalizedExpression(parentMemberExpression));
            }

            throw new NotSupportedException("Only MemberExpression and ParameterExpression types are allowed in the property update selector expression tree.");
        }
    }
}