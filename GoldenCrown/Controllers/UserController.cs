using GoldenCrown.DTOs;
using GoldenCrown.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")] // Post localhost:7777/api/user/request
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(request.login, request.name, request.password);

            if (result) return Ok();

            return BadRequest(new { Message = "User registration failed" });
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.LoginAsync(request.login, request.password);

            if(result != null) return Ok(result);

            return Unauthorized();

        }
    }
}
