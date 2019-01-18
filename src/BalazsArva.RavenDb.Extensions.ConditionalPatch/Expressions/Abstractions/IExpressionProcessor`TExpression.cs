using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public interface IExpressionProcessor<TExpression>
        where TExpression : Expression
    {
        bool TryProcess(TExpression expression, ScriptParameterDictionary parameters, out string result);
    }
}