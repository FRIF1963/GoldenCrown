using FluentValidation;
using GoldenCrown.DTOs.User;
namespace GoldenCrown.Validators

{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(RegisterRequest => RegisterRequest.Login).NotEmpty().MinimumLength(6).WithMessage("Minimal login length is {MinLength}");

            RuleFor(RegisterRequest => RegisterRequest.Name).NotEmpty().WithMessage("Please enter your name");

            RuleFor(RegisterRequest => RegisterRequest.Password).NotEmpty().MinimumLength(6).WithMessage("Minimal password length is 6");
        }
    }
}
