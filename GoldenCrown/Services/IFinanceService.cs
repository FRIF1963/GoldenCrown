using GoldenCrown.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IFinanceService
    {
        Task<decimal> GetBalance(string token);
    }

    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDBContext _context;

        public FinanceService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetBalance(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

            if (session == null) throw new InvalidOperationException("Session expired");

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

            if (account == null) throw new InvalidOperationException("Account Not Found");

            return account.Balance;
        }
    }
}
