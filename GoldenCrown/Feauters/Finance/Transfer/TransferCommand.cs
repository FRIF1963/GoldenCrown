using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace GoldenCrown.Feauters.Finance.Transfer
{
    public class TransferCommand : IRequest<Result>
    {
        public int UserId { get; set; }

        public string ReceiverLogin { get; set; }

        public decimal Amount { get; set; }

        public TransferCommand(int userId, string receiverLogin, decimal amount)
        {
            UserId = userId;
            ReceiverLogin = receiverLogin;
            Amount = amount;
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
                var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.ReceiverLogin);

                if (receiverUser == null)
                {
                    return Result.Failure($"{request.ReceiverLogin} Not Found");
                }

                var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (senderAccount == null)
                {
                    return Result.Failure($"Not Found");
                }

                var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == receiverUser.Id);

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
                    Amoutn = request.Amount
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
