using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class PatchScriptBodyBuilderFactory
    {
        public static IPatchScriptBodyBuilder CreatePatchScriptBodyBuilder()
        {
            return new PatchScriptBodyBuilder(ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline());
        }
    }
}