using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration; // To access configuration for Basic Auth credentials

namespace BlazorTool.Controllers
{
    [Route("api/v1/other")] // Changed to "other" as per the request URL "api/v1/other/getuserslist"
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApiServiceClient _apiServiceClient;

        public PersonController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ApiServiceClient apiServiceClient)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiServiceClient = apiServiceClient;
        }

        [HttpGet("getuserslist")]
        public async Task<IActionResult> GetUsersList()
        {
            try
            {
                var basicAuthClient = _httpClientFactory.CreateClient("ExternalApiBasicAuthClient");

                // Get Basic Auth credentials from configuration
                var username = _configuration["ExternalApi:BasicAuthUsername"];
                var password = _configuration["ExternalApi:BasicAuthPassword"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return StatusCode(500, new ApiResponse<Person>
                    {
                        Data = new List<Person>(),
                        IsValid = false,
                        Errors = new() { "Basic Auth credentials (ExternalApi:BasicAuthUsername or ExternalApi:BasicAuthPassword) are not configured." }
                    });
                }

                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                basicAuthClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var persons = await _apiServiceClient.GetAllPersons(basicAuthClient);

                return Ok(new ApiResponse<Person>
                {
                    Data = persons ?? new List<Person>(),
                    IsValid = true,
                    Errors = new List<string>()
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new ApiResponse<Person>
                {
                    Data = new List<Person>(),
                    IsValid = false,
                    Errors = new List<string> { $"Error calling external API: {ex.Message}" }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Person>
                {
                    Data = new List<Person>(),
                    IsValid = false,
                    Errors = new List<string> { $"An unexpected error occurred: {ex.Message}" }
                });
            }
        }
    }
}
