using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class ExpressionSimplifierPipelineFactory
    {
        public static IExpressionSimplifierPipeline CreateExpressionSimplifierPipeline()
        {
            return new ExpressionSimplifierPipeline();
        }
    }
}