#======Структура проекта:=====
 
[ROOT] BlazorTool
├──  BlazorTool
│        ├──  Program.cs
│        ├──  Components
│        │        ├──  App.razor
│        │        └──  _Imports.razor
│        ├──  Controllers
│        │        ├──  DeviceController.cs
│        │        └──  WoController.cs
│        ├──  Models
│        ├──  Properties
│        └──  Services
└──  BlazorTool.Client
        ├──  Program.cs
        ├──  Routes.razor
        ├──  _Imports.razor
        ├──  Layout
        │        ├──  MainLayout.razor
        │        ├──  MainLayout.razor.css
        │        ├──  NavMenu.razor
        │        └──  NavMenu.razor.css
        ├──  Models
        │        ├──  ApiResponse.cs
        │        ├──  Device.cs
        │        ├──  OrderStatus.cs
        │        └──  WorkOrder.cs
        ├──  Pages
        │        ├──  Home.razor
        │        ├──  OrdersPage.razor
        │        └──  SchedulerPage.razor
        ├──  Properties
        └──  Services
                ├──  ApiServiceClient.cs
                └──  AppointmentService.cs
===============================

# Содержимое файлов

## Файл: ..\..\..\..\BlazorTool\BlazorTool\Program.cs
```csharp
using BlazorTool.Client.Pages;
using BlazorTool.Client.Services;
using BlazorTool.Components;

var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["AuthToken"] ?? string.Empty;
//TODO Get token from API
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddTelerikBlazor();
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["API"]);
});
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("API"));
builder.Services.AddScoped<ApiServiceClient>(sp => new ApiServiceClient(sp.GetRequiredService<HttpClient>(), token));
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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorTool.Client._Imports).Assembly);
app.MapControllers();

app.Run();

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool\Components\App.razor
```
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="app.css" />
    <link rel="stylesheet" href="BlazorTool.styles.css" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet @rendermode="@RenderMode.InteractiveAuto" />

    <link rel="stylesheet" href="https://blazor.cdn.telerik.com/blazor/6.2.0/kendo-theme-bootstrap/swatches/bootstrap-urban.css" />
    <script src="https://blazor.cdn.telerik.com/blazor/6.2.0/telerik-blazor.min.js" defer></script>
</head>

<body>
    <Routes @rendermode="@RenderMode.InteractiveAuto" />
    <script src="_framework/blazor.web.js"></script>
</body>

</html>

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool\Components\_Imports.razor
```
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using BlazorTool
@using BlazorTool.Client
@using BlazorTool.Components

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool\Controllers\DeviceController.cs
```csharp
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ApiServiceClient _apiServiceClient;

        public DeviceController(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] WorkOrderQueryParameters q)
        {
            try
            {
                var data = await _apiServiceClient.GetWorkOrdersAsync(
                    deviceID: q.DeviceID,
                    workOrderID: q.WorkOrderID,
                    deviceName: q.DeviceName,
                    isDep: q.IsDep,
                    isTakenPerson: q.IsTakenPerson,
                    active: q.Active,
                    lang: q.Lang,
                    personID: q.PersonID,
                    isPlan: q.IsPlan,
                    isWithPerson: q.IsWithPerson
                );

                return Ok(new
                {
                    data,
                    isValid = true,
                    errors = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    data = Array.Empty<object>(),
                    isValid = false,
                    errors = new[] { ex.Message }
                });
            }
        }

        public class WorkOrderQueryParameters
        {
            public int DeviceID { get; set; }
            public int? WorkOrderID { get; set; }
            public string? DeviceName { get; set; }
            public bool? IsDep { get; set; }
            public bool? IsTakenPerson { get; set; }
            public bool? Active { get; set; }
            public string Lang { get; set; } = "pl-PL";
            public int? PersonID { get; set; }
            public bool? IsPlan { get; set; }
            public bool? IsWithPerson { get; set; }
        }

        // GET api/<WoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool\Controllers\WoController.cs
```csharp
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WoController : ControllerBase
    {
        private readonly ApiServiceClient _apiServiceClient;

        public WoController(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] WorkOrderQueryParameters q)
        {
            try
            {
                var data = await _apiServiceClient.GetWorkOrdersAsync(
                    deviceID: q.DeviceID,
                    workOrderID: q.WorkOrderID,
                    deviceName: q.DeviceName,
                    isDep: q.IsDep,
                    isTakenPerson: q.IsTakenPerson,
                    active: q.Active,
                    lang: q.Lang,
                    personID: q.PersonID,
                    isPlan: q.IsPlan,
                    isWithPerson: q.IsWithPerson
                );

                return Ok(new
                {
                    data,
                    isValid = true,
                    errors = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                // TODO: logging ex
                return StatusCode(500, new
                {
                    data = Array.Empty<object>(),
                    isValid = false,
                    errors = new[] { ex.Message }
                });
            }
        }

        public class WorkOrderQueryParameters
        {
            public int DeviceID { get; set; }
            public int? WorkOrderID { get; set; }
            public string? DeviceName { get; set; }
            public bool? IsDep { get; set; }
            public bool? IsTakenPerson { get; set; }
            public bool? Active { get; set; }
            public string Lang { get; set; } = "pl-PL";
            public int? PersonID { get; set; }
            public bool? IsPlan { get; set; }
            public bool? IsWithPerson { get; set; }
        }

        // GET api/<WoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Program.cs
```csharp
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

