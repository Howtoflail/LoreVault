using Microsoft.AspNetCore.Mvc;

namespace LoreVault.API.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthCheck : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Backend is running!");
        }
    }
}