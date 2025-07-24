namespace BlazorTool.Client.Services
{
    public class AppConfiguration
    {
        private readonly ApiServiceClient _apiServiceClient;

        public string? ApiAddress { get; private set; }

        public event Action? OnChange;

        public AppConfiguration(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }

        public async Task InitializeAsync()
        {
            await LoadApiAddressAsync();
        }

        public async Task LoadApiAddressAsync()
        {
            // "MES" user is used to get global settings
            ApiAddress = await _apiServiceClient.LoadSettingAsync("apiAddress", "MES");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
