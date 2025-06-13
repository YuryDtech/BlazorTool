using BlazorTool.Client.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
namespace BlazorTool.Client.Services

{
    public class ApiServiceClient
    {
        private readonly HttpClient _http;
        private readonly string _token;
        private Dictionary<int, List<WorkOrder>> _workOrdersCache = new Dictionary<int, List<WorkOrder>>();
        private List<Device> _devicesCache = new List<Device>();
        public ApiServiceClient(HttpClient http, string token)
        {
            _http = http;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _token = token;
        }

        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(int deviceId)
       {
            if (!_workOrdersCache.TryGetValue(deviceId, out var list)
                || list == null
                || list.Count == 0)
            {
               var fresh = await GetWorkOrdersAsync(deviceId);
                _workOrdersCache[deviceId] = fresh;
                return fresh;
            }
            return list;
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
                                        bool? isWithPerson = null)
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

            var wrapper = await _http.GetFromJsonAsync<ApiResponse<WorkOrder>>(url);
            Debug.Print("\n");
            Debug.Print("= = = = = = = = = = response WorkOrder.Count: " + wrapper?.Data.Count.ToString());
            Debug.Print("\n");
            _workOrdersCache[deviceID] = wrapper?.Data ?? new List<WorkOrder>();
            return wrapper?.Data ?? new List<WorkOrder>();
        }

        public async Task<List<OrderStatus>> GetOrderStatusesAsync(
                                        int personID,
                                        int? deviceCategoryID = null,
                                        string lang = "pl-PL",
                                        bool? isEdit = null)
        {
                var qs = new List<string>
            {
                $"DeviceCategoryID={(deviceCategoryID?.ToString() ?? "")}",
                $"PersonID={personID}",
                $"Lang={Uri.EscapeDataString(lang)}",
                $"IsEdit={(isEdit?.ToString() ?? "")}"
            };

            var url = "api/v1/wo/getdict?" + string.Join("&", qs);
            var wrapper = await _http.GetFromJsonAsync<ApiResponse<OrderStatus>>(url);
            Debug.Print("\n= = = = = = = = = response Devices: " + wrapper?.ToString() + "\n");
            return wrapper?.Data ?? new List<OrderStatus>();
        }

        public async Task<List<Device>> GetAllDevicesCachedAsync()
        {
            if (!_devicesCache.Any())
            {
                _devicesCache = await GetDevicesAsync();
            }
            var fresh = await GetDevicesAsync();
            _devicesCache = fresh; 
            return fresh;
        }

        private async Task<List<Device>> GetDevicesAsync(
                                        string lang = "pl-pl",
                                        string? name = null,
                                        int? categoryID = null,
                                        bool? isSet = null,
                                        IEnumerable<int>? machineIDs = null)
        {
            var qp = new List<string>();
            if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (!string.IsNullOrWhiteSpace(name)) qp.Add($"Name={Uri.EscapeDataString(name)}");
            if (categoryID.HasValue) qp.Add($"CategoryID={categoryID.Value}");
            if (isSet.HasValue) qp.Add($"IsSet={isSet.Value}");
            if (machineIDs != null)
                foreach (var id in machineIDs)
                    qp.Add($"MachineIDs={id}");

            var url = "api/v1/device/getlist" + (qp.Count > 0 ? "?" + string.Join("&", qp) : "");
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Debug.Print("\n= = = = = = = = =Devices response error: " + response.ReasonPhrase + "\n");
                return new List<Device>();
            }
            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Device>>();

            Debug.Print("\n= = = = = = = = = response Devices: " + wrapper?.ToString() + "\n");
            // Cache the devices
            if (wrapper?.Data != null && wrapper.Data.Count > 0)
            {
                foreach (var device in wrapper.Data)
                {
                    if (!_devicesCache.Any(d => d.MachineID == device.MachineID))
                    {
                        _devicesCache.Add(device);
                    }
                }
            }
            
            return wrapper?.Data ?? new List<Device>();
        }

    }
}
