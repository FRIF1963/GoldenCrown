using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs.User
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Please enter your login")]
        [MinLength(3,ErrorMessage = "Minimal login length is 6")]
        public string Login {  get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(6, ErrorMessage = "Minimal password length is 6")]
        public string Password { get; set; }
    }
}
