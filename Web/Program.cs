using Web;
using Web.Authentication;
using Web.Components;
using Web.Data;
using ApplicationUser = Web.Authentication.AuthenticationUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();
builder.AddSqlServerDbContext<AuthenticationDbContext>("authdb");
builder.Services.AddScoped<AuthenticationDatabaseInitializer>();
builder.Services.AddSingleton<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
    });
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
    client.BaseAddress = new("https+http://apiservice");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<AuthenticationDatabaseInitializer>();
    await initializer.InitializeAsync(app.Environment.IsDevelopment());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapStaticAssets();
app.MapPost("/account/login", async (
    HttpContext context,
    IAntiforgery antiforgery,
    AuthenticationDbContext database,
    IPasswordHasher<ApplicationUser> passwordHasher) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest();
    }

    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim().ToLowerInvariant();
    var user = await database.Users.SingleOrDefaultAsync(candidate => candidate.Username == username);

    if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, form["password"].ToString()) == PasswordVerificationResult.Failed)
    {
        return Results.Redirect("/Login?error=invalid");
    }

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        AuthenticationUser.CreatePrincipal(user));

    var returnUrl = form["returnUrl"].ToString();
    return Results.LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
});
app.MapPost("/account/logout", async (HttpContext context, IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest();
    }

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/Login");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
