using BlazorTool.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http; // Added
using System.Net.Http.Json; // Added

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory; // Changed

        public DeviceController(IHttpClientFactory httpClientFactory) // Changed
        {
            _httpClientFactory = httpClientFactory; // Changed
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList(
                                        [FromQuery] string lang = "pl-pl",
                                        [FromQuery] string? name = null,
                                        [FromQuery] int? categoryID = null,
                                        [FromQuery] bool? isSet = null,
                                        [FromQuery] IEnumerable<int>? machineIDs = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient"); // Get named client

                var qp = new List<string>();
                if (!string.IsNullOrWhiteSpace(lang)) qp.Add($"Lang={Uri.EscapeDataString(lang)}");
                if (!string.IsNullOrWhiteSpace(name)) qp.Add($"Name={Uri.EscapeDataString(name)}");
                if (categoryID.HasValue) qp.Add($"CategoryID={categoryID.Value}");
                if (isSet.HasValue) qp.Add($"IsSet={isSet.Value}");
                if (machineIDs != null)
                    foreach (var id in machineIDs)
                        qp.Add($"MachineIDs={id}");

                var url = "api/v1/device/getlist" + (qp.Count > 0 ? "?" + string.Join("&", qp) : "");
                
                var response = await client.GetAsync(url); // Use the named client
                response.EnsureSuccessStatusCode();

                var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<Device>>();

                return Ok(new
                {
                    data = wrapper?.Data ?? new List<Device>(),
                    isValid = true,
                    errors = Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    data = Array.Empty<Device>(),
                    isValid = false,
                    errors = new[] { ex.Message }
                });
            }
        }
        

        // GET api/<WoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
    }
}
