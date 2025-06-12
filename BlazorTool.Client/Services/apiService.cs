using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlazorTool.Client.Models;
namespace BlazorTool.Client.Services

{
    public class apiService
    {
        private readonly HttpClient _http;
        private readonly string _token;

        public apiService(HttpClient http, string token)
        {
            _http = http;
            _token = token;
        }
        public async Task<List<WorkOrder>> GetWorkOrdersAsync(
            int? deviceID = null,
            int? workOrderID = null,
            string? deviceName = null,
            bool? isDep = null,
            bool? isTakenPerson = null,
            bool? active = null,
            string lang = "pl-PL",
            int? personID = null,
            bool? isPlan = null,
            bool? isWithPerson = null
        )
        {
            var qs = new List<string>
            {
                $"DeviceID={(deviceID?.ToString() ?? "")}",
                $"WorkOrderID={(workOrderID?.ToString() ?? "")}",
                $"DeviceName={Uri.EscapeDataString(deviceName ?? "")}",
                $"IsDep={(isDep?.ToString() ?? "")}",
                $"isTakenPerson={(isTakenPerson?.ToString() ?? "")}",
                $"Active={(active?.ToString() ?? "")}",
                $"Lang={Uri.EscapeDataString(lang)}",
                $"PersonID={(personID?.ToString() ?? "")}",
                $"IsPlan={(isPlan?.ToString() ?? "")}",
                $"IsWithPerson={(isWithPerson?.ToString() ?? "")}"
            };

            var url = "wo/getlist?" + string.Join("&", qs);

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<List<WorkOrder>>()
                   ?? new List<WorkOrder>();
        }
        
    }
}
