using System.Text.Json;
using AppConstants;
using Models.Character;
using Models.Resources;
using Models.Resources.Abstract;
using UIComponents.ResourceGrid;
using UIComponents.ResourceRegistry;

namespace UIComponents.ResourceRegistry;

public interface IResourceManagerRegistry
{
    bool TryGet(string resourceName, out ResourceManagerConfig config);
    IReadOnlyCollection<string> GetRegisteredResources();
}

public sealed record ResourceManagerConfig(
    Type Editor,
    Type Grid,
    Func<object> CreateModel,
    Func<object> CreateGridItems
);

/// <summary>
/// Registry for ResourceManagementPage component. The page displays an editor and a grid.
/// </summary>
public sealed class ResourceManagerRegistry : IResourceManagerRegistry
{
    private static readonly Dictionary<string, ResourceManagerConfig> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [ResourceKeys.CharacterResources] = new ResourceManagerConfig(
                typeof(CharacterEditor),
                typeof(ResourceGrid<CharacterData>),
                () => new CharacterData
                {
                    Thumbnail = new ThumbnailMetadata(),
                    Values = new Dictionary<string, JsonElement>()
                    
                },
                () => Array.Empty<Resource<CharacterData>>()
            ),

            [ResourceKeys.CharacterTemplateResources] = new ResourceManagerConfig(
                typeof(CharacterTemplateEditor),
                typeof(ResourceGrid<CharacterTemplateData>),
                () => new CharacterTemplateData
                {
                    Name = "New Template",
                    Description = "",
                    Thumbnail = new ThumbnailMetadata(),
                    Sections = new List<CharacterTemplateSection>()
                },
                () => Array.Empty<Resource<CharacterTemplateData>>()
            )
        };

    public bool TryGet(string resourceName, out ResourceManagerConfig config)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
        {
            config = default;
            return false;
        }

        return Map.TryGetValue(resourceName, out config);
    }

    public IReadOnlyCollection<string> GetRegisteredResources()
        => Map.Keys.ToList();
}