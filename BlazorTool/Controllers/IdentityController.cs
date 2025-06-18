using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

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
            var client = _httpClientFactory.CreateClient("API");
            var baseAddress = client.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
            var externalApiUrl = $"{baseAddress}/api/v1/identity/loginpassword?UserName={Uri.EscapeDataString(loginRequest.Username)}&Password={Uri.EscapeDataString(loginRequest.Password)}";

            // Basic Auth from appsettings.json (ExternalApi:)
            var basicAuthUsername = _configuration["ExternalApi:BasicAuthUsername"] ?? string.Empty;
            var basicAuthPassword = _configuration["ExternalApi:BasicAuthPassword"] ?? string.Empty;
            if (!string.IsNullOrEmpty(basicAuthUsername) && !string.IsNullOrEmpty(basicAuthPassword))
            {
                var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuthUsername}:{basicAuthPassword}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            }

            var personID = int.TryParse(_configuration["ExternalApi:PersonID"], out var id) ? id : 1;
            var personPassword = _configuration["ExternalApi:PersonPassword"] ?? loginRequest.Password;

            var body = new LoginPasswordRequest { PersonID = personID, Password = personPassword };
            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, externalApiUrl)
            {
                Content = content   
            };

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, responseString);

            // Попытка извлечь токен из ответа внешнего API
            
            JsonNode data = null;
            try
            {
                var json = JsonNode.Parse(responseString);
                data = json["data"];
            }
            catch {
            return StatusCode(500, "Failed to parse response from external API.");
            }

            var langCode = data?["langCode"]?.ToString() ?? "pl-PL";
            var token = data?["token"]?.ToString() ?? string.Empty; 
            var expires = DateTime.Now.AddHours(1);
            return Ok(new { Token = token, LangCode = langCode, Expires = expires });     
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
