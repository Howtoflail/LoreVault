using Microsoft.AspNetCore.Mvc;

namespace LoreVault.API.Controllers
{
    [ApiController]
    [Route("Health")]
    public class SecondHealthCheck : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Health!");
        }
    }
}