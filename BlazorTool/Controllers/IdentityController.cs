using BlazorTool.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorTool.Controllers
{
    [ApiController]
    [Route("api/v1/identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public IdentityController(IHttpClientFactory httpClientFactory, IConfiguration configuration)       
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("loginpassword")]
        public async Task<IActionResult> LoginPassword([FromBody] BlazorTool.Client.Models.LoginRequest loginRequest)
        {
            var client = _httpClientFactory.CreateClient("ExternalApiBasicAuthClient");

            // Basic Auth from appsettings.json (ExternalApi:)
            var basicAuthUsername = _configuration["ExternalApi:BasicAuthUsername"] ?? string.Empty;
            var basicAuthPassword = _configuration["ExternalApi:BasicAuthPassword"] ?? string.Empty;
            if (!string.IsNullOrEmpty(basicAuthUsername) && !string.IsNullOrEmpty(basicAuthPassword))
            {
                var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuthUsername}:{basicAuthPassword}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            }
            var externalApiRelativeUrl = $"api/v1/identity/loginpassword?UserName={Uri.EscapeDataString(loginRequest.Username)}&Password={Uri.EscapeDataString(loginRequest.Password)}";
            var personID = int.TryParse(_configuration["ExternalApi:PersonID"], out var id) ? id : 1;
            var personPassword = _configuration["ExternalApi:PersonPassword"] ?? loginRequest.Password;

            var body = new LoginPasswordRequest { PersonID = personID, Password = personPassword };
            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            //TODO change to GET?
            var request = new HttpRequestMessage(HttpMethod.Post, externalApiRelativeUrl)
            {
                Content = content   
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var wrapper = await response.Content.ReadFromJsonAsync<SingleResponse<IdentityData>>();
            return Ok(wrapper);     
        }
    }

    public class LoginPasswordRequest
    {
        [JsonPropertyName("PersonID")]
        public int PersonID { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; } = default!;
    }
}
