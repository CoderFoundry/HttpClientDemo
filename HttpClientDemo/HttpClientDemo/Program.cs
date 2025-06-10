using HttpClientDemo.Client.Services;
using HttpClientDemo.Components;
using HttpClientDemo.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var tmdbApiKey = builder.Configuration["TmdbApiKey"]
    ?? Environment.GetEnvironmentVariable("TmdbApiKey")
    ?? throw new InvalidOperationException("TMDB API key is not configured.");

builder.Services.AddHttpClient<ITMDBService, TMDBService>(client => 
{ 
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.DefaultRequestHeaders.Authorization = new("Bearer", tmdbApiKey);
});

//builder.Services.AddScoped<ITMDBService, TMDBServiceWrong>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HttpClientDemo.Client._Imports).Assembly);

app.Run();
