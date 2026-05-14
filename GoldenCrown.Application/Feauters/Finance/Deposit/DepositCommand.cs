using GoldenCrown.Application;
using GoldenCrown.Database;
using GoldenCrown.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Feauters.Finance.Deposit
{
    public class DepositCommand : IRequest<Result>
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DepositCommand(int userId, decimal amount, string currency)
        {
            UserId = userId;
            Amount = amount;
            Currency = currency;
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
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Currency == request.Currency);

                if (account == null)
                {
                    return Result.Failure($"Account Not Found");
                }

                account.Balance += request.Amount;

                var transaction = new Transaction
                {
                    SenderAccountId = account.Id,
                    ReceiverAccountId = account.Id,
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
