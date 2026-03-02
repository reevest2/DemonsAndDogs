using System.Text.Json;

namespace Models.Resources;

public class JsonResource
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? EntityId { get; set; }
    public string? OwnerId { get; set; }
    public string? SubjectId { get; set; }
    public string? CampaignId { get; set; }
    public string? RulesetId { get; set; }
    public string? GameId { get; set; }
    public string? SchemaVersion { get; set; }
    public string? ResourceKind { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; }
    
    //TODO: Vector Storage?
    // [Column(TypeName = "vector(1536)")]
    // public Vector Embedding { get; set; }
    public JsonElement Data { get; set; }
}