using BlazorTool.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/act/getlist?woID={woID}&lang={lang}");

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
    }
}
