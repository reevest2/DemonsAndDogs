using System.Text.Json;

namespace Models.Common;

public abstract record JsonResource
{
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
