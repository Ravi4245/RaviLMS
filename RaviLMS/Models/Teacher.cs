using System.ComponentModel.DataAnnotations;

namespace RaviLMS.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        [Required(ErrorMessage ="Full name is Required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Email Id Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
    }
}
