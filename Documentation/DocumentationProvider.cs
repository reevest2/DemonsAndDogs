using System.Reflection;

namespace Documentation;

public static class DocumentationProvider
{
    private static readonly Assembly Assembly = typeof(DocumentationProvider).Assembly;
    private const string ContentPrefix = "Documentation.Content.";

    public static IReadOnlyList<string> ListDocuments()
    {
        return Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ContentPrefix) && name.EndsWith(".md"))
            .Select(name => name[ContentPrefix.Length..^3]) // Remove prefix and .md extension
            .ToList();
    }

    public static string? GetDocument(string name)
    {
        var resourceName = $"{ContentPrefix}{name}.md";

        using var stream = Assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
