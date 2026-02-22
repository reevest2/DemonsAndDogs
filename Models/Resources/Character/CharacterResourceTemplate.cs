using System.Text.Json;
using Models.Character;

namespace Models.Resources.Character;

public class CharacterTemplateData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ThumbnailMetadata Thumbnail { get; set; }
    public List<CharacterTemplateSection> Sections { get; set; } = new();
}

public class CharacterTemplateSection
{
    public string Key { get; set; }
    public string Label { get; set; }
    public int Order { get; set; }
    public List<CharacterTemplateField> Fields { get; set; } = new();
}

public enum CharacterFieldType
{
    Text,
    MultilineText,
    Number,
    Boolean,
    Select,
    MultiSelect,
    Date,
    Color,
    ImageUrl
}

public class CharacterTemplateField
{
    public string Key { get; set; }
    public string Label { get; set; }
    public CharacterFieldType Type { get; set; }
    public bool Required { get; set; }
    public int Order { get; set; }

    public JsonElement? DefaultValue { get; set; }
    public List<CharacterTemplateOption> Options { get; set; } = new();

    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public int? MaxLength { get; set; }
}

public class CharacterTemplateOption
{
    public string Value { get; set; }
    public string Label { get; set; }
}

public class CharacterData
{
    public string TemplateResourceId { get; set; }
    public string TemplateVersion { get; set; }

    public ThumbnailMetadata Thumbnail { get; set; }

    public Dictionary<string, JsonElement> Values { get; set; } = new();
}