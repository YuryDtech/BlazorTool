using BlazorTool.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlazorTool.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WoController : ControllerBase
    {
        private readonly ApiServiceClient _apiServiceClient;

        public WoController(ApiServiceClient apiServiceClient)
        {
            _apiServiceClient = apiServiceClient;
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] WorkOrderQueryParameters q)
        {
            try
            {
                var data = await _apiServiceClient.GetWorkOrdersAsync(
                    deviceID: q.DeviceID,
                    workOrderID: q.WorkOrderID,
                    deviceName: q.DeviceName,
                    isDep: q.IsDep,
                    isTakenPerson: q.IsTakenPerson,
                    active: q.Active,
                    lang: q.Lang,
                    personID: q.PersonID,
                    isPlan: q.IsPlan,
                    isWithPerson: q.IsWithPerson
                );

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

        public class WorkOrderQueryParameters
        {
            public int? DeviceID { get; set; }
            public int? WorkOrderID { get; set; }
            public string? DeviceName { get; set; }
            public bool? IsDep { get; set; }
            public bool? IsTakenPerson { get; set; }
            public bool? Active { get; set; }
            public string Lang { get; set; } = "pl-PL";
            public int? PersonID { get; set; }
            public bool? IsPlan { get; set; }
            public bool? IsWithPerson { get; set; }
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
