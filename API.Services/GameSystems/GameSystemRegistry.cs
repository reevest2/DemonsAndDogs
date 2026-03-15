using System.Reflection;
using Models.Attributes;
using Models.Interfaces;

namespace API.Services.GameSystems;

public class GameSystemRegistry : IGameSystemRegistry
{
    private readonly Dictionary<string, IRuleBook> _systems = new(StringComparer.OrdinalIgnoreCase);

    public GameSystemRegistry()
    {
        DiscoverSystems();
    }

    private void DiscoverSystems()
    {
        var types = typeof(GameSystemRegistry).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<GameSystemAttribute>() != null && 
                        typeof(IRuleBook).IsAssignableFrom(t) && 
                        !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<GameSystemAttribute>()!;
            var system = (IRuleBook)Activator.CreateInstance(type)!;
            _systems[attribute.SystemId] = system;
        }
    }

    public IRuleBook Get(string systemId)
    {
        if (!_systems.TryGetValue(systemId, out var system))
        {
            throw new KeyNotFoundException($"Game system '{systemId}' not found.");
        }
        return system;
    }

    public IEnumerable<IRuleBook> GetAll()
    {
        return _systems.Values;
    }
}
