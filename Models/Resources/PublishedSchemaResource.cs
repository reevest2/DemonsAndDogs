using System.Text.Json;
using Models.Resources.Abstract;

namespace Models.Resources;

public class PublishedSchemaResource : ResourceBase
{
    public string SchemaKey { get; set; } = string.Empty;
    public int Version { get; set; }
    public JsonElement PublishedJson { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
