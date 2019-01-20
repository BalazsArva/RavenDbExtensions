namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments
{
    public class TestDocument
    {
        public string SomeString { get; set; }

        public int SomeInt { get; set; }

        public long? SomeNullableLong { get; set; }

        public int[] SomeIntArray { get; set; }
    }
}