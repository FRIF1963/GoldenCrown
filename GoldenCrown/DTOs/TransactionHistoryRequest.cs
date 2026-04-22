namespace GoldenCrown.DTOs
{
    public class TransactionHistoryRequest
    {
        public string token {  get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public int ofset { get; set; }

        public int limit { get; set; }
    }
}
