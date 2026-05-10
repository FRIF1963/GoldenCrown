using FluentValidation;
using GoldenCrown.DTOs.Finance;

namespace GoldenCrown.Validators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
            public DepositRequestValidator()
            {
                RuleFor(DepositRequest => DepositRequest.amount).NotEmpty().
                GreaterThan(0).WithMessage("Еhe amount must be greater than 0");
            }
    }
}
