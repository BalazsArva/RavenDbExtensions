using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionSimplifierPipeline
    {
        Expression ProcessExpression(Expression expression);
    }
}