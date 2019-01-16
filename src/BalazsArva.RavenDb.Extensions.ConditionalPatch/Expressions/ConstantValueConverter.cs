using Newtonsoft.Json;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public class ConstantValueConverter
    {
        public static string ConvertToJson(object value)
        {
            // TODO: Ensure that RavenDb's serialization conventions are followed (e.g. enums are strings by default in RavenDb, but int according to Newtonsoft)
            return JsonConvert.SerializeObject(value);
        }
    }
}