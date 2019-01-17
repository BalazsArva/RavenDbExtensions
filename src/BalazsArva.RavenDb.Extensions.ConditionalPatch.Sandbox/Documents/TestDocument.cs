namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox.Documents
{
    public class TestDocument
    {
        public string Id { get; set; }

        public long LastKnownChangeId { get; set; }

        public long[] RecordedChangeIds { get; set; }
    }
}