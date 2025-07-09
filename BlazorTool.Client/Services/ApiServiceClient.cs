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
        private readonly UserState _userState;
        private List<Dict> _dictCache = new List<Dict>();

        public ApiServiceClient(HttpClient http, UserState userState)
        {
            _http = http;
            _userState = userState;
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
                    Console.WriteLine($"[{_userState.UserName}] Work orders found in cache: " + _workOrdersCache.Count + "\n");
                    Debug.WriteLine($"[{_userState.UserName}] Work orders found in cache: " + _workOrdersCache.Count + "\n");
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

            try
            {
                var wrapper = await _http.GetFromJsonAsync<ApiResponse<WorkOrder>>(url);
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = API response-> WorkOrder.Count: " + wrapper?.Data.Count.ToString());
                var result = wrapper?.Data ?? new List<WorkOrder>();
                return result;
            }
            catch (HttpRequestException ex)
            {
                await _userState.ClearAsync();
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<WorkOrder>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<WorkOrder>();
            }
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
            try
            {
                var wrapper = await _http.GetFromJsonAsync<ApiResponse<OrderStatus>>(url);
                Console.WriteLine($"[{_userState.UserName}] \n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
                Debug.WriteLine($"[{_userState.UserName}] \n= = = = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
                return wrapper?.Data ?? new List<OrderStatus>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<OrderStatus>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<OrderStatus>();
            }
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(int workOrderID)
        {
            var orders = await GetWorkOrdersCachedAsync(workOrderID);
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = = = No work order found for ID: " + workOrderID);
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = = = = No work order found for ID: " + workOrderID);
                return null;
            }
            return orders.FirstOrDefault();
        }

        public async Task<SingleResponse<WorkOrder>> UpdateWorkOrderAsync(WorkOrder workOrder)
        {
            if (workOrder == null || workOrder.WorkOrderID < 0)
            {
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = Work order is null or has invalid ID.\n");
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = = Work order is null or has invalid ID.\n");
                return new SingleResponse<WorkOrder> { IsValid = false, Errors = new List<string> { "Work order is null or has invalid ID." } };
            }

            if (workOrder.WorkOrderID == 0) // NEW order
            {
                var saveResult = await SaveWorkOrderAsync(workOrder);
                if (saveResult.IsValid && saveResult.Data != null)
                {
                    // Add to cache after successful save to API
                    if (!_workOrdersCache.ContainsKey(saveResult.Data.MachineID))
                    {
                        _workOrdersCache.Add(saveResult.Data.MachineID, new List<WorkOrder>());
                    }
                    _workOrdersCache[saveResult.Data.MachineID].Add(saveResult.Data);
                    Console.WriteLine($"[{_userState.UserName}] = = = = NEW workorder ID={saveResult.Data.WorkOrderID} saved to cache after API save.\n");
                    Debug.WriteLine($"[{_userState.UserName}] = = = = NEW workorder ID={saveResult.Data.WorkOrderID} saved to cache after API save.\n");
                }
                return saveResult;
            }

            // Existing work order update logic (caching only, assuming API update is handled elsewhere or not needed for existing)
            if (!_workOrdersCache.ContainsKey(workOrder.MachineID))
            {
                //new machineID, add new list to cache
                _workOrdersCache.Add(workOrder.MachineID, new List<WorkOrder>() { workOrder });
                Console.WriteLine($"[{_userState.UserName}] = = = = Work order ID={workOrder.WorkOrderID} was added to cache.");
                Debug.WriteLine($"[{_userState.UserName}] = = = = Work order ID={workOrder.WorkOrderID} was added to cache.");
                return new SingleResponse<WorkOrder> { IsValid = true, Data = workOrder };
            }

            //existing machineID, check if work order already exists in cache
            var ind = _workOrdersCache[workOrder.MachineID].FindIndex(x => x.WorkOrderID == workOrder.WorkOrderID);
            if (ind >= 0)
            {
                //update existing work order in cache
                _workOrdersCache[workOrder.MachineID][ind] = workOrder;
                Console.WriteLine($"[{_userState.UserName}] = = = = = = Work order ID={workOrder.WorkOrderID} was updated in cache. ");
                Console.WriteLine($" === Title={workOrder.AssetNo}, StartDate={workOrder.StartDate}, EndDate={workOrder.EndDate}===");
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = Work order ID={workOrder.WorkOrderID} was updated in cache. ");
                Debug.WriteLine($" === Title={workOrder.AssetNo}, StartDate={workOrder.StartDate}, EndDate={workOrder.EndDate}===");
                return new SingleResponse<WorkOrder> { IsValid = true, Data = workOrder };
            }
            else // work order not found in cache
            {
                _workOrdersCache[workOrder.MachineID].Add(workOrder);
                Console.WriteLine($"[{_userState.UserName}] = = = = Work order ID={workOrder.WorkOrderID} was added to cache.");
                Debug.WriteLine($"[{_userState.UserName}] = = = = Work order ID={workOrder.WorkOrderID} was added to cache.");
                return new SingleResponse<WorkOrder> { IsValid = true, Data = workOrder };
            }
        }

        /// <summary>
        /// Save only new workorder.
        /// </summary>
        /// <param name="workOrder"></param>
        /// <returns></returns>
        public async Task<SingleResponse<WorkOrder>> SaveWorkOrderAsync(WorkOrder workOrder)
        {
            var errors = new List<string>();

            // 1. LevelID - Have to be greater then 0 (обязательный)
            if (!workOrder.LevelID.HasValue || workOrder.LevelID <= 0)
            {
                errors.Add("LevelID must be greater than 0.");
            }

            // 2. Description - Can not be empty (обязательный)
            if (string.IsNullOrWhiteSpace(workOrder.WODesc))
            {
                errors.Add("Description cannot be empty.");
            }

            // 3. MachineID - Have to be greater then 0 (обязательный)
            if (workOrder.MachineID <= 0)
            {
                errors.Add("MachineID must be greater than 0.");
            }

            // 4. PersonID - Have to be greater then 0 (обязательный)
            if (!_userState.PersonID.HasValue || _userState.PersonID <= 0)
            {
                errors.Add("Current user's PersonID is not available or invalid.");
            }

            // 5. End_Date - Start date can not be higher then end date (не обязательный)
            if (workOrder.StartDate.HasValue && workOrder.EndDate.HasValue && workOrder.StartDate > workOrder.EndDate)
            {
                errors.Add("Start date cannot be higher than end date.");
            }

            // 6. ReasonID - Have to be greater then 0 (не обязательный)
            if (workOrder.ReasonID.HasValue && workOrder.ReasonID <= 0)
            {
                errors.Add("ReasonID must be greater than 0 if provided.");
            }

            // 7. CategoryID - Have to be greater then 0 (не обязательный)
            if (workOrder.CategoryID.HasValue && workOrder.CategoryID <= 0)
            {
                errors.Add("CategoryID must be greater than 0 if provided.");
            }

            // 8. DepartmentID - Have to be greater then 0 (не обязательный)
            int? departmentId = null;
            if (!string.IsNullOrWhiteSpace(workOrder.DepName))
            {
                if (!_userState.PersonID.HasValue)
                {
                    errors.Add("Cannot validate DepartmentID: Current user's PersonID is not available.");
                }
                else
                {
                    try
                    {
                        var departments = await GetWODepartments(_userState.PersonID.Value);
                        var department = departments.FirstOrDefault(d => d.Name == workOrder.DepName);
                        if (department == null || department.Id <= 0)
                        {
                            errors.Add($"Department '{workOrder.DepName}' not found or has invalid ID.");
                        }
                        else
                        {
                            departmentId = department.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error validating DepartmentID: {ex.Message}");
                    }
                }
            }

            // 9. AssignedPersonID - Have to be greater then 0 (не обязательный)
            int? assignedPersonId = null;
            if (!string.IsNullOrWhiteSpace(workOrder.AssignedPerson))
            {
                try
                {
                    var persons = await GetAllPersons();
                    var assignedPerson = persons.FirstOrDefault(p => p.Name == workOrder.AssignedPerson);
                    if (assignedPerson == null || assignedPerson.PersonId <= 0)
                    {
                        errors.Add($"Assigned person '{workOrder.AssignedPerson}' not found or has invalid ID.");
                    }
                    else
                    {
                        assignedPersonId = assignedPerson.PersonId;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error validating AssignedPersonID: {ex.Message}");
                }
            }


            if (errors.Any())
            {
                return new SingleResponse<WorkOrder>
                {
                    IsValid = false,
                    Errors = errors
                };
            }

            var url = "api/v1/wo/create";
            try
            {

                // mapping на WorkOrderCreateRequest
                var createRequest = new WorkOrderCreateRequest
                {
                    MachineID = workOrder.MachineID,
                    PersonID = _userState.PersonID.Value, 
                    LevelID = workOrder.LevelID.Value,
                    Description = workOrder.WODesc,
                    StartDate = workOrder.StartDate?.ToString("O"),
                    EndDate = workOrder.EndDate?.ToString("O"),
                    ReasonID = workOrder.ReasonID,
                    CategoryID = workOrder.CategoryID,
                    DepartmentID = departmentId,
                    AssignedPersonID = assignedPersonId 
                }; //TODO location - what is this

                var response = await _http.PostAsJsonAsync(url, createRequest);
                var apiResponse = await response.Content.ReadFromJsonAsync<SingleResponse<WorkOrder>>();

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.IsValid && apiResponse.Data != null)
                {
                    Console.WriteLine($"[{_userState.UserName}] = = = = = = Work order saved successfully. ID: {apiResponse.Data.WorkOrderID}\n");
                    Debug.WriteLine($"[{_userState.UserName}] = = = = = = Work order saved successfully. ID: {apiResponse.Data.WorkOrderID}\n");
                    //request new order from API
                    //url = $"api/v1/wo/get?WorkOrderID={apiResponse.Data.WorkOrderID}";
                    var updatedResponse = await GetWorkOrdersAsync( workOrderID: apiResponse.Data.WorkOrderID); //TODO add userstate.lang
                    if (updatedResponse != null && updatedResponse.Count != 0)
                    {
                        Debug.WriteLine("[{_userState.UserName}] = = = = = = Work order updated successfully from API. ID: " + updatedResponse.First().WorkOrderID);
                        apiResponse.Data = updatedResponse.First();
                        
                    } else
                    {
                        Console.WriteLine($"[{_userState.UserName}] = = = = = = Failed to retrieve updated work order from API.\n");
                        Debug.WriteLine($"[{_userState.UserName}] = = = = = = Failed to retrieve updated work order from API.\n");
                    } 
                        return apiResponse;
                }
                else
                {
                    var responseErrors = apiResponse?.Errors ?? new List<string>();
                    if (!response.IsSuccessStatusCode)
                    {
                        responseErrors.Add($"Server responded with status code: {response.StatusCode}");
                        responseErrors.Add($"Server response content: {await response.Content.ReadAsStringAsync()}");
                    }
                    Console.WriteLine($"[{_userState.UserName}] = = = = = = Failed to save work order. Errors: {string.Join(", ", responseErrors)}\n");
                    Debug.WriteLine($"[{_userState.UserName}] = = = = = = Failed to save work order. Errors: {string.Join(", ", responseErrors)}\n");
                    return new SingleResponse<WorkOrder> { IsValid = false, Errors = responseErrors };
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}\n");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}\n");
                return new SingleResponse<WorkOrder> { IsValid = false, Errors = new List<string> { $"Network error: {ex.Message}" } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}\n");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}\n");
                return new SingleResponse<WorkOrder> { IsValid = false, Errors = new List<string> { $"An unexpected error occurred: {ex.Message}" } };
            }
        }

        public async Task<bool> DeleteWorkOrderAsync(WorkOrder workOrder)
        {
            //TODO : implement deleting work order from API (PUT)
            //var url = $"api/v1/wo/close";
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

            if (!_workOrdersCache.ContainsKey(workOrder.MachineID))
            {
                Console.WriteLine("\n= = = = = = = = = No work orders found for MachineID: " + workOrder.MachineID + "\n");
                return false;
            }
             
            var WO_index = _workOrdersCache[workOrder.MachineID].FindIndex(x => x.WorkOrderID == workOrder.WorkOrderID);
            if (WO_index < 0)
            {
                Console.WriteLine("\n= = = = = = = = = No work order found for ID: " + workOrder.WorkOrderID + "\n");
                return false;
            }
            _workOrdersCache[workOrder.MachineID].RemoveAt(WO_index);
            Console.WriteLine("\n= = = = = = = = = Work order deleted successfully for ID: " + workOrder.WorkOrderID + "\n");
            return true;
        }

        #endregion

        #region Activity
        public async Task<List<Activity>> GetActivityByWO(int workorder_id)
        {
            var url = $"api/v1/activity/getlist?woID={workorder_id}";
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var wrapper = JsonConvert.DeserializeObject<ApiResponse<Activity>>(content);
                return wrapper?.Data ?? new List<Activity>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<Activity>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<Activity>();
            }
        }
        #endregion

        #region users
        public async Task<List<Person>> GetAllPersons()
        {
            var url = "api/v1/other/getuserslist";
            try
            {
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
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<Person>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<Person>();
            }
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
            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"\n[{_userState.UserName}] = = = = = = Devices response error: " + response.ReasonPhrase + "\n");
                    Debug.WriteLine($"\n[{_userState.UserName}] = = = = = Devices response error: " + response.ReasonPhrase + "\n");
                    return new List<Device>();
                }
                var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Device>>();

                Console.WriteLine($"\n[{_userState.UserName}] = = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
                Debug.WriteLine($"\n[{_userState.UserName}] = = = = = response Devices: " + wrapper?.Data.Count.ToString() + "\n");
                return wrapper?.Data ?? new List<Device>();
            }
            catch (HttpRequestException ex)
            {
                await _userState.ClearAsync();
                Console.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<Device>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<Device>();
            }
        }
        #endregion

        #region Other functions
        public async Task<List<Dict>> GetWODictionaries(int personID, string lang = "pl-PL")
        {
            var qp = new List<string>();
            qp.Add($"PersonID={personID}");
            qp.Add($"Lang={lang}");
            var url = "api/v1/wo/getdict?" + string.Join("&", qp);
            try
            {
                var wrapper = await _http.GetFromJsonAsync<ApiResponse<Dict>>(url);
                Console.WriteLine("\n");
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = response Dict.Count: " + wrapper?.Data.Count.ToString());
                Console.WriteLine("\n");
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = = response Dict.Count: " + wrapper?.Data.Count.ToString());
                if (wrapper != null && wrapper.Data != null && wrapper.IsValid)
                {
                    if (wrapper.Errors.Count == 0)
                    // Cache the dictionaries
                    _dictCache = wrapper.Data;
                    else
                    {
                        Console.WriteLine("[{_userState.UserName}] = = = = = = Errors in GetWODictionaries: " + string.Join(", ", wrapper.Errors));
                        Debug.WriteLine($"[{_userState.UserName}] = = = = = = Errors in GetWODictionaries: " + string.Join(", ", wrapper.Errors));
                    }
                }

                return wrapper?.Data ?? new List<Dict>();
            }
            catch (HttpRequestException ex)
            {
                await _userState.ClearAsync();
                Console.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<Dict>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<Dict>();
            }
        }
        public async Task<List<Dict>> GetWODictionariesCached(int personID, string lang = "pl-PL")
        {
            if (_dictCache.Count == 0)
            {//is need cache with personID and lang ?
                _dictCache = await GetWODictionaries(personID, lang);
            }
            return _dictCache;
            
        }

        public async Task<List<Dict>> GetWOCategories(int personID, string lang = "pl-PL")
        {
            return (await GetWODictionariesCached(personID)).Where(d => d.ListType == (int)ListTypeEnum.Category)
                .Distinct()
                .ToList();
        }

        public async Task<List<Dict>> GetWOStates(int personID, string lang = "pl-PL")
        {
            return (await GetWODictionariesCached(personID)).Where(d => d.ListType == (int)ListTypeEnum.State)
                .Distinct()
                .ToList();
        }

        public async Task<List<Dict>> GetWOLevels(int personID, string lang = "pl-PL")
        {
            return (await GetWODictionariesCached(personID)).Where(d => d.ListType == (int)ListTypeEnum.Level)
                .Distinct()
                .ToList();
        }

        public async Task<List<Dict>> GetWOReasons(int personID, string lang = "pl-PL")
        {
            return (await GetWODictionariesCached(personID)).Where(d => d.ListType == (int)ListTypeEnum.Reason)
                .Distinct()
                .ToList();
        }

        public async Task<List<Dict>> GetWODepartments(int personID, string lang = "pl-PL")
        {
            return (await GetWODictionariesCached(personID)).Where(d => d.ListType == (int)ListTypeEnum.Department)
                .Distinct()
                .ToList();
        }

        public async Task<bool> AddNewWODict(string name, int listType, bool isDefault = false, int machineCategoryId = 0)
        {
            if (string.IsNullOrEmpty(name) || listType < 1 || listType > 5) return false;
            Dict newDict = new Dict
            {
                Name = name,
                ListType = listType,
                IsDefault = false,
                MachineCategoryID = null, // or set to a specific value if needed
                Id = 0 // API will assign the ID
            };
            if (_dictCache.Any(d=>d.Name == name && d.ListType == listType && d.MachineCategoryID == machineCategoryId))
            {
                Console.WriteLine($"[{_userState.UserName}] = = = = = = Dictionary with name '{name}' and ListType {listType} already exists.");
                return false;
            }
            _dictCache.Add(newDict); 
            return true;
           //TODO SAVE return await PostSingleAsync<Dict, Dict>("api/v1/wo/adddict", newDict) is { IsValid: true, Data: { } };
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(url, data);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                return apiResponse ?? new ApiResponse<TResponse> { IsValid = false, Message = "Empty response from API." };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}");
                return new ApiResponse<TResponse> { IsValid = false, Message = $"Network error: {ex.Message}" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}");
                return new ApiResponse<TResponse> { IsValid = false, Message = $"An unexpected error occurred: {ex.Message}" };
            }
        }

        public async Task<SingleResponse<TResponse>> PostSingleAsync<TRequest, TResponse>(string url, TRequest data)
        {
            List<string>? errors = null;
            try
            {
                var response = await _http.PostAsJsonAsync(url, data);
                var apiResponse = await response.Content.ReadFromJsonAsync<SingleResponse<TResponse>>();
                errors = apiResponse?.Errors;
                response.EnsureSuccessStatusCode();
                return apiResponse ?? new SingleResponse<TResponse> { IsValid = false, Errors = errors ?? new List<string> { "Empty response from API." } };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}");
                return new SingleResponse<TResponse> { IsValid = false, Errors = errors ?? new List<string> { $"Network error: {ex.Message}" } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}");
                return new SingleResponse<TResponse> { IsValid = false, Errors = new List<string> { $"An unexpected error occurred: {ex.Message}" } };
            }
        }

        public async Task<(bool, string)> CheckApiAddress(string address)
        {
            var url = "api/v1/settings/check";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("address", address)
            });
            try
            {
                var response = await _http.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\n= = = = = = = = = CheckApiAddress error: " + response.ReasonPhrase + "\n");
                    return (false, "API address is invalid. " + response.ReasonPhrase);
                }
                var wrapper = await response.Content.ReadFromJsonAsync<SimpleResponse>();
                return (wrapper.Success,wrapper.Message);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}");
                return (false, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}");
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
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
            try
            {
                var response = await _http.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("\n= = = = = = = = = SaveSettingAsync error: " + response.ReasonPhrase + "\n");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during POST to {url}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Session
        public async Task<HttpResponseMessage> CheckSessionAsync()
        {
            return await _http.GetAsync("api/v1/identity/check-session");
        }
        #endregion
    }
}
