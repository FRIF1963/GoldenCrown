using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDBContext _context;
        private readonly IAccountService _accountService;

        public UserService(ApplicationDBContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        public async Task<Result<bool>> RegisterAsync(string login, string name, string password)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (existing != null)
            {
                return Result<bool>.Failure("Not Found");
            }

            var user = new User { Login = login, Name = name, Password = password };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _accountService.CreateAccountAsync(login);

            return Result<bool>.Success(true);

        }

        public async Task<Result<string>> LoginAsync(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return Result<string>.Failure($"Incorrect password or login"); 
            }

            var session = new Session
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };


            var exestingSession = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (exestingSession != null)
            {
                exestingSession.Token = session.Token;
                exestingSession.ExpiresAt = session.ExpiresAt;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
            }
            

            return Result<string>.Success(session.Token);
        }

    }
}
