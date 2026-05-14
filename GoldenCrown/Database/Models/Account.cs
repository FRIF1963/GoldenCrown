namespace GoldenCrown.Database.Models
{
    public class Account
    {
        public int Id { get; set; }

        public decimal Balance { get; set; }

        public int UserId { get; set; }

        public string Currency {  get; set; }
        //public User? User { get; set; }

    }
}
