using System.Collections.Generic;

namespace ActivityTracker.Models
{
    public class Activity
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }

        public string Name { get; set; }
        public bool Complete { get; set; }
        public int? FunFactor { get; set; }
        public int? Difficulty { get; set; }
        public string Notes { get; set; }

        public ApplicationUser Student { get; set; }

        public ICollection<LogEntry> Log { get; set; }

        public static bool AuthorizeActivityUser(Activity activity, ApplicationUser currentUser)
        {
            if (currentUser.UserType == "S")
            {
                if (activity.ApplicationUserID != currentUser.Id)
                {
                    return false;
                }
                else
                    return true;
            }
            else
            {
                if (currentUser.Id != activity.Student.TeacherID)
                {
                    return false;
                }
                else
                    return true;
            }
        }
    }
}
