using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker2.Models
{
    public class LogEntry
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }

        public DateTime Date { get; set; }
        public int? TimeSpent { get; set; }
        public string Notes { get; set; }
    }
}
