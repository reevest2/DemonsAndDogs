using System.ComponentModel;
using System.Reflection;
using System.Text;
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

    /// <summary>
    /// Call this at the start of every task to load full project context.
    /// Returns index, current state, and available docs in one call.
    /// </summary>
    [McpServerTool]
    [Description("Call this at the start of every task to load full project context. Returns index, current state, and available docs in one call.")]
    public static string GetStarted()
    {
        var assembly = GetAssembly();
        var allResources = assembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var sb = new StringBuilder();

        sb.AppendLine("# MASTER INDEX (index.md)");
        sb.AppendLine(ReadResource(assembly, allResources, "index.md"));
        sb.AppendLine("\n---\n");

        sb.AppendLine("# CURRENT STATE (current-state.md)");
        sb.AppendLine(ReadResource(assembly, allResources, "current-state.md"));
        sb.AppendLine("\n---\n");

        sb.AppendLine("# AVAILABLE DOCUMENTATION INDEX");
        foreach (var resource in allResources.OrderBy(n => n))
        {
            var name = Path.GetFileNameWithoutExtension(resource);
            var description = GetDocDescription(assembly, resource);
            sb.AppendLine($"- **{name}**: {description}");
        }

        return sb.ToString();
    }

    private static string ReadResource(Assembly assembly, IEnumerable<string> resources, string name)
    {
        var resource = resources.FirstOrDefault(n => n.Equals(name, StringComparison.OrdinalIgnoreCase) 
                                                     || n.EndsWith("." + name, StringComparison.OrdinalIgnoreCase));
        
        if (resource == null)
        {
            return $"[ERROR: {name} not found. Please ensure this file exists in the 'docs/' folder and is correctly embedded in the Documentation project.]";
        }

        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string GetDocDescription(Assembly assembly, string resource)
    {
        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var reader = new StreamReader(stream);

        string? firstHeading = null;
        string? firstParagraph = null;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("#") && firstHeading == null)
            {
                firstHeading = line.TrimStart('#').Trim();
                continue;
            }

            if (!line.StartsWith("#"))
            {
                firstParagraph = line;
                break;
            }
        }

        return firstParagraph ?? firstHeading ?? "No description available.";
    }

    private static Assembly GetAssembly() =>
        Assembly.GetAssembly(typeof(DocumentationAnchor))
        ?? throw new InvalidOperationException("Could not locate Documentation assembly.");
}
