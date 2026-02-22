using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;
using Pgvector;

namespace Models.Resources.Abstract;

public class Resource<T> : ResourceBase where T : ResourceBase
{
    [Column(TypeName = "jsonb")]
    public T Data { get; set; }
}