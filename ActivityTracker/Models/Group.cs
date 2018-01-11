using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker.Models
{
    public class Group
    {
        public string ID { get; set; }
        [ForeignKey("ApplicationUserId")]
        public string OwnerID { get; set; }

        public string Name { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }
    }
}
