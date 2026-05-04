using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.Finance
{
    public class DepositRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Еhe amount must be greater than 0")]
        public decimal amount { get; set; }
    }
}
