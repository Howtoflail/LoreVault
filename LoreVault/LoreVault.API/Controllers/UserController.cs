using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoreVault.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService tokenService)
        { 
            _userService = userService;
            _tokenService = tokenService;
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

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginWithGoogleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest("idToken is required.");
            }

            var principal = await _userService.VerifyGoogleTokenAsync(request.IdToken);

            if (principal == null)
            {
                return Unauthorized("Invalid token");
            }

            var googleId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;

            // Check if user already exists
            var user = await _userService.GetUserByGoogleId(googleId);

            if (user == null)
            {
                var userRequest = new CreateUserRequest { FirstName = firstName, LastName = lastName };

                user = await _userService.CreateUserWithGoogleId(userRequest, googleId);
            }

            var jwtToken = _tokenService.GenerateJwt(user, request.IdToken);

            //return Ok(user);
            return Ok(jwtToken);
        }
    }
}
