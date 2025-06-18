using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTelerikBlazor();
builder.Services.AddBlazoredLocalStorage();
var localStorage = builder.Services.BuildServiceProvider().GetRequiredService<ILocalStorageService>();
var loginClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var loginDto = new LoginRequest
{ //TODO auth page
    Username = "Romaniuk Krzysztof",//builder.Configuration["Auth:Username"]!,
    Password = "q"//builder.Configuration["Auth:Password"]!
};

string token = await GetToken(localStorage, loginClient, loginDto);

if (string.IsNullOrEmpty(token))
    Debug.Print("======= CLIENT: Failed to get token");

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    return client;
});

builder.Services.AddScoped<ApiServiceClient>(sp =>
    new ApiServiceClient(sp.GetRequiredService<HttpClient>(), token));

await builder.Build().RunAsync();

static async Task<string> GetToken(ILocalStorageService localStorage, HttpClient loginClient, LoginRequest loginDto)
{ //TODO select storage(local, session, Extension,Cookies, etc.)
    string token = string.Empty;
    var tokenJson = await localStorage.GetItemAsStringAsync("token");
    if (!string.IsNullOrEmpty(tokenJson))
    {
        var tokenResult = JsonSerializer.Deserialize<TokenResponse>(tokenJson);
        if (tokenResult?.Expires > DateTime.UtcNow)
        {
            token = tokenResult?.Token ?? string.Empty;
        }
        else
        {
            var loginResponse = await loginClient.PostAsJsonAsync("api/v1/identity/loginpassword", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            tokenResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
            token = tokenResult?.Token ?? string.Empty;
            await localStorage.SetItemAsStringAsync("token", JsonSerializer.Serialize(tokenResult));
        }
    }
    else
    {
        var loginResponse = await loginClient.PostAsJsonAsync("api/v1/identity/loginpassword", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var tokenResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        token = tokenResult?.Token ?? string.Empty;
        await localStorage.SetItemAsStringAsync("token", JsonSerializer.Serialize(tokenResult));
    }

    return token;
}

public static class AppInfo
{
    public static string Version { get; } = ThisAssembly.AssemblyInformationalVersion;
    public static string Name = "flexiCMMS alfa";
    public static string BuildDate { get; } = ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC";
}