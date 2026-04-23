using GoldenCrown.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IFinanceService
    {
        Task<Result<decimal>> GetBalance(string token);
        Task<Result> Transfer(string token, string receiverLogin, decimal amount);
    }
}
