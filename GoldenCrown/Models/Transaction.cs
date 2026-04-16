namespace GoldenCrown.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int SenderAccountId { get; set; }

        public int ReceiverAccountId { get; set; }

        public DateTime CreateAt { get; set; }

        public decimal Amoutn { get; set; }
    }
}
