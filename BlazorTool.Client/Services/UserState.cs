using Blazored.LocalStorage;
using BlazorTool.Client.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorTool.Client.Services
{
    public class UserState
    {
        private readonly ILocalStorageService _localStorageService;
        public Task InitializationTask { get; private set; }

        public UserState(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            InitializationTask = LoadIdentityDataAsync(); // Set in constructor
        }

        public UserState()
        {
            //_localStorageService = new Blazored.LocalStorage.LocalStorageService();
            InitializationTask = Task.CompletedTask; // For parameterless constructor, if ever used
        }

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string LangCode { get; set; } = "pl-pl"; 
        public string? Token { get; set; }
        public int? PersonID { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(UserName);
        public RightMatrix? RightMatrix { get; set; }
        public bool UseOriginalColors { get; set; } = true;



        public async Task SaveIdentityDataAsync(IdentityData identityData)
        {
            UserName = identityData.Name;
            Token = identityData.Token;
            PersonID = identityData.PersonID;
            LangCode = identityData.LangCode;
            RightMatrix = identityData.RigthMatrix;
            UseOriginalColors = identityData.UseOriginalColors;
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
                    PersonID = identityData.PersonID;
                    LangCode = identityData.LangCode;
                    RightMatrix = identityData.RigthMatrix;
                    UseOriginalColors = identityData.UseOriginalColors;
                }
            }
        }

        public async Task ClearAsync()
        {
            UserName = null;
            Password = null;
            Token = null;
            PersonID = null;
            LangCode = null;
            RightMatrix = null;
            UseOriginalColors = true;
            await _localStorageService.RemoveItemAsync("identityData");
        }
    }
} 