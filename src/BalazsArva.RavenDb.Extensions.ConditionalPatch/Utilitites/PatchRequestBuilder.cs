using System;
using System.Linq.Expressions;
using Raven.Client.Documents.Operations;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchRequestBuilder : IPatchRequestBuilder
    {
        private readonly IPatchScriptBuilder _patchScriptBuilder;

        public PatchRequestBuilder(IPatchScriptBuilder patchScriptBuilder)
        {
            _patchScriptBuilder = patchScriptBuilder ?? throw new ArgumentNullException(nameof(patchScriptBuilder));
        }

        public PatchRequest CreatePatchRequest<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition)
        {
            var parameters = new ScriptParameterDictionary();
            var patchRequest = new PatchRequest
            {
                Values = parameters,
                Script = _patchScriptBuilder.CreateConditionalPatchScript(propertyUpdates, condition, parameters)
            };

            return patchRequest;
        }
    }
}