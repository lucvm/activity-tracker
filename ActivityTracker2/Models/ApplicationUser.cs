﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ActivityTracker2.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int? TeacherID { get; set; }

        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string Prefix { get; set; }
        public string LastName { get; set; }
        public string Notes { get; set; }
        public DateTime LastActive { get; set; }

        public ICollection<Activity> Activities { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<ApplicationUser> Students { get; set; }
    }
}
