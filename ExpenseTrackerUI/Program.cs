using ExpenseTrackerUI.Components;
using ExpenseTrackerUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Simple cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient<IExpenseService, ExpenseService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/");
});

var app = builder.Build();

// Add Permissions Policy middleware to suppress unload warnings
app.Use(async (context, next) =>
{
    // Set Permissions-Policy header to explicitly disallow deprecated features
    context.Response.Headers.Append("Permissions-Policy", "unload=()");
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Auth endpoints - proxy to backend API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/";

app.MapPost("/api/auth/login", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<LoginRequest>();
    if (request == null) return Results.BadRequest();

    using var http = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    var response = await http.PostAsJsonAsync("api/auth/login", request);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return Results.Json(new { message = error?.Message ?? "Login failed." }, statusCode: (int)response.StatusCode);
    }

    var result = await response.Content.ReadFromJsonAsync<AuthResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    var displayName = !string.IsNullOrEmpty(result!.Name) ? result.Name : result.Email;
    var claims = new List<Claim>
    {
        new(ClaimTypes.Email, result.Email),
        new(ClaimTypes.Name, displayName)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Ok(new { email = result.Email });
});

app.MapPost("/api/auth/register", async (HttpContext context) =>
{
    var request = await context.Request.ReadFromJsonAsync<LoginRequest>();
    if (request == null) return Results.BadRequest();

    using var http = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    var response = await http.PostAsJsonAsync("api/auth/register", request);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return Results.Json(new { message = error?.Message ?? "Registration failed." }, statusCode: (int)response.StatusCode);
    }

    var result = await response.Content.ReadFromJsonAsync<AuthResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    // Auto sign-in after registration
    var displayName = !string.IsNullOrEmpty(result!.Name) ? result.Name : result.Email;
    var claims = new List<Claim>
    {
        new(ClaimTypes.Email, result.Email),
        new(ClaimTypes.Name, displayName)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Ok(new { email = result.Email });
});

app.MapGet("/api/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// DTOs for auth endpoints
record LoginRequest(string Email, string Password, string? Name = null);
record AuthResponse(string Email, string? Name = null);
record ErrorResponse(string Message);
