---
name: dotnet-project-organization
description: "Use when: creating or editing .csproj, Directory.Build.props, Directory.Packages.props, Blazor static assets, or JavaScript library dependencies in this repository."
---

# .NET Project Organization

Keep MSBuild project files organized with descriptive labels:

- Every `ItemGroup` in a `.csproj` must have a `Label` describing its contents, such as `Project References`, `Package References`, `Framework References`, or `Global Usings`.
- Do not mix different item types in one `ItemGroup`; separate them so each label is accurate.
- Central package versions belong in `Directory.Packages.props`. Group `PackageVersion` items by consuming project with `ItemGroup Label="<ProjectName>"`.
- Shared package versions must be placed in a group labeled `Shared`.
- Common build properties belong in `Directory.Build.props`; keep project-specific properties in the relevant `.csproj`.
- Add third-party JavaScript and CSS browser libraries for the Blazor application through `Web/libman.json`. Do not manually copy vendor assets into `Web/wwwroot/lib`.
- Use `dotnet libman install` to add a library and `dotnet libman restore` to restore the declared libraries. Keep `Web/libman.json` as the source of truth for those static dependencies.
- When Fluent UI Blazor is referenced by the Web project, use its components wherever an equivalent exists instead of native or custom UI controls. Follow the `fluentui-blazor-mcp` skill to verify the exact v5 API before authoring Fluent markup.
