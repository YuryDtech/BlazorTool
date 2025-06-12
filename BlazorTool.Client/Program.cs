using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorTool.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var token = builder.Configuration["AuthToken"] ?? string.Empty;
builder.Services.AddTelerikBlazor();
builder.Services.AddScoped<apiService>(sp => new apiService(sp.GetRequiredService<HttpClient>(), token));

builder.Services.AddScoped<AppointmentService>();

await builder.Build().RunAsync();
