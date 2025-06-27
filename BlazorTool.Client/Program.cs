using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Blazored.LocalStorage;
using System.Net.Http;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTelerikBlazor();
builder.Services.AddBlazoredLocalStorage();

var loginDto = new LoginRequest
{ //TODO auth page
    Username = "Romaniuk Krzysztof",//builder.Configuration["Auth:Username"]!,
    Password = "q"//builder.Configuration["Auth:Password"]!
};

builder.Services.AddScoped<AuthHeaderHandler>(sp =>
{
    var localStorageService = sp.GetRequiredService<ILocalStorageService>();
    var httpClientForAuth = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return new AuthHeaderHandler(localStorageService, httpClientForAuth, loginDto);
});

builder.Services.AddHttpClient("ServerApi", client =>
{
    var serverBaseUrl = builder.Configuration["InternalApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    client.BaseAddress = new Uri(serverBaseUrl);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped<ApiServiceClient>(sp =>
    new ApiServiceClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi")));

builder.Services.AddScoped<AppointmentService>();

builder.Services.AddScoped(sp => {
    var serverBaseUrl = builder.Configuration["InternalApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    return new HttpClient { BaseAddress = new Uri(serverBaseUrl) };
});




Console.WriteLine("======= APPLICATION (Client) STARTING...");

await builder.Build().RunAsync();

public static class AppInfo
{
    public static string Version { get; } = ThisAssembly.AssemblyInformationalVersion;
    public static string Name = "flexiCMMS alfa";
    public static string BuildDate { get; } = ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC";
}