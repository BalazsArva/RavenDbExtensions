using System;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public interface IPatchScriptBuilder
    {
        string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters);
    }
}