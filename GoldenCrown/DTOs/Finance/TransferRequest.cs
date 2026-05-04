using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.Finance
{
    public class TransferRequest
    {

        [Required]
        public string ReceiverLogin { get; set; }

        [Range(0.01, double.MaxValue,ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
