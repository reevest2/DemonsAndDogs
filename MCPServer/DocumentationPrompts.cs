using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerPromptType]
public static class DocumentationPrompts
{
    [McpServerPrompt(Name = "generate_feature", Title = "Generate feature using solution docs")]
    [Description("Generates an implementation plan and code instructions using the solution documentation via MCP tools.")]
    public static string GenerateFeature(
        [Description("Feature request, e.g. Add customer grid with paging")] string request)
    {
        return
            $"""
             You are working in this repository.

             1) Call the MCP tool ListDocumentation.
             2) Identify the most relevant docs for this request: "{request}".
             3) Call GetDocumentation(name) for each relevant doc (at minimum: index + any doc related to UI/grid/paging/API conventions).
             4) Follow the docs as requirements. If docs conflict with your default approach, prefer the docs.
             5) Produce:
                - Implementation plan (files to change, steps)
                - Then generate the code changes.

             Request: {request}
             """;
    }
}