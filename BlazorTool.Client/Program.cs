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

builder.Services.AddScoped<AuthHeaderHandler>(sp =>
{
    var userState = sp.GetRequiredService<UserState>();
    return new AuthHeaderHandler(userState);
});

var serverBaseUrl = builder.Configuration["InternalApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
if (serverBaseUrl.Contains("localhost") && !serverBaseUrl.Contains("/api"))
{
    serverBaseUrl += "api/v1/";
}

builder.Services.AddHttpClient("ServerApi", client =>
{    
    client.BaseAddress = new Uri(serverBaseUrl);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped<ApiServiceClient>(sp =>
    new ApiServiceClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"), 
    sp.GetRequiredService<UserState>()));

builder.Services.AddScoped<AppointmentService>();

builder.Services.AddScoped(sp => {
    return new HttpClient { BaseAddress = new Uri(serverBaseUrl) };
});

builder.Services.AddScoped<UserState>();

Console.WriteLine("======= APPLICATION (Client) STARTING...");

await builder.Build().RunAsync();

public static class AppInfo
{
    public static string Version { get; } = ThisAssembly.AssemblyInformationalVersion;
    public static string Name = "flexiCMMS alfa";
    public static string BuildDate { get; } = ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC";
}