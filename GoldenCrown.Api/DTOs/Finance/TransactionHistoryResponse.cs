namespace GoldenCrown.DTOs.Finance
{
    public class TransactionHistoryResponse
    {
        public string SenderName {  get; set; }

        public string ReceiverName { get; set; }

        public DateTime CreateAt { get; set; }

        public decimal Amoutn  { get; set; }

        public string Currency {  get; set; }
    }
}
