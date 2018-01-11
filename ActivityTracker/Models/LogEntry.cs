using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityTracker.Models
{
    public class LogEntry
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }
        public int? TimeSpent { get; set; }
        public string Notes { get; set; }
    }
}
