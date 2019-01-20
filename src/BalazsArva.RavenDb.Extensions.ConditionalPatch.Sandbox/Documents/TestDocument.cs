using System.Collections.Generic;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox.Documents
{
    public class TestDocument
    {
        public string Id { get; set; }

        public long LastKnownChangeId { get; set; }

        public long? NullableLong { get; set; }

        public long[] RecordedChangeIds { get; set; }

        public List<string> ReferenceIds { get; set; }

        public HashSet<string> InvolvedUsers { get; set; }
    }
}