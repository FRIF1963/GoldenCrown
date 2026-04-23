using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDBContext _context;

        public AccountService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Result> CreateAccountAsync(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user == null) return Result.Failure($"Not find a user with login {login}");

            
            var account = new Account
            {
                UserId = user.Id,
                Balance = 0,
            };
            
            _context.Add(account);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
