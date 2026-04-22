using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string login, string name, string password);
        
        Task<string> LoginAsync(string login, string password);
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

        public async Task<string> LoginAsync(string login, string password)
        {
            //Найти пользователя по логину 
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) throw new InvalidOperationException($"Not find a user with login {login}");

            //Проверить пароль
            if (user.Password != password) throw new InvalidOperationException($"Incorrect password");

            //Создать новую сессию с токеном(использовать Guid.NewGuid().ToString())
            //Установить время истечения на 1 час от текущего момента
            var session = new Session
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };


            //Сохранить сессию в базу данных(убедиться, что она единственная для пользователя)
            var chekSession = await _context.Sessions.FirstOrDefaultAsync(s => s.UserId == user.Id && s.ExpiresAt > DateTime.UtcNow);
            if (chekSession != null) throw new InvalidOperationException($"User have an unfinished session");

            //Сохранить в базу данных
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            //Вернуть токен
            return session.Token;
        }

    }
}
