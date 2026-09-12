---
name: fluentui-blazor-mcp
description: "Use when: writing, reviewing, or debugging Fluent UI Blazor markup or code (Microsoft.FluentUI.AspNetCore.Components), choosing a Fluent component or enum, or checking Fluent UI Blazor version compatibility in this repository."
---

# Fluent UI Blazor MCP Server

This repository's `Web` project uses `Microsoft.FluentUI.AspNetCore.Components` (v5 preview). A dedicated MCP server, `fluent-ui-blazor`, is configured in [`.vscode/mcp.json`](../../../.vscode/mcp.json) and provides authoritative, version-matched documentation for the library.

**Always consult the `fluent-ui-blazor` MCP server before guessing at Fluent component APIs, parameters, events, or enums.** Do not invent component names, parameter names, or enum values from memory — verify them with the server's tools/resources first.

**Always use Fluent UI Blazor components when the library provides an equivalent.** Do not introduce native HTML controls, custom component substitutes, or hand-authored visual styling for buttons, inputs, selects, checkboxes, navigation, alerts, menus, dialogs, tables, tabs, or similar UI controls. Use native semantic elements only where Fluent UI has no suitable component, including form structure and non-visual inputs.

## When to use it

- Choosing or confirming a component (e.g. `FluentDataGrid`, `FluentNavItem`) and its parameters/events/methods.
- Looking up enum types and valid values (e.g. `Appearance`, `ButtonAppearance`).
- Searching for a component or documentation topic by keyword.
- Verifying the installed `Microsoft.FluentUI.AspNetCore.Components` version in a `.csproj` is compatible with the MCP server's documentation.
- Migrating Fluent UI Blazor code to v5.

## Key tools

| Tool                                                                  | Use for                                                                  |
| --------------------------------------------------------------------- | ------------------------------------------------------------------------ |
| `ListComponents` / `ListCategories`                                   | Browse available components                                              |
| `GetComponentDetails`                                                 | Full docs for one component (parameters, events, methods)                |
| `SearchComponents`                                                    | Find a component by name or description                                  |
| `GetEnumValues` / `GetComponentEnums` / `ListEnums`                   | Look up enum types and values                                            |
| `ListDocumentation` / `GetDocumentationTopic` / `SearchDocumentation` | General Fluent UI Blazor docs (installation, styles, localization, etc.) |
| `GetMigrationGuide`                                                   | Full guide for migrating to v5                                           |
| `GetVersionInfo` / `CheckProjectVersion`                              | Verify component library version compatibility                           |

## Version compatibility workflow

Before relying on documentation results, confirm versions match:

1. Call `GetVersionInfo` to get the MCP server's expected `Microsoft.FluentUI.AspNetCore.Components` version.
2. Read the version pinned in `Directory.Packages.props` (`ItemGroup Label="Web"`) or the relevant `.csproj`.
3. Call `CheckProjectVersion` with that version.
4. If `INCOMPATIBLE`, warn the user and follow the returned upgrade instructions before trusting other results.

## Setup reference

The server is registered as a local .NET tool in [`dotnet-tools.json`](../../../dotnet-tools.json) and configured in [`.vscode/mcp.json`](../../../.vscode/mcp.json):

```json
{
    "servers": {
        "fluent-ui-blazor": {
            "type": "stdio",
            "command": "dotnet",
            "args": ["tool", "run", "fluentui-mcp"]
        }
    }
}
```

Run `dotnet tool restore` after cloning so the `fluentui-mcp` command is available. Keep the pinned version in `dotnet-tools.json` in sync with the `Microsoft.FluentUI.AspNetCore.Components` version used by the `Web` project.
