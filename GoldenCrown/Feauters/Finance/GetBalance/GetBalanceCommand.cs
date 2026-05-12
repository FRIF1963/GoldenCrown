using GoldenCrown.Database;
using GoldenCrown.Feauters.Finance.Deposit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.Finance.GetBalance
{
    public class GetBalanceCommand : IRequest<Result<decimal>>
    {
        public int UserId {  get; set; }

        public GetBalanceCommand(int userId)
        {
            UserId = userId;
        }
        public class GetBalanceCommandHandler : IRequestHandler<GetBalanceCommand, Result<decimal>>
        {
            public ApplicationDBContext _context { get; set; }

            public GetBalanceCommandHandler(ApplicationDBContext context)
            {
                _context = context;
            }

            public async Task<Result<decimal>> Handle(GetBalanceCommand request, CancellationToken cancellationToken)
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
