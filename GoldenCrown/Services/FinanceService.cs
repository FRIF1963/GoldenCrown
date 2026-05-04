using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;

namespace GoldenCrown.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDBContext _context;

        public FinanceService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Result<decimal>> GetBalanceAsync(int userId)
        {

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                return Result<decimal>.Failure("Account Not Found");
            }

            return Result<decimal>.Success(account!.Balance);
        }

        public async Task<Result> DepositAsync(int userId, decimal amount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

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

        public async Task<Result> TransferAsync(int userId, string receiverLogin, decimal amount)
        {
            var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == receiverLogin);

            if (receiverUser == null)
            {
                return Result.Failure($"{receiverLogin} Not Found");
            }

            var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

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

        public async Task<Result<IEnumerable<TransactionHistoryResponse>>> GetTransactionHistoryAsync(int userId, DateTime from, DateTime to, int ofset, int limit)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                return Result<IEnumerable<TransactionHistoryResponse>>.Failure($"Not Found");
            }

            var transactions = await _context.Transactions
                .Where(t =>  (t.SenderAccountId == account.Id ||t.ReceiverAccountId == account.Id)
                && t.CreateAt >= from && t.CreateAt <= to)
                .Skip(ofset)
                .Take(limit)
                .ToListAsync();

            var transactionResult = new List<TransactionHistoryResponse>();

            var allSenderUsers = transactions.Select(u => u.SenderAccountId);

            var allReceiverUsers = transactions.Select(u => u.ReceiverAccountId);

            var allUsers = allSenderUsers.ToHashSet();

            foreach (var reciever in allReceiverUsers)
            {
                allUsers.Add(reciever);
            }

            var names = await _context.Accounts.Where(a => allUsers.Contains(a.Id))
                    .Join(_context.Users,
                    acc => acc.UserId,
                    u => u.Id,
                    (acc, u) => new
                    { 
                        Name = u.Name,
                        AccId = acc.Id
                    }).ToDictionaryAsync(x => x.AccId);

            foreach (var transaction in transactions)
            {
                var senderName = names[transaction.SenderAccountId].Name;
                var ReceiverName = names[transaction.ReceiverAccountId].Name;
                transactionResult.Add(new TransactionHistoryResponse
                {
                    SenderName = senderName,
                    ReceiverName = ReceiverName,
                    CreateAt = transaction.CreateAt,
                    Amoutn = transaction.Amoutn,
                });
            }

            if (transactions == null)
            {
                return Result<IEnumerable<TransactionHistoryResponse>>.Failure("Not Find Transactions");
            }

            return Result<IEnumerable<TransactionHistoryResponse>>.Success(transactionResult);
        }
    }
}
