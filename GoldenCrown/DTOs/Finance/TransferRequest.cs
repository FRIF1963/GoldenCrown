using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.Finance
{
    public class TransferRequest
    {
        [Required]
        public string Token {  get; set; }

        [Required]
        public string ReceiverLogin { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
