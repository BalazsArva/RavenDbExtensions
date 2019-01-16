using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public static class PatchScriptBuilder
    {
        public static string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters)
        {
            var parsedConditionExpression = ExpressionParser.CreateJsScriptFromExpression(condition, parameters);
            var assignmentScripts = new List<string>(propertyUpdates.Length);

            foreach (var propertyUpdate in propertyUpdates)
            {
                var assignmentExpression = Expression.Assign(
                    propertyUpdate.MemberSelector,
                    Expression.Constant(propertyUpdate.NewValue));

                assignmentScripts.Add(ExpressionParser.CreateJsScriptFromExpression(assignmentExpression, parameters));
            }

            var scriptCondition = $"if ({parsedConditionExpression})";
            var scriptBody = string.Join(
                "\n",
                assignmentScripts.Select(script => $"\t{script};"));

            return string.Join("\n", scriptCondition, "{", scriptBody, "}");
        }
    }
}