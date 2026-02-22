using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models.Enums;

namespace Models.Resources.Abstract;

public abstract class ResourceBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? EntityId { get; set; }
    public string? OwnerId { get; set; }
    public string? SubjectId { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; }
    public ResourceType ResourceTypeKey { get; set; }
    public string? ResourceName { get; set; }
    public string? ResourceDescription { get; set; }
    
    //TODO: Vector Storage?
    // [Column(TypeName = "vector(1536)")]
    // public Vector Embedding { get; set; }
}