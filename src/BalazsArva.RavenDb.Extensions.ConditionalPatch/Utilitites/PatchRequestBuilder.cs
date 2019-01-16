using System;
using System.Linq.Expressions;
using Raven.Client.Documents.Operations;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public static class PatchRequestBuilder
    {
        public static PatchRequest CreatePatchRequest<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition)
        {
            var parameters = new ScriptParameterDictionary();
            var patchRequest = new PatchRequest
            {
                Values = parameters,
                Script = PatchScriptBuilder.CreateConditionalPatchScript(propertyUpdates, condition, parameters)
            };

            return patchRequest;
        }
    }
}