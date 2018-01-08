﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker2.Models
{
    public class Activity
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }

        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public int? TimeSpent { get; set; }
        public bool? Complete { get; set; }
        public int? FunFactor { get; set; }
        public int? Difficulty { get; set; }
        public string Notes { get; set; }

        public ICollection<LogEntry> Log { get; set; }
    }
}