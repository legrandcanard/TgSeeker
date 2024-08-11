using System.ComponentModel.DataAnnotations;

namespace TgSeeker.Web.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
