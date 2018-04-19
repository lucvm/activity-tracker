using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
