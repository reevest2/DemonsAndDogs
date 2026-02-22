namespace Models.Resources.Ruleset;

public class CampaignData
{
    public string Name { get; set; } = default!;
    public string RulesetTemplateId { get; set; } = default!;
}

public class RulesetResource
{
    public string TemplateName { get; set; } = default!;
    public string? Notes { get; set; }
    public List<RulesetCategorySpec> Categories { get; set; } = new();
}

public class RulesetCategorySpec
{
    public string Key { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool RequiredForCampaign { get; set; }
    public int? DefaultMin { get; set; }
    public int? DefaultMax { get; set; }
}

public class TemplateData
{
    public string Name { get; set; } = default!;
    public string TemplateId { get; set; } = default!;
    public string CategoryKey { get; set; } = default!;
    public Dictionary<string, object?> Seed { get; set; } = new();
}

public class EntityData
{
    public string Name { get; set; } = default!;
    public string CampaignId { get; set; } = default!;
    public string CategoryKey { get; set; } = default!;
    public string? SourceTemplateId { get; set; }
    public Dictionary<string, object?> Data { get; set; } = new();
}