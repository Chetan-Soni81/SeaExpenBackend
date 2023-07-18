using System.ComponentModel.DataAnnotations;

namespace SeaExpenBackend.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        [Required (ErrorMessage = "Username is required.")]
        public string? Username { get; set; }
        [EmailAddress]
        [Required (ErrorMessage ="Email is required.")]
        public string? Email { get; set; }
        [Required (ErrorMessage ="Password is required.")]
        public string? Password { get; set; }
        public int? Role { get; set; }
        public string? RoleName { get; set; }
        public bool? IsActive { get; set; }
    }
}
