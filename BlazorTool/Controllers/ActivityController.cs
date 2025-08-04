using BlazorTool.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorTool.Client.Models;
using System.Text;
using Newtonsoft.Json;


namespace BlazorTool.Controllers
{
    [Route("api/v1/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ServerAuthTokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ActivityController(ServerAuthTokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("getlist")]
        public async Task<IActionResult> GetList([FromQuery] int woID, [FromQuery] string lang = "pl-pl")
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient");
                var request = new HttpRequestMessage(HttpMethod.Get, $"act/getlist?woID={woID}&lang={lang}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = new[] { ex.Message } });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AddActivity activity)
        {
            if (activity == null)
            {
                return BadRequest("Activity data is null.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApiBearerAuthClient");
                var jsonContent = JsonConvert.SerializeObject(activity);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("act/create", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorBody);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = new[] { ex.Message } });
            }
        }
    }
}
