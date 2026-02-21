using AppConstants;
using Models.Resources;
using UIComponents.CharacterSheet;
using UIComponents.ResourceGrid;

namespace API.Services.ResourceRegistry;

public interface IResourceManagerRegistry
{
    bool TryGet(string resourceName, out (Type editor, Type grid) config);

}

public sealed class ResourceManagerRegistry : IResourceManagerRegistry
{
    private static readonly Dictionary<string, (Type editor, Type grid)> Registry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [ResourceKeys.CharacterResources] = (typeof(CharacterSheetEditor), typeof(ResourceGrid<CharacterResource>))
        };

    public bool TryGet(string resourceName, out (Type editor, Type grid) config)
    {
        if (resourceName == null)
        {
            config = default;
            return false;
        }

        return Registry.TryGetValue(resourceName, out config);
    }
}