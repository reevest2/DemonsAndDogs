using System.ComponentModel;
using Documentation;
using ModelContextProtocol.Server;

namespace MCPServer;

[McpServerToolType]
public static class DocumentationTools
{
    [McpServerTool, Description("Lists all available documentation pages.")]
    public static string ListDocumentation()
    {
        var documents = DocumentationProvider.ListDocuments();

        return documents.Count == 0
            ? "No documentation pages found."
            : string.Join("\n", documents);
    }

    [McpServerTool, Description("Gets the content of a documentation page by name. Use ListDocumentation to see available pages.")]
    public static string GetDocumentation(
        [Description("The name of the documentation page to retrieve (e.g. 'index', 'architecture').")] string name)
    {
        var content = DocumentationProvider.GetDocument(name);

        return content ?? $"Documentation page '{name}' not found. Use ListDocumentation to see available pages.";
    }
}
