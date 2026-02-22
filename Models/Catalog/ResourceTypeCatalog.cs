using Models.Catalog.Record;
using Models.Enums;

namespace Models.Catalog;

public static class ResourceTypeCatalog
{
    public static readonly IReadOnlyList<ResourceTypeDefinition> All =
    [
        new(ResourceType.Document, "Document", "General", null), //typeof(DocumentEditor)),

        new(ResourceType.CampaignNote, "Campaign Note", "Campaign", null), //typeof(CampaignNoteEditor)),
        new(ResourceType.CampaignSessionLog, "Session Log", "Campaign", null), //typeof(SessionLogEditor)),

        new(ResourceType.EntityCharacter, "Character", "Entity", null), //typeof(CharacterEditor)),
        new(ResourceType.EntityNpc, "NPC", "Entity", null), //typeof(NpcEditor)),

        new(ResourceType.RulesetRule, "Rule", "Ruleset", null), //typeof(RuleEditor)),

        new(ResourceType.TemplatePrompt, "Prompt Template", "Template", null), //typeof(PromptEditor))
    ];

    public static ResourceTypeDefinition Get(ResourceType type) =>
        All.First(x => x.Type == type);
}