using AppConstants;
using Models.Character;
using Models.Resources;
using Models.Resources.Abstract;
using UIComponents.CharacterSheet;
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
    /// <summary>
    /// Key Should be the Resource Name from ResourceKeys. Then add an editor and a grid.
    /// </summary>
    private static readonly Dictionary<string, ResourceManagerConfig> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [ResourceKeys.CharacterResources] = new ResourceManagerConfig(
                typeof(CharacterSheetEditor),
                typeof(ResourceGrid<CharacterResource>),
                () => new CharacterResource
                {
                    CharacterSheet = new Models.Character.CharacterSheet
                    {
                        Sections = new List<CharacterSheetSection>()
                    }
                },
                () => Array.Empty<Resource<CharacterResource>>()
            )
        };

    public bool TryGet(string resourceName, out ResourceManagerConfig config)
    {
        if (resourceName == null)
        {
            config = default;
            return false;
        }

        return Map.TryGetValue(resourceName, out config);
    }

    public IReadOnlyCollection<string> GetRegisteredResources()
        => Map.Keys.ToList();
}