using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using BlazorTool.Components;
using BlazorTool.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("ExternalApiBasicAuthClient", client =>
{
    // BaseAddress EXTERNAL API, where get token
    client.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!);
});

builder.Services.AddHttpClient("ExternalApiBearerAuthClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!);
})
.AddHttpMessageHandler<ServerAuthHeaderHandler>();

// inject IHttpClientFactory & IMemoryCache
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var loginDto = new LoginRequest
    {
        Username = "Romaniuk Krzysztof", //TODO auth page, or use config["Auth:Username"]
        Password = "q"                   
    };
    return new ServerAuthTokenService(
        sp.GetRequiredService<IMemoryCache>(),
        sp.GetRequiredService<IHttpClientFactory>(),
        config,
        loginDto
    );
});
builder.Services.AddScoped<ServerAuthHeaderHandler>();

string? internalApiBaseUrl = null;
if (builder.Environment.IsDevelopment())
{
    // ASPNETCORE_URLS "https://localhost:7282;http://localhost:5168")
    var urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
    // 1st URL, "http://"
    internalApiBaseUrl = urls?.FirstOrDefault(url => url.StartsWith("http://"));

    if (string.IsNullOrEmpty(internalApiBaseUrl))
    {
        internalApiBaseUrl = urls?.FirstOrDefault();
    }

    if (string.IsNullOrEmpty(internalApiBaseUrl))
    {
        internalApiBaseUrl = "http://localhost:5168"; //default fallback URL
        Debug.Print("======= SERVER: ASPNETCORE_URLS is empty or invalid, falling back to default http://localhost:5168");
    }
    else
    {
        Debug.Print($"======= SERVER: InternalApiClient BaseAddress set to {internalApiBaseUrl} from ASPNETCORE_URLS");
    }
}
else //prodaction or other environment
{
    internalApiBaseUrl = builder.Configuration["HostAddress"] ?? throw new InvalidOperationException("HostAddress not configured for non-development environment.");
}
//internal controllers Blazor Host
builder.Services.AddHttpClient("InternalApiClient", client =>
{
    client.BaseAddress = new Uri(internalApiBaseUrl);
});

builder.Services.AddScoped<ApiServiceClient>(sp =>
    new ApiServiceClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExternalApiBearerAuthClient")));


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddTelerikBlazor();
builder.Services.AddScoped<AppointmentService>();

var app = builder.Build();

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
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorTool.Client._Imports).Assembly);

app.MapControllers();
app.Run();

