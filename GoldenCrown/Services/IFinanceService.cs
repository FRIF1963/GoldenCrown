using GoldenCrown.Database;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IFinanceService
    {
        Task<Result<decimal>> GetBalanceAsync(int userId);
        Task<Result> TransferAsync(int userId, string receiverLogin, decimal amount);

        Task<Result> DepositAsync(int userId,decimal amount);
        Task<Result<IEnumerable<TransactionHistoryResponse>>> GetTransactionHistoryAsync(int userId, DateTime from, DateTime to, int ofset, int limit);
    }
}
