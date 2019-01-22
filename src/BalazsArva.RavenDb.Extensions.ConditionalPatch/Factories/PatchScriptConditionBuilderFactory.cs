using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class PatchScriptConditionBuilderFactory
    {
        public static IPatchScriptConditionBuilder CreatePatchScriptBodyBuilder()
        {
            return new PatchScriptConditionBuilder(ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline());
        }
    }
}