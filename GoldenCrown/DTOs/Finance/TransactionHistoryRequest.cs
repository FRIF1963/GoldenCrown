using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }

        [Required]
        public int Ofset { get; set; }

        [Required]
        public int Limit { get; set; }
    }
}
