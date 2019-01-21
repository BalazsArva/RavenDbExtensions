using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories
{
    public static class ExpressionProcessorPipelineFactory
    {
        public static IExpressionProcessorPipeline CreateExpressionProcessorPipeline()
        {
            return new ExpressionProcessorPipeline(ExpressionSimplifierPipelineFactory.CreateExpressionSimplifierPipeline());
        }
    }
}