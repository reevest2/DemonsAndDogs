using System.Text.Json;
using Models.Resources.Abstract;

namespace Models.Resources;

public class SchemaResource : ResourceBase
{
    public string SchemaKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JsonElement DraftJson { get; set; }
    public int? LatestPublishedVersion { get; set; }
}
