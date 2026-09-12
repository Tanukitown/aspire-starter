# Web Frontend

The `Web` project is the Blazor frontend for this Aspire application. It uses Fluent UI Blazor v5 preview for its application shell and UI components, and LibMan for third-party browser assets.

## Run and Validate

Start the application from the repository root with the Aspire CLI:

```bash
aspire start --non-interactive
aspire wait webfrontend --non-interactive
```

Use `aspire describe --format Json --non-interactive` to find the published frontend URL. After changing the frontend while Aspire is running, rebuild only that resource:

```bash
aspire resource webfrontend rebuild --non-interactive
aspire wait webfrontend --non-interactive
```

For a focused compile check when Aspire is not running:

```bash
dotnet build Web/Web.csproj
```

## Authentication

The AppHost provisions the `auth-sql` SQL Server resource and its `authdb` database. The Web application uses cookie authentication against the `Users` table in that database. Unauthenticated visitors are redirected to `/Login`; the application navigation, header, and footer are rendered only for authenticated users.

Usernames are generated from the first six letters of the last name and the first letter of the first name. The first matching user receives the base username; later collisions receive a numeric suffix, starting at `2`. For example, Jordan Example receives `examplj`, while a later user with the same name prefix receives `examplj2`.

In Development, the application creates one account when the user table is empty:

```text
Username: examplj
Password: ChangeMe123!
```

The values are configured in `appsettings.Development.json`. Production creates the database schema but does not seed an account; provision production users through an administrative workflow before granting access.

## Fluent UI v5

Fluent UI is registered in `Program.cs`. Components and icons are available project-wide through `Components/_Imports.razor`:

```razor
@using Microsoft.FluentUI.AspNetCore.Components
@using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons
```

Use Fluent components in Razor pages instead of custom equivalents where the library supplies one:

```razor
<FluentButton Appearance="ButtonAppearance.Primary" OnClick="SaveAsync">
    Save
</FluentButton>

<FluentNavItem Href="settings"
               IconRest="@(new Icons.Regular.Size20.Settings())"
               IconActive="@(new Icons.Filled.Size20.Settings())">
    Settings
</FluentNavItem>
```

The shared application shell is in `Components/Layout/MainLayout.razor`; navigation belongs in `Components/Layout/NavMenu.razor`. Use the [Fluent UI v5 icon catalog](https://v5.fluentui-blazor.net/Icon) to find an icon and select matching `Regular` and `Filled` variants for navigation states.

The Fluent package versions are centrally managed in `../Directory.Packages.props`. Add a `PackageReference` without a version to `Web.csproj`, then add its version to the `ItemGroup Label="Web"` group in that central file.

## JavaScript and CSS Libraries

Declare third-party browser libraries in `libman.json`. Do not manually copy vendor files into `wwwroot/lib`.

The LibMan tool is defined in the repository tool manifest. Restore it first when needed:

```bash
dotnet tool restore
```

Add a library by choosing a supported CDN provider and destination, for example:

```bash
dotnet libman install alpinejs@3.14.9 \
  --provider cdnjs \
  --destination wwwroot/lib/alpinejs
```

Restore all libraries declared in `libman.json` with:

```bash
dotnet libman restore
```

Reference the restored files from `Components/App.razor` using `@Assets[...]`, and remove the LibMan declaration when the library is no longer used.
