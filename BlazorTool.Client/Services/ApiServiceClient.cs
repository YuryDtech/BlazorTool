using BlazorTool.Client.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Telerik.SvgIcons;
using Activity = BlazorTool.Client.Models.Activity;
namespace BlazorTool.Client.Services

{
    public class ApiServiceClient
    {
        private readonly HttpClient _http;
        private Dictionary<int, List<WorkOrder>> _workOrdersCache = new Dictionary<int, List<WorkOrder>>();
        private List<Device> _devicesCache = new List<Device>();
        public ApiServiceClient(HttpClient http)
        {
            _http = http;
        }

        

        #region Orders
        /// <summary>
        /// Retrieves a list of work orders, either from a cache or by querying the API.
        /// </summary>
        /// <remarks>If the work orders for the specified device ID are not already cached, this method
        /// queries the API and updates the cache. Subsequent calls for the same device ID will return cached results
        /// unless the cache is empty.</remarks>
        /// <param name="deviceId">The ID of the device for which to retrieve work orders. If <see langword="null"/>, retrieves all work
        /// orders.</param>
        /// <returns>A list of <see cref="WorkOrder"/> objects associated with the specified device ID, or all work orders if
        /// <paramref name="deviceId"/> is <see langword="null"/>.</returns>
        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(int? deviceId = null)
       {
            if (deviceId == null)//get all WorkOrders
            {
                if (_workOrdersCache.Count > 0)
                {
                    Console.WriteLine("Work orders found in cache: " + _workOrdersCache.Count + "\n");
                    return _workOrdersCache.SelectMany(x => x.Value).ToList();
                }
                var allOrders = await GetWorkOrdersAsync();
                //add to cache all work orders by deviceId
                foreach (var order in allOrders)
                {
                    if (!_workOrdersCache.ContainsKey(order.MachineID))
                    {
                        _workOrdersCache.Add(order.MachineID, new List<WorkOrder>());
                    }
                    _workOrdersCache[order.MachineID].Add(order);
                }
                return allOrders;
            }

            //get work orders by deviceId from cache or from API
            if (!_workOrdersCache.TryGetValue((int)deviceId, out var list)
                || list == null
                || list.Count == 0)
            {
               var fresh = await GetWorkOrdersAsync(deviceId);
                _workOrdersCache[(int)deviceId] = fresh;
                return fresh;
            }
            return list;
        }

        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(IEnumerable<Device> devices)
        {
            var result = new List<WorkOrder>();
            if (devices == null || devices.Count() == 0)
            {
                return result;
            }
            foreach (var device in devices)
            {
                if (!_workOrdersCache.TryGetValue(device.MachineID, out var list))
                {
                    var fresh = await GetWorkOrdersAsync(device.MachineID);
                    _workOrdersCache[device.MachineID] = fresh;
                    result.AddRange(fresh);          
                }
                else
                {
                    result.AddRange(list);
                }
            }
            return result;
        }

