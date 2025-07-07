using Blazored.LocalStorage;
using BlazorTool.Client.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorTool.Client.Services
{
    public class UserState
    {
        private readonly ILocalStorageService _localStorageService;

        public UserState(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public UserState()
        {
            //_localStorageService = new Blazored.LocalStorage.LocalStorageService();
        }

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(UserName);
        public RightMatrix? RightMatrix { get; set; }

        public async Task SaveIdentityDataAsync(IdentityData identityData)
        {
            UserName = identityData.Name;
            Token = identityData.Token;
            RightMatrix = identityData.RigthMatrix;
            await _localStorageService.SetItemAsStringAsync("identityData", JsonConvert.SerializeObject(identityData));
        }

        public async Task LoadIdentityDataAsync()
        {
            string? identityDataJson = await _localStorageService.GetItemAsStringAsync("identityData");
            if (!string.IsNullOrEmpty(identityDataJson))
            {
                IdentityData? identityData = JsonConvert.DeserializeObject<IdentityData>(identityDataJson);
                if (identityData != null)
                {
                    UserName = identityData.Name;
                    Token = identityData.Token;
                    RightMatrix = identityData.RigthMatrix;
                }
            }
        }

        public async Task ClearAsync()
        {
            UserName = null;
            Password = null;
            Token = null;
            RightMatrix = null;
            await _localStorageService.RemoveItemAsync("identityData");
        }
    }
} 