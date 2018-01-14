using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ActivityTracker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("ApplicationUserId")]
        public string TeacherID { get; set; }

        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string Prefix { get; set; }
        public string LastName { get; set; }
        public string Notes { get; set; }

        public ICollection<Activity> Activities { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<ApplicationUser> Students { get; set; }
    }
}
