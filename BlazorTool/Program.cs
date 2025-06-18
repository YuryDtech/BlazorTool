using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using BlazorTool.Components;
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

// 2) ???????????? HttpClient ??? ???????? Identity API
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["API"]!);
});

// 3) ??????????????? ???????
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddTelerikBlazor();

// 4) ???????? ????? ??? ??????
using var provider = builder.Services.BuildServiceProvider();
var cache = provider.GetRequiredService<IMemoryCache>();
var httpFactory = provider.GetRequiredService<IHttpClientFactory>();

var loginDto = new LoginRequest
{ //TODO auth page
    Username = "Romaniuk Krzysztof",//builder.Configuration["Auth:Username"]!,
    Password = "q"//builder.Configuration["Auth:Password"]!
};

string token = await GetTokenAsync(cache, httpFactory, builder.Configuration, loginDto);
if (string.IsNullOrEmpty(token))
    Debug.Print("======= SERVER: token is empty");

// 5) ????????????? HttpClient ??? ????????? API ? JWT
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["API"]!);
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
builder.Services.AddScoped<ApiServiceClient>(sp =>
    new ApiServiceClient(sp.GetRequiredService<HttpClient>(), token));

var app = builder.Build();

// Middleware
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

// ??????????????? ????? ??? ????????? ?????? ?? ???????? API ? ???????????
static async Task<string> GetTokenAsync(
    IMemoryCache cache,
    IHttpClientFactory httpFactory,
    IConfiguration config,
    LoginRequest loginDto)
{
    const string cacheKey = "ApiJwtToken";
    if (cache.TryGetValue(cacheKey, out string cachedToken))
    {
        return cachedToken;
    }

    var client = httpFactory.CreateClient("API");

    // ????????? Basic ??????????? ?? ????????
    var basicUser = config["ExternalApi:BasicAuthUsername"]!;
    var basicPass = config["ExternalApi:BasicAuthPassword"]!;
    var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicUser}:{basicPass}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

    // ???????? URL ? query-???????????
    var url = $"api/v1/identity/loginpassword?UserName={Uri.EscapeDataString(loginDto.Username)}&Password={Uri.EscapeDataString(loginDto.Password)}";

    // ???? ??????? ???????? ???????? API
    var personId = int.Parse(config["ExternalApi:PersonID"]!);
    var personPass = config["ExternalApi:PersonPassword"] ?? loginDto.Password;
    var body = new { PersonID = personId, Password = personPass };

    var request = new HttpRequestMessage(HttpMethod.Post, url)
    {
        Content = JsonContent.Create(body)
    };

    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();

    // ?????? ????? ?? ???????? API
    var wrapper = await response.Content.ReadFromJsonAsync<SingleResponse<IdentityData>>();
    var token = wrapper?.Data?.Token ?? string.Empty;
    var expires = DateTime.UtcNow.AddHours(1);

    // ???????? ????? ?? ????? ?????????
    var ttl = expires - DateTime.UtcNow;
    cache.Set(cacheKey, token, ttl);

    return token;
}
