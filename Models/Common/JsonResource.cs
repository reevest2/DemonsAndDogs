using System.Text.Json;
using System.Text.Json.Serialization;
using AppConstants;

namespace Models.Common;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Kind")]
[JsonDerivedType(typeof(CampaignResource), ResourceKinds.Campaign)]
[JsonDerivedType(typeof(CharacterResource), ResourceKinds.Character)]
[JsonDerivedType(typeof(DocumentDefinitionResource), ResourceKinds.DocumentDefinition)]
[JsonDerivedType(typeof(DocumentResource), ResourceKinds.Document)]
[JsonDerivedType(typeof(GameResource), ResourceKinds.Game)]
[JsonDerivedType(typeof(SchemaResource), ResourceKinds.Schema)]
public abstract record JsonResource
{
    [JsonPropertyName("Kind")]
    public abstract string Kind { get; }

    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string? EntityId { get; init; }
    public string? OwnerId { get; init; }
    public string? SubjectId { get; init; }
    public string? CampaignId { get; init; }
    public string? RulesetId { get; init; }
    public string? GameId { get; init; }
    public string? SchemaVersion { get; init; }
    public string? Schema { get; init; }
    public string? ResourceKind { get; init; }
    public JsonElement Data { get; init; }
}
