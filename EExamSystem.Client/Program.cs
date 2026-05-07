using EExamSystem.Client.Components;
using EExamSystem.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Auth infrastructure
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiClient>();

// Named HttpClient — no handler; ApiClient (scoped) attaches the Bearer token
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "BlazorServer";
    options.DefaultChallengeScheme = "BlazorServer";
})
.AddCookie("BlazorServer", options =>
{
    options.LoginPath = "/login";
});
builder.Services.AddAuthorization();

// Registers CascadingAuthenticationState globally (no App.razor change needed)
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<LanguageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAuthentication();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
