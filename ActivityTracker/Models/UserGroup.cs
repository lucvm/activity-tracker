using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker.Models
{
    public class UserGroup
    {
        public string GroupID { get; set; }
        public string ApplicationUserID { get; set; }

        public Group Group { get; set; }
        public ApplicationUser Student { get; set; }
    }
}
