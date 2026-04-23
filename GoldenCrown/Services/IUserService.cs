using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IUserService
    {
        Task<Result<bool>> RegisterAsync(string login, string name, string password);
        
        Task<Result<string>> LoginAsync(string login, string password);
    }
}
