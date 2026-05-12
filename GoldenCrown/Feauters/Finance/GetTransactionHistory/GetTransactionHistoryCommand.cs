using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Feauters.Finance.Deposit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.Finance.GetTransactionHistory
{
    public class GetTransactionHistoryCommand : IRequest<Result<IEnumerable<TransactionHistoryResponse>>>
    {
        public int UserId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Ofset { get; set; }
        public int Limit { get; set; }

        public GetTransactionHistoryCommand(int userId, DateTime from, DateTime to, int ofset, int limit)
        {
            UserId = userId;
            From = from;
            To = to;
            Ofset = ofset;
            Limit = limit;
        }
        public class GetTransactionHistoryCommandHandler : IRequestHandler<GetTransactionHistoryCommand, Result<IEnumerable<TransactionHistoryResponse>>>
        {
            public ApplicationDBContext _context { get; set; }

            public GetTransactionHistoryCommandHandler(ApplicationDBContext context)
            {
                _context = context;
            }

            public async Task<Result<IEnumerable<TransactionHistoryResponse>>> Handle(GetTransactionHistoryCommand request, CancellationToken cancellationToken)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (account == null)
                {
                    return Result<IEnumerable<TransactionHistoryResponse>>.Failure($"Not Found");
                }

                var transactions = await _context.Transactions
                    .Where(t => (t.SenderAccountId == account.Id || t.ReceiverAccountId == account.Id)
                    && t.CreateAt >= request.From && t.CreateAt <= request.To)
                    .Skip(request.Ofset)
                    .Take(request.Limit)
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
}

