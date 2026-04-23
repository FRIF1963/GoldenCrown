using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IFinanceService
    {
        Task<Result<decimal>> GetBalanceAsync(string token);
        Task<Result> TransferAsync(string token, string receiverLogin, decimal amount);

        Task<Result> DepositAsync(string token, decimal amount);
        Task<Result<List<Transaction>>> GetHistoryAsync(string token, DateTime from, DateTime to, int ofset, int limit);
    }
}
