using System.ComponentModel.DataAnnotations;

namespace SeaExpenBackend.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage ="Username is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
