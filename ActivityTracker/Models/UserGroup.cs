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
