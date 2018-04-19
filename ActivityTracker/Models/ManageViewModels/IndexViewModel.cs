using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTracker.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Remote("UsernameExists", "Account", HttpMethod = "POST", ErrorMessage = "Username already registered.")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string StatusMessage { get; set; }
    }
}
