using FluentValidation;
using GoldenCrown.DTOs.Finance;

namespace GoldenCrown.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(TransferRequest => TransferRequest.ReceiverLogin).NotEmpty();

            RuleFor(TransferRequest => TransferRequest.Amount).NotEmpty().
                GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(TransferRequest => TransferRequest.Currency).NotEmpty();

            RuleFor(TransferRequest => TransferRequest.ReceiverCurrency).NotEmpty();
        }
    }
}
