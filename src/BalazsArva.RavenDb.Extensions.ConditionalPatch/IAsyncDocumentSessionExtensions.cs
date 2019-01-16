using System;
using System.Linq.Expressions;
using Raven.Client.Documents.Session;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static void PatchIf<TDocument>(this IAsyncDocumentSession session, string id, PropertyUpdateBatch<TDocument> updates, Expression<Func<TDocument, bool>> condition)
        {
        }
    }
}