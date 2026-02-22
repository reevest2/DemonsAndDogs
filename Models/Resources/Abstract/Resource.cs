using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace Models.Resources.Abstract;

public class Resource<T>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? EntityId { get; set; }
    public string? OwnerId { get; set; }
    public string? SubjectId { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool IsDeleted { get; set; }
    
    [Column(TypeName = "jsonb")]
    public T Data { get; set; }
    
    //TODO: Vector Storage?
    // [Column(TypeName = "vector(1536)")]
    // public Vector Embedding { get; set; }
}