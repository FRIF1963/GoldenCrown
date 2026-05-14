using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Feauters.Finance.Deposit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.Finance.GetTransactionHistory
{
    public class GetTransactionHistoryQuery : IRequest<Result<IEnumerable<TransactionHistoryResponse>>>
    {
        public int UserId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Ofset { get; set; }
        public int Limit { get; set; }


        public GetTransactionHistoryQuery(int userId, DateTime from, DateTime to, int ofset, int limit)
        {
            UserId = userId;
            From = from;
            To = to;
            Ofset = ofset;
            Limit = limit;
        }
        public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<IEnumerable<TransactionHistoryResponse>>>
        {
            public ApplicationDBContext _context { get; set; }

            public GetTransactionHistoryQueryHandler(ApplicationDBContext context)
            {
                _context = context;
            }

            public async Task<Result<IEnumerable<TransactionHistoryResponse>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
            {
                var userAccountIds = await _context.Accounts
                    .Where(a => a.UserId == request.UserId)
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken);

                var transactions = _context.Transactions.Where(x =>
                userAccountIds.Contains(x.SenderAccountId) || userAccountIds.Contains(x.ReceiverAccountId));

                if(request.From != null)
                {
                    transactions = transactions.Where(x => x.CreateAt >= request.From);
                }

                if (request.To != null)
                {
                    transactions = transactions.Where(x => x.CreateAt <= request.To);
                }

                transactions = transactions.Skip(request.Ofset).Take(request.Limit);

                var dbTransactions = await transactions.ToListAsync(cancellationToken);

                var allAccountIds = dbTransactions.Select(x => x.SenderAccountId)
                    .Concat(dbTransactions.Select(x => x.ReceiverAccountId))
                    .ToHashSet();

                var names = await _context.Accounts
                    .Where(x => allAccountIds.Contains(x.Id))
                    .Join(_context.Users, 
                    acc => acc.UserId,
                    u => u.Id,
                    (acc, u) => new
                    {
                        Name = u.Name,
                        AccId = acc.Id,
                    })
                    .ToDictionaryAsync(x => x.AccId, cancellationToken);

                var result = dbTransactions.Select(t => new TransactionHistoryResponse
                {
                    SenderName = names[t.SenderAccountId].Name,
                    ReceiverName = names[t.ReceiverAccountId].Name,
                    Amoutn = t.Amoutn,
                    CreateAt = t.CreateAt,
                    Currency = t.Currency,

                }).ToList();


                return Result<IEnumerable<TransactionHistoryResponse>>.Success(result);
            }
        }
    }
}

