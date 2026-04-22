namespace GoldenCrown.DTOs
{
    public class TransferRequest
    {
        public string token {  get; set; }

        public string receiverLogin { get; set; }

        public decimal amount { get; set; }
    }
}
