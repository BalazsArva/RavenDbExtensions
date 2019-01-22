namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public interface IPatchScriptBodyBuilder
    {
        string CreateScriptBody(PropertyUpdateDescriptor[] propertyUpdates, ScriptParameterDictionary parameters);
    }
}