using BlazorTool.Client.Models; 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTool.Services 
{
    public class ServerAuthTokenService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly LoginRequest _loginDto;

        private const string CacheKey = "ApiJwtToken";

        public ServerAuthTokenService(IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration configuration, LoginRequest loginDto)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _loginDto = loginDto;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_cache.TryGetValue(CacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            // Получаем HTTP-клиент, настроенный для внешнего API с Basic-аутентификацией
            // Используем здесь "ExternalApiClient"
            var client = _httpClientFactory.CreateClient("ExternalApiBasicAuthClient");

            var basicUser = _configuration["ExternalApi:BasicAuthUsername"]!;
            var basicPass = _configuration["ExternalApi:BasicAuthPassword"]!;
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicUser}:{basicPass}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var url = $"api/v1/identity/loginpassword?UserName={Uri.EscapeDataString(_loginDto.Username)}&Password={Uri.EscapeDataString(_loginDto.Password)}";

            var personId = int.Parse(_configuration["ExternalApi:PersonID"]!);
            var personPass = _configuration["ExternalApi:PersonPassword"] ?? _loginDto.Password;
            var body = new { PersonID = personId, Password = personPass };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(body)
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content.ReadFromJsonAsync<SingleResponse<IdentityData>>();
            var token = wrapper?.Data?.Token ?? string.Empty;
            var expires = DateTime.UtcNow.AddHours(1); 
            var ttl = expires - DateTime.UtcNow;

            _cache.Set(CacheKey, token, ttl);

            return token;
        }
    }
}