# Demons and Dogs Documentation

Welcome to the Demons and Dogs project documentation. This documentation is served via an MCP server and provides guidance on project architecture, patterns, and how to contribute.

## Available Pages

- **architecture** - Overview of the project architecture and patterns used.
- **generalRules** - General rules and guidelines for contributing to the project.

## How to Add a New Documentation Page

1. Create a new `.md` file in the `Documentation/Content` folder.
2. Write your documentation using standard Markdown syntax.
3. Rebuild the `Documentation` project — the file will be automatically embedded as a resource.
4. The MCP server will serve the new page using the file name (without extension) as the page name.

For example, adding `Documentation/Content/deployment.md` makes it available via the `GetDocumentation` tool with `name: "deployment"`.
