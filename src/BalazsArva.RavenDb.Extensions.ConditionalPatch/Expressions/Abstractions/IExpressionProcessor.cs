using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionProcessor
    {
        bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result);
    }
}