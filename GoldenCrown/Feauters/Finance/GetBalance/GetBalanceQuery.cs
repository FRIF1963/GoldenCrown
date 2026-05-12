using GoldenCrown.Database;
using GoldenCrown.Feauters.Finance.Deposit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.Finance.GetBalance
{
    public class GetBalanceQuery : IRequest<Result<decimal>>
    {
        public int UserId {  get; set; }

        public GetBalanceQuery(int userId)
        {
            UserId = userId;
        }
        public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, Result<decimal>>
        {
            public ApplicationDBContext _context { get; set; }

            public GetBalanceQueryHandler(ApplicationDBContext context)
            {
                _context = context;
            }

            public async Task<Result<decimal>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (account == null)
                {
                    return Result<decimal>.Failure("Account Not Found");
                }

                return Result<decimal>.Success(account!.Balance);
            }
        }
    }
}
