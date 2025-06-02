using System.ComponentModel.DataAnnotations;

namespace CMS.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be {2} and the max {1} character long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [Compare("ConfirmNewPassword", ErrorMessage = "The New password and confirmation password do not match.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }
    }
}
