using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorTool.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var token = builder.Configuration["AuthToken"] ?? string.Empty;
builder.Services.AddTelerikBlazor();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ApiServiceClient>(sp => new ApiServiceClient(sp.GetRequiredService<HttpClient>(), token));

#region DEV
//builder.Logging.SetMinimumLevel(LogLevel.Debug);
//TODO Authentication
#endregion

await builder.Build().RunAsync();
