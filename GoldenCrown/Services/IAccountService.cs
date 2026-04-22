using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IAccountService
    {
        Task CreateAccountAsync(string login);
    }

    public class AccountService : IAccountService
    {
        private readonly ApplicationDBContext _context;

        public AccountService(ApplicationDBContext context) 
        {
            _context = context;
        }
        public async Task CreateAccountAsync(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if(user == null) throw new InvalidOperationException($"Not find a user with login {login}");

            //Создать новый счет для пользователя с балансом 0
            var account = new Account {
                UserId = user.Id,
                Balance = 0,
            };
            //Сохранить в базу данных
            _context.Add(account);
            await _context.SaveChangesAsync();

            //Вызывать этот метод в UserService.Register после создания пользователя


        }
    }
}
