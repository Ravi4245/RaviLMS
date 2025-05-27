using System.ComponentModel.DataAnnotations;

namespace RaviLMS.Models
{
    public class LoginRequest
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Status is required")]
        public string Role { get; set; }
    }
}
