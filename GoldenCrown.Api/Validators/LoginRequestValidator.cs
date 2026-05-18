using FluentValidation;
using GoldenCrown.DTOs.User;
namespace GoldenCrown.Validators

{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(LoginRequest => LoginRequest.Login).NotEmpty().WithMessage("Please enter your login");


            RuleFor(RegisterRequest => RegisterRequest.Password).NotEmpty().MinimumLength(6).WithMessage("Please enter your password");
        }
    }
}
