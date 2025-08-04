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
        private List<Person> _personsCache = new List<Person>();

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
                var allOrders = await GetWorkOrdersAsync(lang: _userState.LangCode);
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
            {}
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
                                        string? lang = null,
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
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = _userState.LangCode;
            }
            qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (personID.HasValue) qp.Add($"PersonID={personID.Value}");
            if (isPlan.HasValue) qp.Add($"IsPlan={isPlan.Value}");
            if (isWithPerson.HasValue) qp.Add($"IsWithPerson={isWithPerson.Value}");

            var url = "wo/getlist?" + string.Join("&", qp);

            try
            {
                var wrapper = await _http.GetFromJsonAsync<ApiResponse<WorkOrder>>(url);
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = API response-> WorkOrder.Count: " + wrapper?.Data.Count.ToString());
                var result = wrapper?.Data ?? new List<WorkOrder>();
                if (result.Count > 0) 
                    {
                        UpdateWorkOrdersInCache(result);
                    }
                return result;
            }
            catch (HttpRequestException ex)
            {
                //await _userState.ClearAsync(); //??
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

        #region Cache Management
        /// <summary>
        /// Adds or updates a work order in the cache. Can also be used to remove an item.
        /// This is the single point of truth for modifying the work order cache.
        /// </summary>
        /// <param name="workOrder">The work order to update in the cache.</param>
        /// <param name="remove">If true, the work order will be removed from the cache.</param>
        private void UpdateWorkOrderInCache(WorkOrder workOrder, bool remove = false)
        {
            if (workOrder == null) return;

            // First, remove any existing instance of this work order from the cache,
            // to handle cases where the MachineID might have changed.
            foreach (var list in _workOrdersCache.Values)
            {
                list.RemoveAll(wo => wo.WorkOrderID == workOrder.WorkOrderID);
            }

            if (!remove)
            {
                // Now, add the updated work order to the correct list.
                if (!_workOrdersCache.TryGetValue(workOrder.MachineID, out var orderList))
                {
                    // If the list for this machine doesn't exist, create it.
                    orderList = new List<WorkOrder>();
                    _workOrdersCache[workOrder.MachineID] = orderList;
                }
                orderList.Add(workOrder);
            }
        }

        /// <summary>
        /// Adds or updates a list of work orders in the cache.
        /// </summary>
        /// <param name="workOrders">The list of work orders to add or update.</param>
        public void UpdateWorkOrdersInCache(IEnumerable<WorkOrder> workOrders)
        {
            if (workOrders == null) return;

            foreach (var workOrder in workOrders)
            {
                UpdateWorkOrderInCache(workOrder);
            }
        }

        /// <summary>
        /// Forces a refresh of a single work order in the cache from the server.
        /// </summary>
        /// <param name="workOrderId">The ID of the work order to refresh.</param>
        public async Task RefreshWorkOrderInCacheAsync(int workOrderId)
        {
            // 1. Get the latest version from the API
            var freshWorkOrder = await GetWorkOrderByIdAsync(workOrderId);

            // 2. Use the centralized method to update the cache
            if (freshWorkOrder != null)
            {
                UpdateWorkOrderInCache(freshWorkOrder);
                Console.WriteLine($"[{_userState.UserName}] Refreshed and updated WO {workOrderId} in cache.");
                Debug.WriteLine($"[{_userState.UserName}] Refreshed and updated WO {workOrderId} in cache.");
            }
            else
            {
                // If the work order is not found on the server, remove it from the cache.
                // We need a dummy WorkOrder object with the ID to perform the removal.
                UpdateWorkOrderInCache(new WorkOrder { WorkOrderID = workOrderId }, remove: true);
                Console.WriteLine($"[{_userState.UserName}] WO {workOrderId} not found on server, removed from cache.");
                Debug.WriteLine($"[{_userState.UserName}] WO {workOrderId} not found on server, removed from cache.");
            }
        }

        /// <summary>
        /// Clears the entire work orders cache.
        /// </summary>
        public void InvalidateWorkOrdersCache()
        {
            _workOrdersCache.Clear();
            Console.WriteLine($"[{_userState.UserName}] Work orders cache invalidated.");
            Debug.WriteLine($"[{_userState.UserName}] Work orders cache invalidated.");
        }

        /// <summary>
        /// Clears the work orders cache for a specific device.
        /// </summary>
        /// <param name="deviceId">The device ID for which to clear the cache.</param>
        public void InvalidateWorkOrdersCacheForDevice(int deviceId)
        {
            if (_workOrdersCache.Remove(deviceId))
            {
                Console.WriteLine($"[{_userState.UserName}] Work orders cache for device {deviceId} invalidated.");
                Debug.WriteLine($"[{_userState.UserName}] Work orders cache for device {deviceId} invalidated.");
            }
        }
        #endregion

        

        public async Task<WorkOrder?> GetWorkOrderByIdCachedAsync(int workOrderID)
        {
            // Search in the cache first
            var cachedOrder = _workOrdersCache.Values
                                              .SelectMany(list => list)
                                              .FirstOrDefault(wo => wo.WorkOrderID == workOrderID);

            if (cachedOrder != null)
            {
                return cachedOrder;
            }

            // If not in cache, fetch from API
            var apiOrder = await GetWorkOrderByIdAsync(workOrderID);
            if (apiOrder != null)
            {
                // Add the newly fetched order to the cache
                UpdateWorkOrderInCache(apiOrder);
            }
            return apiOrder;
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(int workOrderID)
        {
            var orders = await GetWorkOrdersAsync(workOrderID: workOrderID); // FROM API
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine($"[{_userState.UserName}] = = = = = = = = = No work order found for ID: " + workOrderID);
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = = = = No work order found for ID: " + workOrderID);
                return null;
            }
            else
            {
                UpdateWorkOrderInCache(orders.FirstOrDefault());
                return orders.FirstOrDefault();
            }
        }
        public async Task<WorkOrderInfo?> GetWOInfo(int workOrderID)
        {
            if (workOrderID < 0) return null;
            var url = "wo/get?WorkOrderID=" + workOrderID;
            try
            {
                var wrapper = await _http.GetFromJsonAsync<SingleResponse<WorkOrderInfo>>(url);
                var result = wrapper?.Data ?? null;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_userState.UserName}] ApiServiceClient.GetWOInfo: Unexpected error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"[{_userState.UserName}] ApiServiceClient.GetWOInfo: Unexpected error during GET to {url}: {ex.Message}");
                return null;
            }

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
                    UpdateWorkOrderInCache(saveResult.Data);
                    Console.WriteLine($"[{_userState.UserName}] = = = = NEW workorder ID={saveResult.Data.WorkOrderID} saved to cache after API save.\n");
                    Debug.WriteLine($"[{_userState.UserName}] = = = = NEW workorder ID={saveResult.Data.WorkOrderID} saved to cache after API save.\n");
                }
                return saveResult;
            }

            // --- Existing Work Order Update Logic ---
            var errors = new List<string>();

            // --- Validation ---
            if (workOrder.WorkOrderID <= 0) errors.Add("WorkOrderID - Have to be greater then 0");
            if (!workOrder.LevelID.HasValue || workOrder.LevelID.Value <= 0) errors.Add("LevelID - Have to be greater then 0");
            if (string.IsNullOrWhiteSpace(workOrder.WODesc)) errors.Add("Description - Can not be empty");
            if (!_userState.PersonID.HasValue || _userState.PersonID.Value <= 0) errors.Add("PersonID - Have to be greater then 0");

            // --- ID Lookups ---
            int? departmentId = null;
            if (!string.IsNullOrWhiteSpace(workOrder.DepName))
            {
                var departments = GetWODepartments();
                departmentId = departments.FirstOrDefault(d => d.Name.Equals(workOrder.DepName, StringComparison.OrdinalIgnoreCase))?.Id;
            }

            int? assignedPersonId = null;
            if (!string.IsNullOrWhiteSpace(workOrder.AssignedPerson))
            {
                var persons = await GetAllPersons();
                assignedPersonId = persons.FirstOrDefault(p => p.Name.Equals(workOrder.AssignedPerson, StringComparison.OrdinalIgnoreCase))?.PersonId;
            }

            int? stateId = null;
            if (!string.IsNullOrWhiteSpace(workOrder.WOState))
            {
                var states = GetWOStates();
                stateId = states.FirstOrDefault(s => s.Name.Equals(workOrder.WOState, StringComparison.OrdinalIgnoreCase))?.Id;
            }

            if (!stateId.HasValue || stateId.Value < 0) stateId = 5;

            if (errors.Any())
            {
                return new SingleResponse<WorkOrder> { IsValid = false, Errors = errors };
            }

            // --- Request Creation ---
            var request = new UpdateWorkOrderRequest
            {
                WorkOrderID = workOrder.WorkOrderID,
                PersonID = _userState.PersonID.Value,
                CategoryID = workOrder.CategoryID,
                LevelID = workOrder.LevelID,
                ReasonID = workOrder.ReasonID,
                Description = workOrder.WODesc,
                DepartmentID = departmentId,
                AssignedPersonID = assignedPersonId,
                Location = null, // As requested
                Start_Date = workOrder.StartDate?.ToString("O"),
                End_Date = workOrder.EndDate?.ToString("O"),
                StateID = stateId
            };

            var result = await PutSingleAsync<UpdateWorkOrderRequest, bool>("wo/update", request);

            // --- Cache Update ---
            if (result.IsValid)
            {
                UpdateWorkOrderInCache(workOrder);
            }

            return new SingleResponse<WorkOrder> {Data = workOrder, IsValid = result.IsValid, Errors = result.Errors };
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
                        var departments = GetWODepartments();
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

            var url = "wo/create";
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
                }; //TODO location - what is this?

                var response = await _http.PostAsJsonAsync(url, createRequest);
                var apiResponse = await response.Content.ReadFromJsonAsync<SingleResponse<WorkOrder>>();

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.IsValid && apiResponse.Data != null)
                {
                    Console.WriteLine($"[{_userState.UserName}] = = = = = = Work order saved successfully. ID: {apiResponse.Data.WorkOrderID}\n");
                    Debug.WriteLine($"[{_userState.UserName}] = = = = = = Work order saved successfully. ID: {apiResponse.Data.WorkOrderID}\n");
                    //request new order from API
                    //url = $"wo/get?WorkOrderID={apiResponse.Data.WorkOrderID}";
                    var updatedResponse = await GetWorkOrdersAsync( workOrderID: apiResponse.Data.WorkOrderID); //TODO add userstate.lang
                    if (updatedResponse != null && updatedResponse.Count != 0)
                    {
                        Debug.WriteLine("[{_userState.UserName}] = = = = = = Work order updated successfully from API. ID: " + updatedResponse.First().WorkOrderID);
                        apiResponse.Data = updatedResponse.First();
                        UpdateWorkOrderInCache(apiResponse.Data);
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

        public async Task<SingleResponse<bool>> CloseWorkOrderAsync(WorkOrder workOrder)
        {
            var errors = new List<string>();

            if (!_userState.PersonID.HasValue || _userState.PersonID.Value <= 0)
            {
                errors.Add("PersonID - Have to be greater then 0");
            }
            if (workOrder.WorkOrderID <= 0)
            {
                errors.Add("WorkOrderID - Have to be greater then 0");
            }
            if (!workOrder.CategoryID.HasValue || workOrder.CategoryID <= 0)
            {
                errors.Add("CategoryID - Have to be greater then 0");
            }
            if (!workOrder.ReasonID.HasValue || workOrder.ReasonID <= 0)
            {
                errors.Add("ReasonID - Have to be greater then 0");
            }
            if (!workOrder.ActCount.HasValue || workOrder.ActCount.Value <= 0)
            {
                errors.Add("WorkOrder must have at least one activity (act_Count > 0)");
            }

            if (workOrder.LevelID <= 0)
            {
                errors.Add("LevelID - Have to be greater then 0");
            }

            if (errors.Any())
            {
                return new SingleResponse<bool>
                {
                    IsValid = false,
                    Errors = errors
                };
            }

            var request = new CloseWorkOrderRequest
            {
                WorkOrderID = workOrder.WorkOrderID,
                PersonID = _userState.PersonID.Value,
                CategoryID = workOrder.CategoryID ?? 0,
                ReasonID = workOrder.ReasonID ?? 0,
                LevelID = workOrder.LevelID ?? 0,
            };

            var result = await PutSingleAsync<CloseWorkOrderRequest, bool>("wo/close", request);

            if (result.IsValid && result.Data)
            {
                UpdateWorkOrderInCache(workOrder);
            }
            return result;
        }

        public async Task<int> GetWOStateID(int? workorderID)
        { 
            if (workorderID == null || workorderID <= 0) return 0;
            var woInfo = await GetWOInfo(workorderID??0);
            if (woInfo != null && woInfo.StateID.HasValue && woInfo.StateID.Value > 0)
            {
                return woInfo.StateID.Value;
            }else
                return 0;
        }
        #endregion

        #region Activity
        public async Task<List<Activity>> GetActivitiesByWO(int workorder_id)
        {
            var url = $"activity/getlist?woID={workorder_id}&lang={_userState.LangCode}";
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

        public async Task<SingleResponse<ActivityResponse>> CreateActivityAsync(AddActivity activity)
        {
            var url = "activity/create";
            try
            {
                var response = await _http.PostAsJsonAsync(url, activity);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var activityResponse = JsonConvert.DeserializeObject<ActivityResponse>(content);
                    return new SingleResponse<ActivityResponse> { IsValid = true, Data = activityResponse };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ApiServiceClient: Failed to create activity. Status: {response.StatusCode}, Details: {errorContent}");
                    return new SingleResponse<ActivityResponse> { IsValid = false, Errors = new List<string> { $"Server error: {errorContent}" } };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during POST to {url}: {ex.Message}");
                return new SingleResponse<ActivityResponse> { IsValid = false, Errors = new List<string> { $"An unexpected error occurred: {ex.Message}" } };
            }
        }
        #endregion

        #region users
        public async Task<List<Person>> GetAllPersons()
        {
            if (_personsCache.Any())
            {
                return _personsCache;
            }

            var url = "other/getuserslist";
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

                _personsCache = wrapper?.Data ?? new List<Person>();
                return _personsCache;
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

        public List<Person> GetAllPersonsCached()
        {
            if (_personsCache.Any())
            {
                return _personsCache;
            }
            return new List<Person>();
        }

        public Person? GetPersonByIDCached(int personID)
        {
            if (_personsCache.Any())
            {
                return _personsCache.FirstOrDefault(p => p.PersonId == personID);
            }
            return null;
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
                                        string? lang = null,
                                        string? name = null,
                                        int? categoryID = null,
                                        bool? isSet = null,
                                        IEnumerable<int>? machineIDs = null)
        {
            var qp = new List<string>();
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = _userState.LangCode; 
            }
            if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
            if (!string.IsNullOrWhiteSpace(name)) qp.Add($"Name={Uri.EscapeDataString(name)}");
            if (categoryID.HasValue) qp.Add($"CategoryID={categoryID.Value}");
            if (isSet.HasValue) qp.Add($"IsSet={isSet.Value}");
            if (machineIDs != null)
                foreach (var id in machineIDs)
                    qp.Add($"MachineIDs={id}");
            Console.WriteLine($" ====    name:{name} MachineIDs.count={machineIDs?.Count()}");
            var url = "device/getlist" + (qp.Count > 0 ? "?" + string.Join("&", qp) : "");
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
                //await _userState.ClearAsync();
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
        public async Task<List<Dict>> GetWODictionaries(int? personID, string? lang = null)
        {
            if (personID == null || personID <= 0)
            {
                Console.WriteLine($"[{_userState.UserName}] = = = = = = Invalid PersonID: {personID}");
                Debug.WriteLine($"[{_userState.UserName}] = = = = = = Invalid PersonID: {personID}");
                return new List<Dict>();
            }
            var qp = new List<string>();
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = _userState.LangCode;
            }

            qp.Add($"PersonID={personID}");
            qp.Add($"Lang={lang}");
            var url = "wo/getdict?" + string.Join("&", qp);
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
                        Console.WriteLine($"[{_userState.UserName}] = = = = = = Errors in GetWODictionaries: " + string.Join(", ", wrapper.Errors));
                        Debug.WriteLine($"[{_userState.UserName}] = = = = = = Errors in GetWODictionaries: " + string.Join(", ", wrapper.Errors));
                    }
                }

                return wrapper?.Data ?? new List<Dict>();
            }
            catch (HttpRequestException ex)
            {
                //await _userState.ClearAsync();
                Console.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                Debug.WriteLine($"ApiServiceClient: HTTP Request error during GET to {url}: {ex.Message}");
                return new List<Dict>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during GET to {url}: {ex.Message}");
                return new List<Dict>();
            }
        }
        public List<Dict> GetWODictionariesCached()
        {
            //if (_dictCache.Count == 0)
            //{//is need cache with personID and lang ?
            //    _dictCache = GetWODictionaries(personID, lang).Result;
            //}
            return _dictCache;
            
        }

        public async Task<string> GetStateColor(int? workorderID)
        {
            var stateId = await GetWOStateID(workorderID);
            if (stateId == 0)
                return "primary";

            return GetColorByStateId(stateId);
        }

        public string ConvertStateColor(string state)
        {
            var states = GetWOStates();
            var stateId = states.FirstOrDefault(s => s.Name == state)?.Id;
            return GetColorByStateId(stateId ?? 0);
        }

        private string GetColorByStateId(int stateId)
        {
            return stateId switch
            {
                5 => "bg-success-light",
                0 => "bg-success-light",
                1 => "bg-danger-light",
                2 => "bg-warning-light",
                6 => "bg-warning-light",
                _ => "bg-gray-light"
            };
        }
        public List<Dict> GetWOCategories()
        {            
            return (GetWODictionariesCached()).Where(d => d.ListType == (int)WOListTypeEnum.Category)
                .Distinct()
                .ToList();
        }

        public List<Dict> GetWOStates()
        {            
            return (GetWODictionariesCached()).Where(d => d.ListType == (int)WOListTypeEnum.State)
                .Distinct()
                .ToList();
        }

        public List<Dict> GetWOLevels()
        {
            return (GetWODictionariesCached()).Where(d => d.ListType == (int)WOListTypeEnum.Level)
                .Distinct()
                .ToList();
        }

        public List<Dict> GetWOReasons()
        {
            return (GetWODictionariesCached()).Where(d => d.ListType == (int)WOListTypeEnum.Reason)
                .Distinct()
                .ToList();
        }

        public List<Dict> GetWODepartments()
        {
            return (GetWODictionariesCached()).Where(d => d.ListType == (int)WOListTypeEnum.Department)
                .Distinct()
                .ToList();
        }

        public async Task<bool> AddNewWODict(string name, int listType, bool isDefault = false, int? machineCategoryId = null)
        {//TODO: remove this method, dict is only read from API
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
                return false;
            }
            _dictCache.Add(newDict); 
            Console.WriteLine($"[{_userState.UserName}] = = = = = = Dictionary with name '{name}' and ListType {listType} added to cache.");
            return true;
           //TODO SAVE return await PostSingleAsync<Dict, Dict>("wo/adddict", newDict) is { IsValid: true, Data: { } };
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

        /// <summary>
        /// Sends a PUT request to a specified URL with a JSON payload and deserializes the response.
        /// </summary>
        public async Task<SingleResponse<TResponse>> PutSingleAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _http.PutAsJsonAsync(url, data);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return System.Text.Json.JsonSerializer.Deserialize<SingleResponse<TResponse>>(responseContent)
                           ?? new SingleResponse<TResponse> { IsValid = false, Errors = new List<string> { "Failed to deserialize successful response." } };
                }

                try
                {
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<SingleResponse<bool>>(responseContent);
                    if (errorResponse != null && (errorResponse.Errors?.Any() ?? false))
                    {
                        return new SingleResponse<TResponse>
                        {
                            Data = default,
                            IsValid = false,
                            Errors = errorResponse.Errors
                        };
                    }
                }
                catch {  }

                return new SingleResponse<TResponse>
                {
                    IsValid = false,
                    Errors = new List<string> { $"API request failed with status {response.StatusCode}.", responseContent }
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ApiServiceClient: HTTP Request error during PUT to {url}: {ex.Message}");
                return new SingleResponse<TResponse> { IsValid = false, Errors = new List<string> { $"Network error: {ex.Message}" } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiServiceClient: Unexpected error during PUT to {url}: {ex.Message}");
                return new SingleResponse<TResponse> { IsValid = false, Errors = new List<string> { $"An unexpected error occurred: {ex.Message}" } };
            }
        }

        public async Task<(bool, string)> CheckApiAddressAsync(string address)
        {
            var url = "settings/check";
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
        
        public async Task<string> LoadSettingAsync(string key, string user)
        {
            var url = $"settings/get?key={Uri.EscapeDataString(key)}&user={Uri.EscapeDataString(user)}";
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

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
            var url = "settings/set";
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
            return await _http.GetAsync("identity/check-session");
        }
        #endregion
    }
}
