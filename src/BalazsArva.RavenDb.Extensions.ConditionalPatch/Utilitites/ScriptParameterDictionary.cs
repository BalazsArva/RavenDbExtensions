using System.Collections.Generic;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class ScriptParameterDictionary : Dictionary<string, object>
    {
        private readonly object lockObject = new object();
        private int counter = 0;

        public string AddNext(object value)
        {
            var key = $"__param{++counter}";

            this[key] = value;

            return key;
        }
    }
}