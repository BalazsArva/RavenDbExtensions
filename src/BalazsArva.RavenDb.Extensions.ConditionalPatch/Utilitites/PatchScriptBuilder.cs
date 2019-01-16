using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public static class PatchScriptBuilder
    {
        public static string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, IDictionary<string, object> parameters)
        {
            var parsedConditionExpression = ExpressionParser.CreateJsScriptFromExpression(condition);
            var assignmentScripts = new List<string>(propertyUpdates.Length);

            foreach (var propertyUpdate in propertyUpdates)
            {
                var assignmentExpression = Expression.Assign(
                    propertyUpdate.MemberSelector,
                    Expression.Constant(propertyUpdate.NewValue));

                assignmentScripts.Add(ExpressionParser.CreateJsScriptFromExpression(assignmentExpression));
            }

            var scriptCondition = $"if ({parsedConditionExpression})";
            var scriptBody = string.Join(
                "\n",
                assignmentScripts.Select(script => $"\t{script};"));

            return string.Join("\n", scriptCondition, "{", scriptBody, "}");
        }
    }
}