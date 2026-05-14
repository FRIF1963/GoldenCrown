using GoldenCrown.Application;
using GoldenCrown.Database;
using GoldenCrown.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Feauters.User.UserLogin
{
    public class UserLoginCommand : IRequest<Result<string>>
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public UserLoginCommand(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<string>>
        {
            private readonly ApplicationDBContext _context;

            public UserLoginCommandHandler(ApplicationDBContext context) 
            { 
                _context = context; 
            }

            public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login && u.Password == request.Password);
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
}
