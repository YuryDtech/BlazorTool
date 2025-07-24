using BlazorTool.Client.Models; // Added for ApiResponse
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.Http; // Added
using System.Net.Http.Json; // Added

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory; // Changed

        public WoController(IHttpClientFactory httpClientFactory) // Changed
        {
            _httpClientFactory = httpClientFactory; // Changed
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] WorkOrderQueryParameters q)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient"); // Get named client

                var qp = new List<string>();
                if (q.DeviceID.HasValue) qp.Add($"DeviceID={q.DeviceID.Value}");
                if (q.WorkOrderID.HasValue) qp.Add($"WorkOrderID={q.WorkOrderID.Value}");
                if (!string.IsNullOrWhiteSpace(q.DeviceName)) qp.Add($"DeviceName={Uri.EscapeDataString(q.DeviceName)}");
                if (q.IsDep.HasValue) qp.Add($"IsDep={q.IsDep.Value}");
                if (q.IsTakenPerson.HasValue) qp.Add($"IsTakenPerson={q.IsTakenPerson.Value}");
                if (q.Active.HasValue) qp.Add($"Active={q.Active.Value}");
                if (!string.IsNullOrWhiteSpace(q.Lang)) qp.Add($"Lang={Uri.EscapeDataString(q.Lang)}");
                if (q.PersonID.HasValue) qp.Add($"PersonID={q.PersonID.Value}");
                if (q.IsPlan.HasValue) qp.Add($"IsPlan={q.IsPlan.Value}");
                if (q.IsWithPerson.HasValue) qp.Add($"IsWithPerson={q.IsWithPerson.Value}");

                var url = "wo/getlist?" + string.Join("&", qp);

                var response = await client.GetAsync(url); // Use the named client
                response.EnsureSuccessStatusCode();

                var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<WorkOrder>>();

                var data = wrapper?.Data ?? new List<WorkOrder>();

                return Ok(new
                {
                    data,
                    isValid = true,
                    errors = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                // TODO: logging ex
                return StatusCode(500, new
                {
                    data = Array.Empty<object>(),
                    isValid = false,
                    errors = new[] { ex.Message }
                });
            }
        }

        [HttpGet("getdict")]
        public async Task<IActionResult> GetWOCategories([FromQuery] WOCategoriesParameters q)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient"); // Get named client

                var qp = new List<string>();
                qp.Add($"PersonID={q.PersonID}");
                qp.Add($"Lang={q.Lang}");
                var url = "wo/getdict?" + string.Join("&", qp);

                var response = await client.GetAsync(url); // Use the named client
                response.EnsureSuccessStatusCode();

                var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Dict>>();

                var data = wrapper?.Data ?? new List<Dict>();

                return Ok(new
                {
                    data,
                    isValid = true,
                    errors = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    data = Array.Empty<object>(),
                    isValid = false,
                    errors = new[] { ex.Message }
                });
            }
        }



        // GET api/<WoController>/get?WorkOrderID=2
        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] int WorkOrderID)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient"); // Get named client
                string url = $"wo/get?WorkOrderID={WorkOrderID}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<SingleResponse<WorkOrderInfo>>();
                if (apiResponse == null || !apiResponse.IsValid)
                {
                    var errors = apiResponse?.Errors ?? new List<string> { "Unknown error occurred." };
                    return NotFound(new SingleResponse<WorkOrderInfo>
                    {
                        IsValid = false,
                        Errors = errors
                    });
                }
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST api/<WoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] WorkOrderCreateRequest createRequest)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient");
                var url = "wo/create";

                var response = await client.PostAsJsonAsync(url, createRequest);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<SingleResponse<WorkOrder>>();

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // TODO: logging ex
                return StatusCode(500, new SingleResponse<WorkOrder>
                {
                    IsValid = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // PUT api/<WoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPut("close")]
        public async Task<IActionResult> CloseWorkOrder([FromBody] CloseWorkOrderRequest request)
        {
            if (request == null || request.WorkOrderID <= 0)
            {
                return BadRequest(new SingleResponse<bool>
                {
                    IsValid = false,
                    Errors = new List<string> { "WorkOrderID is required." }
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient");
                var url = "wo/close";

                var response = await client.PutAsJsonAsync(url, request);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<SingleResponse<bool>>(responseContent);
                    return Ok(apiResponse);
                }
                else
                {
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<SingleResponse<bool>>(responseContent);
                        if (errorResponse != null)
                        {
                            return StatusCode((int)response.StatusCode, errorResponse);
                        }
                    }
                    catch { }

                    return StatusCode((int)response.StatusCode, new SingleResponse<bool>
                    {
                        IsValid = false,
                        Errors = new List<string> { $"API Error: {response.ReasonPhrase}", responseContent }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SingleResponse<bool>
                {
                    IsValid = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    

    

    [HttpPut("update")]
        public async Task<IActionResult> UpdateWorkOrder([FromBody] UpdateWorkOrderRequest request)
        {
            if (request == null || request.WorkOrderID <= 0)
            {
                return BadRequest(new SingleResponse<bool>
                {
                    IsValid = false,
                    Errors = new List<string> { "A valid WorkOrderID is required." }
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient");
                var url = "wo/update";

                var response = await client.PutAsJsonAsync(url, request);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<SingleResponse<bool>>(responseContent);
                    return Ok(apiResponse);
                }
                else
                {
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<SingleResponse<bool>>(responseContent);
                        if (errorResponse != null && errorResponse.Errors.Any())
                        {
                            return StatusCode((int)response.StatusCode, errorResponse);
                        }
                    }
                    catch {  }

                    return StatusCode((int)response.StatusCode, new SingleResponse<bool>
                    {
                        IsValid = false,
                        Errors = new List<string> { $"API Error: {response.ReasonPhrase}", responseContent }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SingleResponse<bool>
                {
                    IsValid = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        } }
}
