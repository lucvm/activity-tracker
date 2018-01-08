﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker2.Models
{
    public class Group
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }
    }
}