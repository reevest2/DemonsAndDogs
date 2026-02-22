using Models.Resources.Abstract;
using Models.Resources.Ruleset;

namespace API.Services;

public class EntityFactory
{
    public Resource<EntityData> Create(
        string ownerId,
        string campaignId,
        string categoryKey,
        TemplateData template,
        string name,
        Dictionary<string, object?>? overrides = null)
    {
        var data = template.Seed.Count == 0
            ? new Dictionary<string, object?>()
            : template.Seed.ToDictionary(k => k.Key, v => v.Value);

        if (overrides is not null)
        {
            foreach (var kv in overrides)
                data[kv.Key] = kv.Value;
        }

        return new Resource<EntityData>
        {
            OwnerId = ownerId,
            EntityId = campaignId,
            CreatedAt = DateTime.UtcNow,
            Data = new EntityData
            {
                Name = name,
                CampaignId = campaignId,
                CategoryKey = categoryKey,
                SourceTemplateId = template.TemplateId,
                Data = data
            }
        };
    }
}