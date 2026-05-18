using FluentValidation;
using GoldenCrown.Api.DTOs.Finance;
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
