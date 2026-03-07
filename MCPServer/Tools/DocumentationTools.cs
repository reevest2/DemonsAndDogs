using System.ComponentModel;
using System.Reflection;
using Documentation;
using ModelContextProtocol.Server;

namespace MCPServer.Tools;

[McpServerToolType]
public static class DocumentationTools
{
    /// <summary>
    /// Lists all available documentation files embedded in the Documentation assembly.
    /// Call this first to see what context is available before calling GetDoc.
    /// </summary>
    [McpServerTool]
    [Description("List all available project documentation files. Call this to see what docs exist before fetching one.")]
    public static string[] ListDocs()
    {
        return GetAssembly()
            .GetManifestResourceNames()
            .Where(n => n.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .Select(n => Path.GetFileNameWithoutExtension(n))
            .OrderBy(n => n)
            .ToArray();
    }

    /// <summary>
    /// Returns the full contents of a documentation file.
    /// </summary>
    [McpServerTool]
    [Description(
        "Get the contents of a project documentation file by name. " +
        "Examples: 'architecture', 'best-practices', 'game-engine'. " +
        "Call ListDocs first if you are unsure what is available.")]
    public static string GetDoc(
        [Description("The doc name without extension, e.g. 'architecture' or 'game-engine'")]
        string name)
    {
        var assembly = GetAssembly();

        var resource = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(n => n.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (resource is null)
        {
            var available = string.Join(", ", assembly
                .GetManifestResourceNames()
                .Where(n => n.EndsWith(".md"))
                .Select(Path.GetFileNameWithoutExtension));
            return $"No doc found matching '{name}'. Available docs: {available}";
        }

        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Returns all docs concatenated — useful for giving full project context in one call.
    /// </summary>
    [McpServerTool]
    [Description("Get all project documentation in one call. Use when you need full project context.")]
    public static string GetAllDocs()
    {
        var assembly = GetAssembly();

        var resources = assembly
            .GetManifestResourceNames()
            .Where(n => n.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => n);

        var sections = resources.Select(resource =>
        {
            using var stream = assembly.GetManifestResourceStream(resource)!;
            using var reader = new StreamReader(stream);
            var name = Path.GetFileNameWithoutExtension(resource);
            return $"# [{name}]\n\n{reader.ReadToEnd()}";
        });

        return string.Join("\n\n---\n\n", sections);
    }

    private static Assembly GetAssembly() =>
        Assembly.GetAssembly(typeof(DocumentationAnchor))
        ?? throw new InvalidOperationException("Could not locate Documentation assembly.");
}
