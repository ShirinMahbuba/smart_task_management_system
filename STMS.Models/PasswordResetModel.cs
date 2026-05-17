using System.ComponentModel.DataAnnotations;

namespace STMS.Models
{
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(255, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