builder.Services.AddScoped<AppointmentService>();

await builder.Build().RunAsync();

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Routes.razor
```
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
</Router>

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\_Imports.razor
```
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using BlazorTool.Client
@using Telerik.Blazor
@using Telerik.Blazor.Components
@using Telerik.SvgIcons
@using Telerik.DataSource
@using Telerik.DataSource.Extensions
@using static Microsoft.AspNetCore.Components.Web.RenderMode
```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Layout\MainLayout.razor
```
@inherits LayoutComponentBase

<TelerikRootComponent>
    <div class="page">
        <div class="sidebar">
            <NavMenu />
        </div>

        <main>
            <div class="top-row px-4">
                <a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
            </div>

            <article class="content px-4">
                @Body
            </article>
        </main>
    </div>

    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
</TelerikRootComponent>
```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Layout\MainLayout.razor.css
```css
.page {
    position: relative;
    display: flex;
    flex-direction: column;
}

main {
    flex: 1;
}

.sidebar {
    background-image: linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%);
}

.top-row {
    background-color: #f7f7f7;
    border-bottom: 1px solid #d6d5d5;
    justify-content: flex-end;
    height: 3.5rem;
    display: flex;
    align-items: center;
}

    .top-row ::deep a, .top-row ::deep .btn-link {
        white-space: nowrap;
        margin-left: 1.5rem;
        text-decoration: none;
    }

    .top-row ::deep a:hover, .top-row ::deep .btn-link:hover {
        text-decoration: underline;
    }

    .top-row ::deep a:first-child {
        overflow: hidden;
        text-overflow: ellipsis;
    }

@media (max-width: 640.98px) {
    .top-row {
        justify-content: space-between;
    }

    .top-row ::deep a, .top-row ::deep .btn-link {
        margin-left: 0;
    }
}

@media (min-width: 641px) {
    .page {
        flex-direction: row;
    }

    .sidebar {
        width: 250px;
        height: 100vh;
        position: sticky;
        top: 0;
    }

    .top-row {
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .top-row.auth ::deep a:first-child {
        flex: 1;
        text-align: right;
        width: 0;
    }

    .top-row, article {
        padding-left: 2rem !important;
        padding-right: 1.5rem !important;
    }
}

#blazor-error-ui {
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

    #blazor-error-ui .dismiss {
        cursor: pointer;
        position: absolute;
        right: 0.75rem;
        top: 0.5rem;
    }

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Layout\NavMenu.razor
```
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">BlazorTool</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="navbar-nav flex-column">
        <div class="nav-item">
            <NavLink class="nav-link d-flex align-items-center px-3 py-2" href="/" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill me-2" aria-hidden="true"></span>
                <span>Main</span>
            </NavLink>
        </div>
        <div class="nav-item">
            <NavLink class="nav-link d-flex align-items-center px-3 py-2" href="orders">
                <span class="bi bi-cart-fill me-2" aria-hidden="true"></span>
                <span>Orders</span>
            </NavLink>
        </div>
        <div class="nav-item">
            <NavLink class="nav-link d-flex align-items-center px-3 py-2" href="scheduler">
                <span class="bi bi-calendar-fill me-2" aria-hidden="true"></span>
                <span>Scheduler</span>
            </NavLink>
        </div>
    </nav>
</div>

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Layout\NavMenu.razor.css
```css
.navbar-toggler {
    appearance: none;
    cursor: pointer;
    width: 3.5rem;
    height: 2.5rem;
    color: white;
    position: absolute;
    top: 0.5rem;
    right: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 30 30'%3e%3cpath stroke='rgba%28255, 255, 255, 0.55%29' stroke-linecap='round' stroke-miterlimit='10' stroke-width='2' d='M4 7h22M4 15h22M4 23h22'/%3e%3c/svg%3e") no-repeat center/1.75rem rgba(255, 255, 255, 0.1);
}

.navbar-toggler:checked {
    background-color: rgba(255, 255, 255, 0.5);
}

.top-row {
    height: 3.5rem;
    background-color: rgba(0,0,0,0.4);
}

.navbar-brand {
    font-size: 1.1rem;
}

.bi {
    display: inline-block;
    position: relative;
    width: 1.25rem;
    height: 1.25rem;
    margin-right: 0.75rem;
    top: -1px;
    background-size: cover;
}

.bi-house-door-fill {
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='white' class='bi bi-house-door-fill' viewBox='0 0 16 16'%3E%3Cpath d='M6.5 14.5v-3.505c0-.245.25-.495.5-.495h2c.25 0 .5.25.5.5v3.5a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5v-7a.5.5 0 0 0-.146-.354L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.354 1.146a.5.5 0 0 0-.708 0l-6 6A.5.5 0 0 0 1.5 7.5v7a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5Z'/%3E%3C/svg%3E");
}

.bi-plus-square-fill {
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='white' class='bi bi-plus-square-fill' viewBox='0 0 16 16'%3E%3Cpath d='M2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2zm6.5 4.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3a.5.5 0 0 1 1 0z'/%3E%3C/svg%3E");
}

