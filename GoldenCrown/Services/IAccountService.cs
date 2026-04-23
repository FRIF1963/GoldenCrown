using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IAccountService
    {
        Task<Result> CreateAccountAsync(string login);
    }
}
