using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