.bi-list-nested {
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='white' class='bi bi-list-nested' viewBox='0 0 16 16'%3E%3Cpath fill-rule='evenodd' d='M4.5 11.5A.5.5 0 0 1 5 11h10a.5.5 0 0 1 0 1H5a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 3 7h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 1 3h10a.5.5 0 0 1 0 1H1a.5.5 0 0 1-.5-.5z'/%3E%3C/svg%3E");
}

.nav-item {
    font-size: 0.9rem;
    padding-bottom: 0.5rem;
}

    .nav-item:first-of-type {
        padding-top: 1rem;
    }

    .nav-item:last-of-type {
        padding-bottom: 1rem;
    }

    .nav-item ::deep a {
        color: #d7d7d7;
        border-radius: 4px;
        height: 3rem;
        display: flex;
        align-items: center;
        line-height: 3rem;
    }

.nav-item ::deep a.active {
    background-color: rgba(255,255,255,0.37);
    color: white;
}

.nav-item ::deep a:hover {
    background-color: rgba(255,255,255,0.1);
    color: white;
}

.nav-scrollable {
    display: none;
}

.navbar-toggler:checked ~ .nav-scrollable {
    display: block;
}

@media (min-width: 641px) {
    .navbar-toggler {
        display: none;
    }

    .nav-scrollable {
        /* Never collapse the sidebar for wide screens */
        display: block;

        /* Allow sidebar to scroll for tall menus */
        height: calc(100vh - 3.5rem);
        overflow-y: auto;
    }
}

.nav-link:hover {
    background-color: rgba(0, 0, 0, 0.1);
    border-radius: 0.375rem;
}

.nav-link.active {
    background-color: var(--bs-primary);
    color: white !important;
    border-radius: 0.375rem;
}

