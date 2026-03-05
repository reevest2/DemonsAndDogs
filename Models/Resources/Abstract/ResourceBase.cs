using System.Text.Json;

namespace Models.Resources;

public class ResourceBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? OwnerId { get; set; }
    public string? Key1 { get; set; }
    public string? Key2 { get; set; }
    public string? Key3 { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; }
}