        public async Task<List<WorkOrder>> GetWorkOrdersCachedAsync(IEnumerable<int> deviceIds)
        {
            var result = new List<WorkOrder>();
            if (deviceIds == null || deviceIds.Count() == 0)
            {
                return result;
            }
            foreach (var deviceId in deviceIds)
            {
                if (!_workOrdersCache.TryGetValue(deviceId, out var list))
                {
                    var fresh = await GetWorkOrdersAsync(deviceId);
                    _workOrdersCache[deviceId] = fresh;
                    result.AddRange(fresh);
                }
                else
                {
                    result.AddRange(list);
                }
            }
            return result;
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
                                        bool? isWithPerson = null)
        {
            var qp = new List<string>();
            if (deviceID.HasValue) qp.Add($"DeviceID={deviceID.Value}"); 
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
            Console.WriteLine("= = = = = = = API response-> WorkOrder.Count: " + wrapper?.Data.Count.ToString());
            var result = wrapper?.Data ?? new List<WorkOrder>();
            return result;
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
            Console.WriteLine("\n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
            return wrapper?.Data ?? new List<OrderStatus>();
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(int workOrderID)
        {
            var orders = await GetWorkOrdersCachedAsync(workOrderID);
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("= = = = = = = = = No work order found for ID: " + workOrderID);
                return null;
            }
            return orders.FirstOrDefault();
        }

        public async Task<bool> SaveWorkOrderAsync(WorkOrder workOrder)
        {
            //TODO : implement saving work order to API
            //var url = "api/v1/wo/save";
            //var content = new StringContent(JsonConvert.SerializeObject(workOrder), System.Text.Encoding.UTF8, "application/json");
            //var response = await _http.PostAsync(url, content);
            //if (response.IsSuccessStatusCode)
            //{
            //    Debug.Print("\n= = = = = = = = = Work order saved successfully.\n");
            //    return true;
            //}
            //else
            //{
            //    Debug.Print("\n= = = = = = = = = SaveWorkOrderAsync error: " + response.ReasonPhrase + "\n");
            //    return false;
            //}
            //for test save only cache
            if (workOrder == null || workOrder.WorkOrderID <= 0)
            {
                Console.WriteLine("= = = = = = = = = Work order is null.\n");
                return false;
            }
            if (!_workOrdersCache.ContainsKey(workOrder.MachineID))
            {
                //new machineID, add new list to cache
                _workOrdersCache.Add(workOrder.MachineID, new List<WorkOrder>() { workOrder});
                Console.WriteLine("= = = = = = = = = Work order was added to cache.");
                return true;
            }
            
            //existing machineID, check if work order already exists in cache
            var ind = _workOrdersCache[workOrder.MachineID].FindIndex(x => x.WorkOrderID == workOrder.WorkOrderID);
            if (ind >= 0)
            {
                //update existing work order in cache
                _workOrdersCache[workOrder.MachineID][ind] = workOrder;
                Console.WriteLine("= = = = = = = = = Work order was updated in cache. ");
                Console.WriteLine($" === Title={workOrder.AssetNo}, StartDate={workOrder.StartDate}, EndDate={workOrder.EndDate}===");
                return true;
            }
         // new work - cant insert a new one
         //_workOrdersCache[workOrder.MachineID].Add(workOrder);
         Console.WriteLine("= = = = = = = = = Work order not found in base: " + workOrder.WorkOrderID);
            return false;
        }

        public async Task<bool> DeleteWorkOrderAsync(int workOrderID, int MachineID)
        {
            //TODO : implement deleting work order from API
            //var url = $"api/v1/wo/delete?workOrderID={workOrderID}";
            //var response = await _http.DeleteAsync(url);
            //if (response.IsSuccessStatusCode)
            //{
            //    Debug.Print("\n= = = = = = = = = Work order deleted successfully.\n");
            //    return true;
            //}
            //else
            //{
            //    Debug.Print("\n= = = = = = = = = DeleteWorkOrderAsync error: " + response.ReasonPhrase + "\n");
            //    return false;
            //}
            
            if (!_workOrdersCache.ContainsKey(MachineID))
            {
                Console.WriteLine("\n= = = = = = = = = No work orders found for MachineID: " + MachineID + "\n");
                return false;
            }
             
            var WO_index = _workOrdersCache[MachineID].FindIndex(x => x.WorkOrderID == workOrderID);
            if (WO_index < 0)
            {
                Console.WriteLine("\n= = = = = = = = = No work order found for ID: " + workOrderID + "\n");
                return false;
            }
            _workOrdersCache[MachineID].RemoveAt(WO_index);
            Console.WriteLine("\n= = = = = = = = = Work order deleted successfully for ID: " + workOrderID + "\n");
            return true;
        }

        //temp, for test only
        //TODO : implement getting work order categories from Dicts from API
        //TODO : implement Dicts model and methods
        public async Task<List<Dict>> GetWODictionaries(int personID, string lang = "pl-PL")
        {
            var qp = new List<string>();
            qp.Add($"PersonID={personID}");
            qp.Add($"Lang={lang}");
            var url = "api/v1/wo/getdict?" + string.Join("&", qp);
            var wrapper = await _http.GetFromJsonAsync<ApiResponse<Dict>>(url);
            Console.WriteLine("\n");
            Console.WriteLine("= = = = = = = = = = response Dict.Count: " + wrapper?.Data.Count.ToString());
            Console.WriteLine("\n");
            return wrapper?.Data ?? new List<Dict>();
        }

        #endregion

        #region Activity
        public async Task<List<Activity>> GetActivityByWO(int workorder_id)
        {
            var url = $"api/v1/activity/getlist?woID={workorder_id}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<ApiResponse<Activity>>(content);
            return wrapper?.Data ?? new List<Activity>();
        }
        #endregion

        #region users
        public async Task<List<Person>> GetAllPersons()
        {
            var url = "api/v1/other/getuserslist";
            Console.WriteLine("====== Start GetAllPersons() request: " + _http.BaseAddress + url);
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("\n= = = = = = = = = Users response error: " + response.ReasonPhrase + "\n");
                return new List<Person>();
            }
            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Person>>();
            Console.WriteLine($"\n= = = = = = = = = response {_http.BaseAddress}{url} \n====== Users: " + wrapper?.Data.Count.ToString() + "\n");
            return wrapper?.Data ?? new List<Person>();
        }
        #endregion

        #region Devices
        public async Task<List<Device>> GetAllDevicesCachedAsync()
        {
            if (!_devicesCache.Any())
            {
                _devicesCache = await GetDevicesAsync();
            }                
            
            return _devicesCache;
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
            Console.WriteLine($" ====    name:{name} MachineIDs.count={machineIDs?.Count()}");
            var url = "api/v1/device/getlist" + (qp.Count > 0 ? "?" + string.Join("&", qp) : "");
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("\n= = = = = = = = Devices response error: " + response.ReasonPhrase + "\n");
                return new List<Device>();
            }
            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Device>>();

            Console.WriteLine("\n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
            
            return wrapper?.Data ?? new List<Device>();
        }
        #endregion

        #region Other functions
        public async Task<(bool, string)> CheckApiAddress(string address)
        {
            var url = "api/v1/settings/check";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("address", address)
            });
            var response = await _http.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("\n= = = = = = = = = CheckApiAddress error: " + response.ReasonPhrase + "\n");
                return (false, "API address is invalid. " + response.ReasonPhrase);
            }
            var wrapper = await response.Content.ReadFromJsonAsync<SimpleResponse>();
            return (wrapper.Success,wrapper.Message);
        }
        
        public async Task<string> GetSettingAsync(string key, string user)
        {
            var url = $"api/v1/settings/get?key={Uri.EscapeDataString(key)}&user={Uri.EscapeDataString(user)}";
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Предполагаем, что GetSettingAsync возвращает простую строку
                var settingValue = await response.Content.ReadAsStringAsync();

                return settingValue;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error in GetSettingAsync: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetSettingAsync: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> SaveSettingAsync(string key, string value, string user)
        {
            var url = "api/v1/settings/set";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key", key),
                new KeyValuePair<string, string>("value", value),
                new KeyValuePair<string, string>("user", user)
            });
            var response = await _http.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Debug.Print("\n= = = = = = = = = SaveSettingAsync error: " + response.ReasonPhrase + "\n");
                return false;
            }
        }
        #endregion

    }
}
