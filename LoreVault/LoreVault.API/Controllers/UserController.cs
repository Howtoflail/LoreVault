using Google.Apis.Auth;
using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        //[HttpPost("create-user")]
        //public async Task<IActionResult> CreateUser([FromBody] Domain.Models.CreateUserRequest userRequest)
        //{
        //    await _userService.CreateUser(userRequest);
        //    return Ok();
        //}

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string idToken)
        {
            var googleUser = await VerifyGoogleTokenAsync(idToken);

            if (googleUser == null) 
            {
                return Unauthorized("Invalid token");
            }

            // Check if user already exists
            var user = await _userService.GetUserByGoogleId(googleUser.Subject);
            if (user == null)
            {
                var userRequest = new CreateUserRequest { FirstName = googleUser.GivenName, LastName = googleUser.FamilyName };

                await _userService.CreateUserWithGoogleId(userRequest, googleUser.Subject);
            }
                 
            return Ok(user);
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync(idToken);

            return JsonConvert.DeserializeObject<GoogleJsonWebSignature.Payload>(response);
        }
    }
}
