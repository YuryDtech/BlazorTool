using Blazored.LocalStorage;
using BlazorTool.Client.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BlazorTool.Client.Services
{
    public class UserState
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly ILogger<UserState> _logger;
        public Task InitializationTask { get; private set; }
        public event Action OnChange;

        public UserState(ILocalStorageService localStorageService, ILogger<UserState> logger)
        {
            _localStorageService = localStorageService;
            _logger = logger;
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
            _logger.LogInformation("DEBUG: SaveIdentityDataAsync started. LangCode from data: {LangCode}", identityData.LangCode);
            UserName = identityData.Name;
            Token = identityData.Token;
            PersonID = identityData.PersonID;
            LangCode = identityData.LangCode;
            RightMatrix = identityData.RigthMatrix;
            UseOriginalColors = identityData.UseOriginalColors;
            var cultureInfo = new CultureInfo(identityData.LangCode);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            _logger.LogInformation("DEBUG: Culture set to {CultureName} in SaveIdentityDataAsync.", cultureInfo.Name);
            await _localStorageService.SetItemAsStringAsync("identityData", JsonConvert.SerializeObject(identityData));
            NotifyStateChanged();
        }

        public async Task LoadIdentityDataAsync()
        {
            _logger.LogInformation("DEBUG: LoadIdentityDataAsync started.");
            string? identityDataJson = await _localStorageService.GetItemAsStringAsync("identityData");
            if (!string.IsNullOrEmpty(identityDataJson))
            {
                IdentityData? identityData = JsonConvert.DeserializeObject<IdentityData>(identityDataJson);
                if (identityData != null)
                {
                    _logger.LogInformation("DEBUG: Loaded LangCode from storage: {LangCode}", identityData.LangCode);
                    UserName = identityData.Name;
                    Token = identityData.Token;
                    PersonID = identityData.PersonID;
                    LangCode = identityData.LangCode;
                    RightMatrix = identityData.RigthMatrix;
                    UseOriginalColors = identityData.UseOriginalColors;
                    var cultureInfo = new CultureInfo(identityData.LangCode);
                    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                    _logger.LogInformation("DEBUG: Culture set to {CultureName} in LoadIdentityDataAsync.", cultureInfo.Name);
                }
            }
            else
            {
                _logger.LogWarning("DEBUG: No identityData found in local storage.");
            }
        }

        public async Task ClearAsync()
        {
            UserName = null;
            Password = null;
            Token = null;
            PersonID = null;
            LangCode = string.Empty;
            RightMatrix = null;
            UseOriginalColors = true;
            //var cultureInfo = new CultureInfo("en-EN");
            //CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            await _localStorageService.RemoveItemAsync("identityData");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
} 