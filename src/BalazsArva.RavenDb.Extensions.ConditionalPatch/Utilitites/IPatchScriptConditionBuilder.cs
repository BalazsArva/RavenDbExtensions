using System;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public interface IPatchScriptConditionBuilder
    {
        string CreateScriptCondition<TDocument>(Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters);
    }
}