using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace FitJourney.API.Json;

public static class MongoIdAliasModifier
{
    public static void AddIdAlias(JsonTypeInfo info)
    {
        if (info.Kind != JsonTypeInfoKind.Object) return;

        var idProp = info.Properties.FirstOrDefault(p =>
            p.Name.Equals("id", StringComparison.OrdinalIgnoreCase)
            && p.PropertyType == typeof(string));
        if (idProp == null) return;

        var alias = info.CreateJsonPropertyInfo(typeof(string), "_id");
        alias.Get = idProp.Get;
        info.Properties.Add(alias);
    }
}
