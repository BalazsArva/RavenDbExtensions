using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public static class PatchScriptBuilder
    {
        public static string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, IDictionary<string, object> parameters)
        {
            var scriptCondition = $"if ()";
            var scriptBody = "";

            return string.Join("\n", scriptCondition, "{", scriptBody, "}");
        }
    }
}