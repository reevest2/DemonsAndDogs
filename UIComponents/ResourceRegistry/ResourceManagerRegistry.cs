
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
    Func<object> CreateGrid
);

/// <summary>
/// Registry for ResourceManagementPage component. The page displays an editor and a grid.
/// </summary>
public sealed class ResourceManagerRegistry : IResourceManagerRegistry
{
    private static readonly Dictionary<string, ResourceManagerConfig> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            
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