using BlazorTool.Client.Models;
using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ApiServiceClient _apiServiceClient;

        public DeviceController(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                // берём список устройств из ApiServiceClient
                var data = await _apiServiceClient.GetAllDevicesCachedAsync();
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
