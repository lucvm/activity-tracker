using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker.Models
{
    public class Group
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }
    }
}
