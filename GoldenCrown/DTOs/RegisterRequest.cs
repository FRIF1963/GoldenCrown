using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Please enter your login")]
        [MinLength(3,ErrorMessage = "Minimal login length is 6")]
        public string login {  get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(6, ErrorMessage = "Minimal password length is 6")]
        public string password { get; set; }
    }
}
