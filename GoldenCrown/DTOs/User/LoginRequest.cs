using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Please enter your login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }
    }
}
