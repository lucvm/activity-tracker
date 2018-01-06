using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker2.Models
{
    public class UserGroup
    {
        public int ApplicationUserID { get; set; }
        public ApplicationUser Student { get; set; }

        public int GroupID { get; set; }
        public Group Group { get; set; }
    }
}