.nav-scrollable {
    max-height: 100vh;
    overflow-y: auto;
}
```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Models\ApiResponse.cs
```csharp
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Models\Device.cs
```csharp
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class Device
    {
        [JsonPropertyName("machineID")]
        public int MachineID { get; set; }

        [JsonPropertyName("assetNo")]
        public string? AssetNo { get; set; }

        [JsonPropertyName("assetNoShort")]
        public string? AssetNoShort { get; set; }

        [JsonPropertyName("deviceCategory")]
        public string? DeviceCategory { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("serialNo")]
        public string? SerialNo { get; set; }

        [JsonPropertyName("stateID")]
        public int? StateID { get; set; }

        [JsonPropertyName("categoryID")]
        public int? CategoryID { get; set; }

        [JsonPropertyName("documentationPath")]
        public string? DocumentationPath { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("locationRequired")]
        public bool? LocationRequired { get; set; }

        [JsonPropertyName("locationName")]
        public string? LocationName { get; set; }

        [JsonPropertyName("place")]
        public string? Place { get; set; }

        [JsonPropertyName("isCritical")]
        public bool? IsCritical { get; set; }

        [JsonPropertyName("setName")]
        public string? SetName { get; set; }

        [JsonPropertyName("setID")]
        public int? SetID { get; set; }

        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("cycle")]
        public object? Cycle { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Models\OrderStatus.cs
```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class OrderStatus
    {
        [JsonPropertyName("machineCategoryID")]
        public int? MachineCategoryID { get; set; }

        [JsonPropertyName("is_Default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("listType")]
        public int ListType { get; set; }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Models\WorkOrder.cs
```csharp
using System;
using System.Text.Json.Serialization;

namespace BlazorTool.Client.Models
{
    public class WorkOrder
    {
        [JsonPropertyName("workOrderID")]
        public int WorkOrderID { get; set; }

        [JsonPropertyName("machineID")]
        public int MachineID { get; set; }

        [JsonPropertyName("asset_No")]
        public string? AssetNo { get; set; }

        [JsonPropertyName("device_Category")]
        public string? DeviceCategory { get; set; }

        [JsonPropertyName("wO_Desc")]
        public string? WODesc { get; set; }

        [JsonPropertyName("wO_Category")]
        public string? WOCategory { get; set; }

        [JsonPropertyName("wO_State")]
        public string? WOState { get; set; }

        [JsonPropertyName("wO_Reason")]
        public string? WOReason { get; set; }

        [JsonPropertyName("add_Date")]
        public DateTime AddDate { get; set; }

        [JsonPropertyName("take_Date")]
        public DateTime? TakeDate { get; set; }

        [JsonPropertyName("close_Date")]
        public DateTime CloseDate { get; set; }

        [JsonPropertyName("cost")]
        public decimal? Cost { get; set; }

        [JsonPropertyName("take_Persons")]
        public string? TakePersons { get; set; }

        [JsonPropertyName("planID")]
        public int? PlanID { get; set; }

        [JsonPropertyName("state_Color")]
        public string StateColor { get; set; }

        [JsonPropertyName("mod_Date")]
        public DateTime ModDate { get; set; }

        [JsonPropertyName("mod_Person")]
        public string? ModPerson { get; set; }

        [JsonPropertyName("wO_Level")]
        public string? WOLevel { get; set; }

        [JsonPropertyName("levelID")]
        public int? LevelID { get; set; }

        [JsonPropertyName("act_Count")]
        public int? ActCount { get; set; }

        [JsonPropertyName("dep_Name")]
        public string DepName { get; set; }

        [JsonPropertyName("assigned_Person")]
        public string? AssignedPerson { get; set; }

        [JsonPropertyName("file_Count")]
        public int? FileCount { get; set; }

        [JsonPropertyName("part_Count")]
        public int? PartCount { get; set; }

        [JsonPropertyName("plan_Act_Count")]
        public int PlanActCount { get; set; }

        [JsonPropertyName("start_Date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_Date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("is_Scheduled_Planned")]
        public bool IsScheduledPlanned { get; set; }

        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("reasonID")]
        public int? ReasonID { get; set; }

        [JsonPropertyName("ineffective_Count")]
        public int? IneffectiveCount { get; set; }

        [JsonPropertyName("person_Take_Date")]
        public DateTime? PersonTakeDate { get; set; }

        [JsonPropertyName("deviceCategoryID")]
        public int? DeviceCategoryID { get; set; }
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Pages\Home.razor
```
@page "/"
@inject NavigationManager NavigationManager
<TelerikButton ThemeColor="@ThemeConstants.Button.ThemeColor.Warning" @onclick="OnOrderClick">ORDERS</TelerikButton>
<TelerikButton ThemeColor="@ThemeConstants.Button.ThemeColor.Success" @onclick="OnSchedulerClick">SCHEDULER</TelerikButton>
@code {

    private void OnOrderClick()
    {
        // Navigate to the Orders page
        NavigationManager.NavigateTo("/orders");
    }

    private void OnSchedulerClick()
    {
        NavigationManager.NavigateTo("/scheduler");
    }
}
```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Pages\OrdersPage.razor
```
@page "/orders"

@using BlazorTool.Client.Models
@using BlazorTool.Client.Services
@using System.Text
@inject BlazorTool.Client.Services.ApiServiceClient apiService
@rowStyles

<label>Devices:</label>
@if (isDevicesLoading)
{
    <div>loading device list...</div>
}else{
    <TelerikListBox Data="@devices"
                    SelectionMode="@ListBoxSelectionMode.Single"
                    TextField="AssetNo"
                    @bind-SelectedItems="@selectedDevices"
                    Width="300px"
    >
        <ListBoxToolBarSettings>
            <ListBoxToolBar Visible="false" />
        </ListBoxToolBarSettings>
    </TelerikListBox>
    <TelerikFilter  ValueChanged="@OnValueChanged">
        <FilterFields>
            <FilterField Name="@nameof(WorkOrder.WOState)" Type="typeof(string)" />
            <FilterField Name="@nameof(WorkOrder.DepName)" Type="typeof(string)" />
            <FilterField Name="@nameof(WorkOrder.WOLevel)" Type="@typeof(string)" />
            <FilterField Name="@nameof(WorkOrder.WOReason)" Type="typeof(string)"/>
            <FilterField Name="@nameof(WorkOrder.ModPerson)" Type="typeof(string)"/>
            <FilterField Name="@nameof(WorkOrder.AddDate)" Type="typeof(DateTime)"/>
            <FilterField Name="@nameof(WorkOrder.CloseDate)" Type="typeof(DateTime)"/>
        </FilterFields>
    </TelerikFilter>
}

<h3>Orders</h3>
@if (isOrdersLoading)
{
    <div>loading order list...</div>
}
else
{
    <TelerikGrid Data="@OrdersForGrid"
                 Id="Grid1"
                 RowDraggable="true"
                 @ref="@GridRef"
                 Sortable="true"
                 Pageable="true"
                 PageSize="20"
                 OnRowRender="@OnRowRenderHandler"
                     >
        <GridSettings>
            <GridRowDraggableSettings DragClueField="@nameof(WorkOrder.WorkOrderID)"></GridRowDraggableSettings>
        </GridSettings>
        <GridColumns>
            <GridColumn Field="@nameof(WorkOrder.WorkOrderID)" Editable="false" Visible="false"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.DepName)" Title="DepName"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.DeviceCategory)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.WOReason)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.ModPerson)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.WOState)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.WOLevel)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.AssignedPerson)" Title="AssignedPerson"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.AddDate)"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.StartDate)" Title="Start Date"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.EndDate)" Title="End Date"></GridColumn>
            <GridColumn Field="@nameof(WorkOrder.CloseDate)"></GridColumn>
        </GridColumns>
    </TelerikGrid>
}


