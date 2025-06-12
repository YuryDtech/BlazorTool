using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlazorTool.Client.Models;
using System.Diagnostics;
namespace BlazorTool.Client.Services

{
    public class ApiServiceClient
    {
        private readonly HttpClient _http;
        private readonly string _token;

        public ApiServiceClient(HttpClient http, string token)
        {
            _http = http;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _token = token;
        }
        public async Task<List<WorkOrder>> GetWorkOrdersAsync(
                                        int deviceID,
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
            var qp = new List<string>();
            qp.Add($"DeviceID={deviceID}");
            if (workOrderID.HasValue) qp.Add($"WorkOrderID={workOrderID.Value}");
            if (!string.IsNullOrWhiteSpace(deviceName)) qp.Add($"DeviceName={Uri.EscapeDataString(deviceName)}");
            if (isDep.HasValue) qp.Add($"IsDep={isDep.Value}");
            if (isTakenPerson.HasValue) qp.Add($"IsTakenPerson={isTakenPerson.Value}");
            if (active.HasValue) qp.Add($"Active={active.Value}");
            if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (personID.HasValue) qp.Add($"PersonID={personID.Value}");
            if (isPlan.HasValue) qp.Add($"IsPlan={isPlan.Value}");
            if (isWithPerson.HasValue) qp.Add($"IsWithPerson={isWithPerson.Value}");

            var url = "api/v1/wo/getlist?" + string.Join("&", qp);

            var wrapper = await _http.GetFromJsonAsync<WorkOrderResponse>(url);
            Debug.Print("\n");
            Debug.Print("= = = = = = = = = = response: " + wrapper?.ToString());
            Debug.Print("\n");

            return wrapper?.Data ?? new List<WorkOrder>();
        }

        public async Task<List<OrderStatus>> GetOrderStatusesAsync(
                                        int? deviceCategoryID = null,
                                        int? personID = null,
                                        string lang = "pl-PL",
                                        bool? isEdit = null
                                    )
        {
            var qs = new List<string>
    {
        $"DeviceCategoryID={(deviceCategoryID?.ToString() ?? "")}",
        $"PersonID={(personID?.ToString() ?? "")}",
        $"Lang={Uri.EscapeDataString(lang)}",
        $"IsEdit={(isEdit?.ToString() ?? "")}"
    };

            var url = "api/v1/wo/getdict?" + string.Join("&", qs);

            var wrapper = await _http.GetFromJsonAsync<OrderStatusResponse>(url);
            return wrapper?.Data ?? new List<OrderStatus>();
        }


    }
}
