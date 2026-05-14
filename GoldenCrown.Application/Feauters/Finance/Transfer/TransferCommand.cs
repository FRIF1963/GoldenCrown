using GoldenCrown.Application;
using GoldenCrown.Database;
using GoldenCrown.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Feauters.Finance.Transfer
{
    public class TransferCommand : IRequest<Result>
    {
        public int UserId { get; set; }

        public string ReceiverLogin { get; set; }

        public decimal Amount { get; set; }

        public string Currency {  get; set; }

        public TransferCommand(int userId, string receiverLogin, decimal amount, string currency)
        {
            UserId = userId;
            ReceiverLogin = receiverLogin;
            Amount = amount;
            Currency = currency;
        }

        public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
        {
            public ApplicationDBContext _context;

            public TransferCommandHandler(ApplicationDBContext context) 
            { 
                _context = context;
            }

            public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
            {
                var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.ReceiverLogin, cancellationToken);

                if (receiverUser == null)
                {
                    return Result.Failure($"{request.ReceiverLogin} Not Found");
                }

                var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Currency == request.Currency, cancellationToken);

                if (senderAccount == null)
                {
                    return Result.Failure($"Not Found");
                }

                var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == receiverUser.Id && a.Currency == request.Currency, cancellationToken);

                if (receiverAccount == null)
                {
                    return Result.Failure($"Not Found");
                }

                if (senderAccount.Balance < request.Amount)
                {
                    return Result.Failure($"Your balance is less than {request.Amount}");
                }
                else if (senderAccount.Balance <= 0)
                {
                    return Result.Failure("Your balance must be greater than 0");
                }

                senderAccount.Balance -= request.Amount;

                receiverAccount.Balance += request.Amount;


                var transaction = new Transaction
                {
                    SenderAccountId = senderAccount.Id,
                    ReceiverAccountId = receiverAccount.Id,
                    CreateAt = DateTime.UtcNow,
                    Amount = request.Amount,
                    Currency = request.Currency
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
