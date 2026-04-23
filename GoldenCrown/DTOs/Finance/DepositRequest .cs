using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.Finance
{
    public class DepositRequest
    {
        [FromQuery]
        [Required(ErrorMessage = "Token has necessarily")]
        public string token { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Еhe amount must be greater than 0")]
        public decimal amount { get; set; }
    }
}
