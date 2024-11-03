using LoreVault.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoreVault.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        { 
            _userService = userService;
        }

        [HttpGet("get-user-by-id-private")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id) 
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpGet("get-user-by-id-private-scoped")]
        [Authorize("read:messages")]
        public async Task<IActionResult> GetUserByIdScoped(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("get-users-private")]
        public async Task<IActionResult> GetUsersPrivateAsync()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [Authorize(Policy = "read:users")]
        [HttpGet("get-users-private-scoped")]
        public async Task<IActionResult> GetUsersPrivateScopedAsync()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] Domain.Models.User user)
        {
            await _userService.CreateUser(user);
            return Ok();
        }
    }
}
