using D3code.Client.Pages;
using D3code.Components;
using D3code.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register HttpClientFactory with a named HttpClient
builder.Services.AddHttpClient("EnerQuantumApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseUrl") ?? "https://localhost:7293");
});

// Configure EF Core with Npgsql
builder.Services.AddDbContextPool<EnerQuantumDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add support for API controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure static files (e.g., CSS) are served
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(D3code.Client._Imports).Assembly);

// Map API controllers
app.MapControllers();

app.Run();