using Blazored.LocalStorage;
using BlazorTool.Client.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorTool.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient; 
        private readonly LoginRequest _loginDto; 

        public AuthHeaderHandler(ILocalStorageService localStorageService, HttpClient httpClient, LoginRequest loginDto)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _loginDto = loginDto;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = await GetValidToken();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                // Обработка случая, когда токен не получен (например, перенаправление на страницу входа)
                Console.WriteLine("AuthHeaderHandler: No valid token available. Request sent without Bearer token.");
                // В реальном приложении здесь можно было бы добавить логику для перенаправления на страницу логина
                // или выбросить исключение, если токен обязателен.
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetValidToken()
        {
            string tokenJson = await _localStorageService.GetItemAsStringAsync("token");
            TokenResponse tokenResult = null;

            if (!string.IsNullOrEmpty(tokenJson))
            {
                try
                {
                    tokenResult = JsonConvert.DeserializeObject<TokenResponse>(tokenJson);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"AuthHeaderHandler: Error deserializing token from LocalStorage: {ex.Message}");
                    tokenResult = null;
                }
            }

            // Проверяем, есть ли токен и не истек ли он
            if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.Token) && tokenResult.Expires > DateTime.UtcNow)
            {
                return tokenResult.Token;
            }
            else
            {
                // Токен отсутствует или истек, пытаемся получить новый
                return await GetNewToken();
            }
        }

        private async Task<string> GetNewToken()
        {
            try
            {
                var loginResponse = await _httpClient.PostAsJsonAsync("api/v1/identity/loginpassword", _loginDto);
                loginResponse.EnsureSuccessStatusCode(); // Выбросит исключение, если статус-код не 2xx

                var tokenResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

                if (tokenResult != null && !string.IsNullOrEmpty(tokenResult.Token))
                {
                    await _localStorageService.SetItemAsStringAsync("token", JsonConvert.SerializeObject(tokenResult));
                    return tokenResult.Token;
                }
                else
                {
                    Console.WriteLine("AuthHeaderHandler: Failed to get token from login API. Token or TokenResult is null.");
                    return string.Empty;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"AuthHeaderHandler: HTTP Request error during token refresh: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthHeaderHandler: Unexpected error during token refresh: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
