using System.Text.Json;
using Models.Resources.Abstract;

namespace Models.Resources;

public class DataResource : ResourceBase
{
    public string SchemaKey { get; set; } = string.Empty;
    public int SchemaVersion { get; set; }
    public string Name { get; set; } = string.Empty;
    public JsonElement JsonPayload { get; set; }
}
