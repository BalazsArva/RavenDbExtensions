using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionProcessorPipeline
    {
        string ProcessExpression(Expression expression, ScriptParameterDictionary parameters);
    }
}