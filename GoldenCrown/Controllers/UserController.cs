using FluentValidation;
using GoldenCrown.DTOs.User;
using GoldenCrown.Feauters.User.UserLogin;
using GoldenCrown.Validators;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("register")] // Post localhost:7777/api/user/request
        public async Task<IActionResult> Register([FromBody] DTOs.User.RegisterRequest request, [FromServices] IValidator<DTOs.User.RegisterRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new UserRegisterCommand(request.Login, request.Name, request.Password);
            var result = await _mediator.Send(command);

            if (result) return Ok();

            return BadRequest();
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.User.LoginRequest request, [FromServices] IValidator<DTOs.User.LoginRequest> validator)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new UserLoginCommand(request.Login, request.Password);
            var result = await _mediator.Send(command);

            if(result) return Ok(result);

            return Unauthorized();

        }
    }
}
