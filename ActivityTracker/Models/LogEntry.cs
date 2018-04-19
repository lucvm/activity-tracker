using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models
{
    public class LogEntry
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        public float? TimeSpent { get; set; }
        public string Notes { get; set; }
    }
}
