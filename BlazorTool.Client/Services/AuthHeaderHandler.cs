using Blazored.LocalStorage;
using BlazorTool.Client.Models;
using Microsoft.AspNetCore.Components;
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
        private readonly UserState _userState;

        public AuthHeaderHandler(ILocalStorageService localStorageService, HttpClient httpClient, UserState userState)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _userState = userState;
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
            string? identityDataJson = await _localStorageService.GetItemAsStringAsync("identityData");
            IdentityData? identityData = null;

            if (!string.IsNullOrEmpty(identityDataJson))
            {
                try
                {
                    identityData = JsonConvert.DeserializeObject<IdentityData>(identityDataJson);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"AuthHeaderHandler: Error deserializing identityData from LocalStorage: {ex.Message}");
                    identityData = null;
                }
            }

            // Проверяем, есть ли токен и не истек ли он
            if (identityData != null && !string.IsNullOrEmpty(identityData.Token) && identityData.Expires > DateTime.UtcNow)
            {
                return identityData.Token;
            }
            else
            {
                // Токен отсутствует или истек, пытаемся получить новый
                return await GetNewToken();
            }
        }

        private async Task<string> GetNewToken()
        {
            if (string.IsNullOrEmpty(_userState.UserName) || string.IsNullOrEmpty(_userState.Password))
            {
                Console.WriteLine("AuthHeaderHandler: UserState does not contain username or password for token refresh.");
                return string.Empty;
            }

            try
            {
                var loginRequest = new LoginRequest
                {
                    Username = _userState.UserName,
                    Password = _userState.Password
                };

                var loginResponse = await _httpClient.PostAsJsonAsync("api/v1/identity/loginpassword", loginRequest);
                loginResponse.EnsureSuccessStatusCode(); // Выбросит исключение, если статус-код не 2xx

                var apiResponse = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<IdentityData>>();

                if (apiResponse != null && apiResponse.IsValid && apiResponse.Data != null && apiResponse.Data.Any() && !string.IsNullOrEmpty(apiResponse.Data[0].Token))
                {
                    var identityData = apiResponse.Data[0];
                    await _localStorageService.SetItemAsStringAsync("identityData", JsonConvert.SerializeObject(identityData));
                    _userState.Token = identityData.Token; // Update UserState with new token
                    return identityData.Token;
                }
                else
                {
                    Console.WriteLine("AuthHeaderHandler: Failed to get token from login API. ApiResponse is invalid or data is null/empty.");
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
