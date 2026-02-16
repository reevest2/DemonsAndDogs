using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Resources;

public class Resource<T> : ResourceBase where T : ResourceBase
{
    [Column(TypeName = "jsonb")]
    public T Data { get; set; }
}