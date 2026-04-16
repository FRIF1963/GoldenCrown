namespace GoldenCrown.Models
{
    public class Session
    {
        public int UserId {  get; set; }

        //public User? User { get; set; } 

        public string Token { get; set; }

        public DateTime ExpiresAt  { get; set; }
    }
}
