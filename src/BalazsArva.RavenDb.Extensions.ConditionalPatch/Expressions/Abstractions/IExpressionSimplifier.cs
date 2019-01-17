using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionSimplifier
    {
        bool TrySimplifyExpression(Expression expression, out Expression result);
    }
}