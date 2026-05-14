using FluentValidation;
using GoldenCrown.Api.Database.Models;
using GoldenCrown.Api.DTOs.Finance;
using GoldenCrown.DTOs.User;
namespace GoldenCrown.Validators

{
    public class BalanceRequestValidator : AbstractValidator<BalanceRequest>
    {
        public BalanceRequestValidator()
        {
            RuleFor(CurrencyRequest => CurrencyRequest.Currency).NotEmpty().WithMessage("Please enter currency");
        }
    }
}