@code {
    private bool isOrdersLoading = false;
    private bool isDevicesLoading = false;
    private List<WorkOrder> OrdersForGrid = new List<WorkOrder>();
    private IEnumerable<Device> selectedDevices = new List<Device>();
    private MarkupString rowStyles = new MarkupString();
    private List<Device> devices = new List<Device>();
    public TelerikGrid<WorkOrder> GridRef { get; set; }
    public CompositeFilterDescriptor Value { get; set; } = new CompositeFilterDescriptor();

    protected override async Task OnInitializedAsync()
    {
        isDevicesLoading = true;
        StateHasChanged();
        devices = await apiService.GetAllDevicesCachedAsync();
        isDevicesLoading = false;
        StateHasChanged();
        await LoadData();
    }

    private async Task LoadData()
    {
        isOrdersLoading = true;
        Value = GetCompositeFilterDescriptor();
        StateHasChanged();
        if (devices == null || devices.Count == 0)
        {
            return; // No devices available, exit early
        }

        if (selectedDevices == null || selectedDevices.Count() == 0 && devices.Count > 0)
        {
            selectedDevices = new List<Device>() { devices.FirstOrDefault() };
        }
        OrdersForGrid = await apiService.GetWorkOrdersCachedAsync(selectedDevices);
        rowStyles = RenderStatesStyle(OrdersForGrid);
        isOrdersLoading = false;
        StateHasChanged();
    }

    private CompositeFilterDescriptor GetCompositeFilterDescriptor()
    {
       return new CompositeFilterDescriptor()
        {
            FilterDescriptors = new FilterDescriptorCollection
        {
            new FilterDescriptor
            {
                Member = "WOState",
                MemberType = typeof(string),
                Value = "W trakcie realizacji",
                Operator = FilterOperator.IsEqualTo
            },
            new FilterDescriptor
            {
                Member = "dep_Name",
                MemberType = typeof(string),
                Value = "UR",
                Operator = FilterOperator.IsEqualTo
            }
                      
        }
        };
    }
    void OnValueChanged(CompositeFilterDescriptor filter)
    {
        Value = filter;
        ApplyFilter(Value);
    }

    void ApplyFilter(CompositeFilterDescriptor filter)
    {
        var dataSourceRequest = new DataSourceRequest { Filters = new List<IFilterDescriptor> { filter } };

        var datasourceResult = OrdersForGrid.ToDataSourceResult(dataSourceRequest);

        OrdersForGrid = datasourceResult.Data.Cast<WorkOrder>().ToList();
    }

    void OnRowRenderHandler(GridRowRenderEventArgs args)
    {
        var item = args.Item as WorkOrder;

        args.Class += "state-" + item?.StateColor.TrimStart('#');
    }

    private MarkupString RenderStatesStyle(List<WorkOrder> orders)
    {
        var uniqueColors = orders
           .Select(o => o.StateColor)
           .Where(c => !string.IsNullOrWhiteSpace(c))
           .Distinct();

        var sb = new StringBuilder();
        sb.AppendLine("<style>");
        foreach (var color in uniqueColors)
        {
            var safeName = color.TrimStart('#');
            sb.AppendLine($".k-grid .k-master-row.state-{safeName} {{");
            sb.AppendLine($"    background-color: {color};");
            sb.AppendLine("}");
        }
        sb.AppendLine("</style>");

        return new MarkupString(sb.ToString());
    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Pages\SchedulerPage.razor
```
@page "/scheduler"

@using BlazorTool.Client.Models
@using BlazorTool.Client.Services
@using System.Text
@inject BlazorTool.Client.Services.ApiServiceClient apiService
@rowStyles

<h3>Scheduler</h3>
<div style="display: flex; gap: 10px;">
    <div style="flex: 1;">
        <TelerikScheduler Data="@AppointmentsScheduler"
                          @bind-Date="@SchedulerStartDate"
                          @bind-View="@SchedulerCurrentView"
                          @ref="@SchedulerRef"
                          OnUpdate="@UpdateAppointment"
                          OnCreate="@AddAppointment"
                          OnDelete="@DeleteAppointment"
                          ConfirmDelete="true"
                          AllowCreate="true"
                          AllowDelete="true"
                          AllowUpdate="true"
                          Id="Scheduler1">
            <SchedulerViews>
                <SchedulerDayView StartTime="@DayStart" EndTime="@DayEnd" />
                <SchedulerWeekView StartTime="@DayStart" EndTime="@DayEnd" />
                <SchedulerMonthView />
                <SchedulerTimelineView ColumnWidth=30 NumberOfDays=2 StartTime="@DayStart" EndTime="@DayEnd" />
            </SchedulerViews>
        </TelerikScheduler>
    </div>
    <div style="flex: 1;">
        @if (isOrdersLoading)
        {
            <div>data loading...</div>
        }
        else
        {
            <TelerikGrid Data="@OrdersForGrid"
                     Id="Grid1"
                     
                     RowDraggable="true"
                     @ref="@GridRef"
                         Sortable="true"
                         Pageable="true"
                         PageSize="12"
                         OnRowRender="@OnRowRenderHandler"
                     OnRowDrop="@((GridRowDropEventArgs<WorkOrder> args) => GridRowDrop(args))">
            <GridSettings>
                <GridRowDraggableSettings DragClueField="@nameof(WorkOrder.WorkOrderID)"></GridRowDraggableSettings>
            </GridSettings>
            <GridColumns>
                <GridColumn Field="@nameof(WorkOrder.WorkOrderID)" Editable="false" Visible="false"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.DepName)" Title="DepName"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.DeviceCategory)"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.WOReason)"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.ModPerson)"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.WOState)"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.WOLevel)"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.AssignedPerson)" Title="AssignedPerson"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.AddDate)" ></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.StartDate)" Title="Start Date"></GridColumn>
                <GridColumn Field="@nameof(WorkOrder.EndDate)" Title="End Date"></GridColumn> 
                <GridColumn Field="@nameof(WorkOrder.CloseDate)"></GridColumn>
            </GridColumns>
        </TelerikGrid>
        }
        
    </div>
</div>

@code {
    private DateTime DayStart { get; set; } = DateTime.Today.AddHours(7);
    private DateTime DayEnd { get; set; } = DateTime.Today.AddHours(18);
    private DateTime SchedulerStartDate { get; set; } = DateTime.Now;
    private SchedulerView SchedulerCurrentView { get; set; } = SchedulerView.Week;
    private List<SchedulerAppointment> AppointmentsScheduler = new List<SchedulerAppointment>();
    private List<WorkOrder> OrdersForGrid = new List<WorkOrder>();
    public TelerikGrid<WorkOrder> GridRef { get; set; }
    public TelerikScheduler<SchedulerAppointment> SchedulerRef { get; set; }
    public AppointmentService appointmentServiceScheduler = new AppointmentService();
    private bool isOrdersLoading = false;
    private List<Device> devices = new List<Device>();
    private Device selectedDevice = new Device();
    private MarkupString rowStyles = new MarkupString();

    public class SchedulerAppointment
    {
        public string Title { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        AppointmentsScheduler = appointmentServiceScheduler.GetAllAppointments();

        isOrdersLoading = true;
        StateHasChanged(); 
        devices = await apiService.GetAllDevicesCachedAsync(); 
        if (selectedDevice == null && devices.Count > 0)
        {
            selectedDevice = devices.First();
        }
        else if (selectedDevice != null && !devices.Any(d => d.MachineID == selectedDevice.MachineID))
        {
            selectedDevice = devices.First();
        }
        OrdersForGrid = await apiService.GetWorkOrdersCachedAsync(selectedDevice?.MachineID ?? 1) ; 
        rowStyles = RenderStatesStyle(OrdersForGrid);
        isOrdersLoading = false;
        StateHasChanged(); 
    }

    #region Grid
    void OnRowRenderHandler(GridRowRenderEventArgs args)
    {
        var item = args.Item as WorkOrder;

        args.Class += "state-" + item.StateColor.TrimStart('#'); 
    }

    private MarkupString RenderStatesStyle(List<WorkOrder> orders)
    {
        var uniqueColors = orders
           .Select(o => o.StateColor)
           .Where(c => !string.IsNullOrWhiteSpace(c))
           .Distinct();

        var sb = new StringBuilder();
        sb.AppendLine("<style>");
        foreach (var color in uniqueColors)
        {
            var safeName = color.TrimStart('#');
            sb.AppendLine($".k-grid .k-master-row.state-{safeName} {{");
            sb.AppendLine($"    background-color: {color};");
            sb.AppendLine("}");
        }
        sb.AppendLine("</style>");

        return new MarkupString(sb.ToString());
    }
    #endregion
    private void GridRowDrop(GridRowDropEventArgs<WorkOrder> args)
    {
        // foreach (var item in args.Items)
        // {
        //     appointmentServiceGrid.DeleteAppointment(item.Id);
        //     AppointmentsGrid = appointmentServiceGrid.GetAllAppointments();
        // }

        // if (args.DestinationComponentId == "Scheduler1")
        // {
        //     foreach (var item in args.Items)
        //     {
        //         DropAppointment(args.DestinationIndex, item);
        //     }

        //     SchedulerRef.Rebind();
        // }
        // else if (args.DestinationComponentId == "Grid1")
        // {
        //     InsertItemsIntoGrid(args.Items, args.DestinationItem, args.DropPosition);
        // }
    }

    private void DropAppointment(string index, SchedulerAppointment item)
    {
        var slot = SchedulerRef.GetTimeSlotFromDropIndex(index);

        var appointment = item;
        appointment.Start = slot.Start;
        appointment.IsAllDay = slot.IsAllDay;
        appointment.End = slot.End;

        appointmentServiceScheduler.AddAppointment(appointment);

    }

    private void InsertItemsIntoGrid(IEnumerable<WorkOrder> items, WorkOrder destinationItem, GridRowDropPosition dropPosition)
    {
        var destinationIndex = 0;
        if (destinationItem != null)
        {
            destinationIndex = OrdersForGrid.IndexOf(destinationItem);
            if (dropPosition == GridRowDropPosition.After)
            {
                destinationIndex += 1;
            }
        }

        OrdersForGrid.InsertRange(destinationIndex, items);
    }

    async Task UpdateAppointment(SchedulerUpdateEventArgs args)
    {
        appointmentServiceScheduler.UpdateAppointment((SchedulerAppointment)args.Item);
        // Rebind the scheduler to reflect the changes
        SchedulerRef.Rebind();
    }

    async Task AddAppointment(SchedulerCreateEventArgs args)
    {
        appointmentServiceScheduler.AddAppointment((SchedulerAppointment)args.Item);
        SchedulerRef.Rebind();
    }

    async Task DeleteAppointment(SchedulerDeleteEventArgs args)
    {
        appointmentServiceScheduler.DeleteAppointment(((SchedulerAppointment)args.Item).Id);
        SchedulerRef.Rebind();
    }


}


```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Services\ApiServiceClient.cs
```csharp
using BlazorTool.Client.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
namespace BlazorTool.Client.Services

{
    public class ApiServiceClient
    {
        private readonly HttpClient _http;
        private readonly string _token;
        private Dictionary<int, List<WorkOrder>> _workOrdersCache = new Dictionary<int, List<WorkOrder>>();
        private List<Device> _devicesCache = new List<Device>();
        public ApiServiceClient(HttpClient http, string token)
        {
            _http = http;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _token = token;
        }

        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(int deviceId)
       {
            if (!_workOrdersCache.TryGetValue(deviceId, out var list)
                || list == null
                || list.Count == 0)
            {
               var fresh = await GetWorkOrdersAsync(deviceId);
                _workOrdersCache[deviceId] = fresh;
                return fresh;
            }
            return list;
        }

        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(IEnumerable<Device> devices)
        {
            var result = new List<WorkOrder>();
            if (devices == null || devices.Count() == 0)
            {
                return result;
            }
            foreach (var device in devices)
            {
                if (!_workOrdersCache.ContainsKey(device.MachineID))
                {
                    var fresh = await GetWorkOrdersAsync(device.MachineID);
                    _workOrdersCache.Add(device.MachineID, fresh);
                }
                else
                {
                    result.AddRange(_workOrdersCache[device.MachineID]);
                }
            }
            return result;
        }
        

        public async Task<List<WorkOrder>> GetWorkOrdersAsync(
                                        int deviceID,
                                        int? workOrderID = null,
                                        string? deviceName = null,
                                        bool? isDep = null,
                                        bool? isTakenPerson = null,
                                        bool? active = null,
                                        string lang = "pl-PL",
                                        int? personID = null,
                                        bool? isPlan = null,
                                        bool? isWithPerson = null)
        {
            var qp = new List<string>();
            qp.Add($"DeviceID={deviceID}");
            if (workOrderID.HasValue) qp.Add($"WorkOrderID={workOrderID.Value}");
            if (!string.IsNullOrWhiteSpace(deviceName)) qp.Add($"DeviceName={Uri.EscapeDataString(deviceName)}");
            if (isDep.HasValue) qp.Add($"IsDep={isDep.Value}");
            if (isTakenPerson.HasValue) qp.Add($"IsTakenPerson={isTakenPerson.Value}");
            if (active.HasValue) qp.Add($"Active={active.Value}");
            if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (personID.HasValue) qp.Add($"PersonID={personID.Value}");
            if (isPlan.HasValue) qp.Add($"IsPlan={isPlan.Value}");
            if (isWithPerson.HasValue) qp.Add($"IsWithPerson={isWithPerson.Value}");

            var url = "api/v1/wo/getlist?" + string.Join("&", qp);

            var wrapper = await _http.GetFromJsonAsync<ApiResponse<WorkOrder>>(url);
            Debug.Print("\n");
            Debug.Print("= = = = = = = = = = response WorkOrder.Count: " + wrapper?.Data.Count.ToString());
            Debug.Print("\n");
            var result = wrapper?.Data ?? new List<WorkOrder>();
            //if (_workOrdersCache.ContainsKey(deviceID))
            //    _workOrdersCache[deviceID] = result;
            //else 
            //    _workOrdersCache.Add(deviceID, result);
            return result;
        }

        public async Task<List<OrderStatus>> GetOrderStatusesAsync(
                                        int personID,
                                        int? deviceCategoryID = null,
                                        string lang = "pl-PL",
                                        bool? isEdit = null)
        {
                var qs = new List<string>
            {
                $"DeviceCategoryID={(deviceCategoryID?.ToString() ?? "")}",
                $"PersonID={personID}",
                $"Lang={Uri.EscapeDataString(lang)}",
                $"IsEdit={(isEdit?.ToString() ?? "")}"
            };

            var url = "api/v1/wo/getdict?" + string.Join("&", qs);
            var wrapper = await _http.GetFromJsonAsync<ApiResponse<OrderStatus>>(url);
            Debug.Print("\n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
            return wrapper?.Data ?? new List<OrderStatus>();
        }

        public async Task<List<Device>> GetAllDevicesCachedAsync()
        {
            if (!_devicesCache.Any())
            {
                _devicesCache = await GetDevicesAsync();
            }                
            
            return _devicesCache;
        }

        private async Task<List<Device>> GetDevicesAsync(
                                        string lang = "pl-pl",
                                        string? name = null,
                                        int? categoryID = null,
                                        bool? isSet = null,
                                        IEnumerable<int>? machineIDs = null)
        {
            var qp = new List<string>();
            if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (!string.IsNullOrWhiteSpace(name)) qp.Add($"Name={Uri.EscapeDataString(name)}");
            if (categoryID.HasValue) qp.Add($"CategoryID={categoryID.Value}");
            if (isSet.HasValue) qp.Add($"IsSet={isSet.Value}");
            if (machineIDs != null)
                foreach (var id in machineIDs)
                    qp.Add($"MachineIDs={id}");
            Debug.Print($"name:{name}\nMachineIDs.count={machineIDs?.Count()}");
            var url = "api/v1/device/getlist" + (qp.Count > 0 ? "?" + string.Join("&", qp) : "");
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Debug.Print("\n= = = = = = = = =Devices response error: " + response.ReasonPhrase + "\n");
                return new List<Device>();
            }
            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Device>>();

            Debug.Print("\n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
            
            return wrapper?.Data ?? new List<Device>();
        }

    }
}

```

## Файл: ..\..\..\..\BlazorTool\BlazorTool.Client\Services\AppointmentService.cs
```csharp
using static BlazorTool.Client.Pages.SchedulerPage;
namespace BlazorTool.Client.Services

{
    public class AppointmentService
    {
        private List<SchedulerAppointment> _appointments;
        public AppointmentService()
        {
            _appointments = GetAppointments();
        }
        public List<SchedulerAppointment> GetAllAppointments()
        {
            return _appointments;
        }
        public SchedulerAppointment GetAppointmentById(string id)
        {
            return _appointments.FirstOrDefault(x => x.Id == id);
        }

        public void AddAppointment(SchedulerAppointment appointment)
        {
            appointment.Id = Guid.NewGuid().ToString();
            _appointments.Add(appointment);
        }
        public void UpdateAppointment(SchedulerAppointment appointment)
        {
            var existingAppointment = _appointments.FirstOrDefault(x => x.Id == appointment.Id);
            if (existingAppointment != null)
            {
                existingAppointment.Title = appointment.Title;
                existingAppointment.Start = appointment.Start;
                existingAppointment.End = appointment.End;
                existingAppointment.Description = appointment.Description;
                existingAppointment.IsAllDay = appointment.IsAllDay;
            }
        }
        public void DeleteAppointment(string id)
        {
            var appointment = _appointments.FirstOrDefault(x => x.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
        }

        public void DeleteAllAppointments()
        {
            _appointments.Clear();
        }


        private List<SchedulerAppointment> GetAppointments()
        {
            var appoi = new List<SchedulerAppointment>()
        {
            new SchedulerAppointment
            {
                Title = "Daily Stand-up (or Sit-down)",
                Start = new DateTime(2025, 6, 1, 9, 0, 0),
                End = new DateTime(2025, 6, 1, 9, 30, 0),
                Description = "Daily team sync-up meeting."
            },
            new SchedulerAppointment
            {
                Title = "Coffee Machine Debugging Session",
                Start = new DateTime(2025, 6, 1, 10, 15, 0),
                End = new DateTime(2025, 6, 1, 10, 45, 0),
                Description = "Discussing coffee-related issues."
            },
            new SchedulerAppointment
            {
                Title = "Rubber Duck Programming Hour",
                Start = new DateTime(2025, 6, 2, 11, 0, 0),
                End = new DateTime(2025, 6, 2, 12, 0, 0),
                Description = "Time to talk to your rubber duck."
            },
            new SchedulerAppointment
            {
                Title = "404 Error: Meeting Not Found",
                Start = new DateTime(2025, 6, 2, 14, 0, 0),
                End = new DateTime(2025, 6, 2, 15, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Ctrl+Alt+Delete Your Problems",
                Start = new DateTime(2025, 6, 3, 9, 30, 0),
                End = new DateTime(2025, 6, 3, 10, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Code Review: Judging Other People's Code",
                Start = new DateTime(2025, 6, 3, 13, 0, 0),
                End = new DateTime(2025, 6, 3, 14, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Stack Overflow Research Time",
                Start = new DateTime(2025, 6, 4, 10, 0, 0),
                End = new DateTime(2025, 6, 4, 11, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Agile Sprint Planning (More Like Marathon)",
                Start = new DateTime(2025, 6, 4, 15, 0, 0),
                End = new DateTime(2025, 6, 4, 17, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Debugging: It's Not a Bug, It's a Feature",
                Start = new DateTime(2025, 6, 5, 9, 0, 0),
                End = new DateTime(2025, 6, 5, 10, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Team Building: Escape Room (Debug Edition)",
                Start = new DateTime(2025, 6, 5, 16, 0, 0),
                End = new DateTime(2025, 6, 5, 18, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Weekend Hackathon: Pizza and Coding",
                IsAllDay = true,
                Start = new DateTime(2025, 6, 7),
                End = new DateTime(2025, 6, 8)
            },
            new SchedulerAppointment
            {
                Title = "Monday Morning Motivation Meeting",
                Start = new DateTime(2025, 6, 9, 8, 0, 0),
                End = new DateTime(2025, 6, 9, 9, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Git Commit Therapy Session",
                Start = new DateTime(2025, 6, 9, 11, 30, 0),
                End = new DateTime(2025, 6, 9, 12, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Scrum Master's Stand-up Comedy",
                Start = new DateTime(2025, 6, 10, 14, 0, 0),
                End = new DateTime(2025, 6, 10, 14, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "CSS Zen Garden Meditation",
                Start = new DateTime(2025, 6, 10, 16, 0, 0),
                End = new DateTime(2025, 6, 10, 17, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Database Relationship Counseling",
                Start = new DateTime(2025, 6, 11, 10, 0, 0),
                End = new DateTime(2025, 6, 11, 11, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "API Documentation: The Novel Nobody Reads",
                Start = new DateTime(2025, 6, 11, 13, 0, 0),
                End = new DateTime(2025, 6, 11, 15, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Localhost Lunch Break",
                Start = new DateTime(2025, 6, 12, 12, 0, 0),
                End = new DateTime(2025, 6, 12, 13, 0, 0)
            },
            new SchedulerAppointment
            {
                Title = "Deployment Day: Crossing Fingers and Toes",
                Start = new DateTime(2025, 6, 12, 15, 0, 0),
                End = new DateTime(2025, 6, 12, 17, 30, 0)
            },
            new SchedulerAppointment
            {
                Title = "Tech Conference: Learning New Ways to Overthink",
                IsAllDay = true,
                Start = new DateTime(2025, 6, 13),
                End = new DateTime(2025, 6, 15)
            }

        };
            appoi.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
            });
            return appoi;
        }
    }
}

```
