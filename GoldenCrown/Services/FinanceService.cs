using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;
using GoldenCrown.Database.Models;

namespace GoldenCrown.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDBContext _context;

        public FinanceService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Result<decimal>> GetBalanceAsync(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
            {
                return Result<decimal>.Failure("Session expired");
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

            if (account == null)
            {
                return Result<decimal>.Failure("Account Not Found");
            }

            return Result<decimal>.Success(account!.Balance);
        }

        public async Task<Result> DepositAsync(string token, decimal amount)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
            {
                return Result.Failure("Session expired");
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

            if (account == null)
            {
                return Result.Failure($"Not Found");
            }

            account.Balance += amount;

            var transaction = new Transaction
            {
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id,
                CreateAt = DateTime.UtcNow,
                Amoutn = amount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> TransferAsync(string token, string receiverLogin, decimal amount)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
            {
                return Result.Failure("Session expired");
            }

            var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == receiverLogin);

            if (receiverUser == null)
            {
                return Result.Failure($"{receiverLogin} Not Found");
            }

            var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

            if (senderAccount == null)
            {
                return Result.Failure($"Not Found");
            }

            var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == receiverUser.Id);

            if (receiverAccount == null)
            {
                return Result.Failure($"Not Found");
            }

            if (senderAccount.Balance < amount)
            {
                return Result.Failure($"Your balance is less than {amount}");
            }
            else if (senderAccount.Balance <= 0)
            {
                return Result.Failure("Your balance must be greater than 0");
            }

            senderAccount.Balance -= amount;

            receiverAccount.Balance += amount;


            var transaction = new Transaction
            {
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = receiverAccount.Id,
                CreateAt = DateTime.UtcNow,
                Amoutn = amount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<List<Transaction>>> GetHistoryAsync(string token, DateTime from, DateTime to, int ofset, int limit)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

            if (session == null)
            {
                return Result<List<Transaction>>.Failure("Session expired");
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

            if (account == null)
            {
                return Result<List<Transaction>>.Failure($"Not Found");
            }

            var transactions = await _context.Transactions
                .Where(t =>  (t.SenderAccountId == account.Id ||t.ReceiverAccountId == account.Id)
                && t.CreateAt >= from && t.CreateAt <= to)
                .Skip(ofset)
                .Take(limit)
                .ToListAsync();

            if(transactions == null)
            {
                return Result<List<Transaction>>.Failure("Not Find Transactions");
            }

            return Result<List<Transaction>>.Success(transactions);

        }
    }
}
