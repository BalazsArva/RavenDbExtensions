using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Session;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static void PatchIf<TDocument>(this IAsyncDocumentSession session, string id, PropertyUpdateBatch<TDocument> updates, Expression<Func<TDocument, bool>> condition)
        {
            var batch = updates.CreateBatch();

            var patchRequest = PatchRequestBuilder.CreatePatchRequest(batch, condition);
            var patchCommandData = new PatchCommandData(id, null, patchRequest, null);

            session.Advanced.Defer(new ICommandData[] { patchCommandData });
        }
    }
}