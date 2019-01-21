using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class PatchScriptBuilderFactory
    {
        public static IPatchScriptBuilder CreatePatchScriptBuilder()
        {
            return new PatchScriptBuilder(ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline());
        }
    }
}