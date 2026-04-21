using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string login, string name, string password);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDBContext _context;
        private readonly IAccountService  _accountService;

        public UserService(ApplicationDBContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        public async Task<bool> RegisterAsync(string login, string name, string password)
        {
            //Проверить, существует ли пользователь с таким логином
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if(existing != null)
            {
                return false;
            }

            //Проверить сложность пароля(минимум 6 символов)
            if(string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return false;
            }

            //Создать нового пользователя
            var user = new User { Login = login, Name = name, Password = password};
            

            //Сохранить в базу данных
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _accountService.CreateAccountAsync(login);

            //Вернуть результат(успех / ошибка)

            return true;

        }

    }
}
