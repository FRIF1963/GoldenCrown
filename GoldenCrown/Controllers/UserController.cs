using FluentValidation;
using GoldenCrown.DTOs.User;
using GoldenCrown.Services;
using GoldenCrown.Validators;
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
        public async Task<IActionResult> Register([FromBody] DTOs.User.RegisterRequest request, [FromServices] IValidator<DTOs.User.RegisterRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var result = await _userService.RegisterAsync(request.Login, request.Name, request.Password);

            if (result) return Ok();

            return BadRequest(new { Message = "User registration failed" });
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.User.LoginRequest request, [FromServices] IValidator<DTOs.User.LoginRequest> validator)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var result = await _userService.LoginAsync(request.Login, request.Password);

            if(result) return Ok(result);

            return Unauthorized();

        }
    }
}
