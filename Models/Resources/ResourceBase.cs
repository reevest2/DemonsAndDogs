using System.Text.Json;

namespace Models.Resources;

public class ResourceBase<T>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntityId { get; set; }
    public string OwnerId { get; set; }
    public string SubjectId { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; }
    public required T Data { get; set; }
}