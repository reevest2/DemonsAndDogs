namespace Models.Resources.Campaign;

public class CampaignTemplateResource
{
    public string TemplateId { get; set; }
    public string TemplateName { get; set; }
    public string TemplateNotes { get; set; }
    public string? RulesetId { get; set; }
    public List<TemplateCategory> Templates { get; set; } = new();
}

public class TemplateCategory
{
    public string Key { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool IsEnabled { get; set; } = true;

    public int? Min { get; set; }
    public int? Max { get; set; }

    public List<string> TemplateIds { get; set; } = new();
    public string? DefaultTemplateId { get; set; }
}