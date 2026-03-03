using System.Linq;

namespace Models.Resources;

public class JsonResourceQuery
{
    public string? EntityId { get; set; }
    public string? OwnerId { get; set; }
    public string? SubjectId { get; set; }
    public string? CampaignId { get; set; }
    public string? RulesetId { get; set; }
    public string? GameId { get; set; }
    public string? ResourceKind { get; set; }

    public IQueryable<JsonResource> ApplyTo(IQueryable<JsonResource> query)
    {
        if (!string.IsNullOrEmpty(EntityId)) query = query.Where(r => r.EntityId == EntityId);
        if (!string.IsNullOrEmpty(OwnerId)) query = query.Where(r => r.OwnerId == OwnerId);
        if (!string.IsNullOrEmpty(SubjectId)) query = query.Where(r => r.SubjectId == SubjectId);
        if (!string.IsNullOrEmpty(CampaignId)) query = query.Where(r => r.CampaignId == CampaignId);
        if (!string.IsNullOrEmpty(RulesetId)) query = query.Where(r => r.RulesetId == RulesetId);
        if (!string.IsNullOrEmpty(GameId)) query = query.Where(r => r.GameId == GameId);
        if (!string.IsNullOrEmpty(ResourceKind)) query = query.Where(r => r.ResourceKind == ResourceKind);
        return query;
    }
}
