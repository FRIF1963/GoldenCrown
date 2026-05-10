using FluentValidation;
using GoldenCrown.DTOs.Finance;

namespace GoldenCrown.Validators
{
    public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
    {
        public TransactionHistoryRequestValidator()
        {
            RuleFor(TransactionHistoryRequest => TransactionHistoryRequest.From).NotEmpty();

            RuleFor(TransactionHistoryRequest => TransactionHistoryRequest.To).NotEmpty();

            RuleFor(TransactionHistoryRequest => TransactionHistoryRequest.Ofset).NotEmpty();

            RuleFor(TransactionHistoryRequest => TransactionHistoryRequest.Limit).NotEmpty();
        }
    
    }
}
