using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionProcessor
    {
        bool TryProcess(Expression expression, out string result);
    }
}