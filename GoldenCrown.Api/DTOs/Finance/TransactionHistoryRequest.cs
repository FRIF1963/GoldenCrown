
namespace GoldenCrown.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Ofset { get; set; }

        public int Limit { get; set; }
    }
}
