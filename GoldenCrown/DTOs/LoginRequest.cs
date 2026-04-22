using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Please enter your login")]
        public string login { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string password { get; set; }
    }
}
