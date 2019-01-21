using System;
using System.Linq.Expressions;
using Raven.Client.Documents.Operations;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public interface IPatchRequestBuilder
    {
        PatchRequest CreatePatchRequest<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition);
    }
}