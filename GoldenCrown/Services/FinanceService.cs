using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDBContext _context;

        public FinanceService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Result<decimal>> GetBalance(string token)
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
    }
}
