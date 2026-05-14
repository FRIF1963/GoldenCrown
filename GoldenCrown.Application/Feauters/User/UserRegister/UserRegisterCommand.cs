using GoldenCrown.Api.Database.Models;
using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Feauters.User.UserLogin
{
    public class UserRegisterCommand : IRequest<Result<bool>>
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public UserRegisterCommand(string login, string name, string password)
        {
            Login = login;
            Name = name;
            Password = password;
        }

        public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result<bool>>
        {
            private readonly ApplicationDBContext _context;

            public UserRegisterCommandHandler(ApplicationDBContext context) 
            { 
                _context = context;
            }

            public async Task<Result<bool>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
                if (existing != null)
                {
                    return Result<bool>.Failure("Not Found");
                }

                var user = new Database.Models.User { Login = request.Login, Name = request.Name, Password = request.Password };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                foreach (var currency in new List<string> { Currency.RUB, Currency.USD, Currency.EUR })
                {
                    var account = new Account
                    {
                        UserId = user.Id,
                        Balance = 0,
                        Currency = currency
                    };
                    _context.Add(account);
                }
                await _context.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
        }
    }
}
