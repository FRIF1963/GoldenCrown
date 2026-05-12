using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using GoldenCrown.Feauters.User.UserLogin;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.Finance.Deposit
{
    public class DepositCommand : IRequest<Result>
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public DepositCommand(int userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
        }

        public class DepositCommandHandler : IRequestHandler<DepositCommand, Result>
        {
            public ApplicationDBContext _context;
            public DepositCommandHandler(ApplicationDBContext context)
            {
                _context = context;
            }
            public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (account == null)
                {
                    return Result.Failure($"Not Found");
                }

                account.Balance += request.Amount;

                var transaction = new Transaction
                {
                    SenderAccountId = account.Id,
                    ReceiverAccountId = account.Id,
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
