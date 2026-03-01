using System.Text.Json;
using Models.Resources.Abstract;

namespace Models.Resources;

public abstract class JsonResource : ResourceBase
{
    public JsonElement Data { get; set; }
}