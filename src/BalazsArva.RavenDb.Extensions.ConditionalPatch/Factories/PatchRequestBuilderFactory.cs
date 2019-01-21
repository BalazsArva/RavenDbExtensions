using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class PatchRequestBuilderFactory
    {
        public static IPatchRequestBuilder CreatePatchRequestBuilder()
        {
            return new PatchRequestBuilder(PatchScriptBuilderFactory.CreatePatchScriptBuilder());
        }
    }
}