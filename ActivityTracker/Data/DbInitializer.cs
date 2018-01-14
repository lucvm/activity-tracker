using System;
using System.Linq;
using ActivityTracker.Models;

namespace ActivityTracker.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            //context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var users = new ApplicationUser[]
            {
                // Teachers
                new ApplicationUser{Id="1", FirstName="Teacher", Prefix="", LastName="1", Email="teacher@hotmail.com", UserType="T", Notes=""},
                new ApplicationUser{Id="2", FirstName="Jan", Prefix="", LastName="Janssen", Email="jan@hotmail.com", UserType="T", Notes=""},
                new ApplicationUser{Id="3", FirstName="Piet", Prefix="van", LastName="Dijk", Email="piet@gmail.com", UserType="T", Notes=""},
                // Students
                new ApplicationUser{Id="4", FirstName="Student", Prefix="", LastName="1", TeacherID="1", Email="student@hotmail.com", UserType="S", Notes=""},
                new ApplicationUser{Id="5", FirstName="Henk", Prefix="van de", LastName="Waard", TeacherID="1", Email="henk@hotmail.com", UserType="S", Notes="Lorem ipsum dolor sit amet"},
                new ApplicationUser{Id="6", FirstName="Marie", Prefix="", LastName="Smit", TeacherID="1", Email="marie@live.net", UserType="S", Notes=""},
                new ApplicationUser{Id="7", FirstName="Freek", Prefix="de", LastName="Jong", TeacherID="2", Email="freek@gmail.com", UserType="S", Notes=""},
                new ApplicationUser{Id="8", FirstName="Cornelia", Prefix="", LastName="Heiniken", TeacherID="2", Email="corrie@hotmail.com", UserType="S", Notes=""},
                new ApplicationUser{Id="9", FirstName="Piet", Prefix="", LastName="Heijn", TeacherID="3", Email="p.heijn@gmail.com", UserType="S", Notes=""},
                new ApplicationUser{Id="10", FirstName="Roos", Prefix="", LastName="Bloem", TeacherID="3", Email="roos@gmail.com", UserType="S", Notes=""},
            };
            foreach (ApplicationUser u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var groups = new Group[]
            {
                new Group{ID="1", Name="GR22"},
                new Group{ID="2", Name="C#"},
                new Group{ID="3", Name="GR23"},
            };
            foreach (Group g in groups)
            {
                context.Groups.Add(g);
            }
            context.SaveChanges();

            var userGroups = new UserGroup[]
            {
                new UserGroup{ApplicationUserID="4", GroupID="1"},
                new UserGroup{ApplicationUserID="4", GroupID="2"},
                new UserGroup{ApplicationUserID="5", GroupID="3"},
                new UserGroup{ApplicationUserID="6", GroupID="2"},
                new UserGroup{ApplicationUserID="7", GroupID="1"},
                new UserGroup{ApplicationUserID="8", GroupID="2"},
                new UserGroup{ApplicationUserID="9", GroupID="3"},
            };
            foreach (UserGroup ug in userGroups)
            {
                context.UserGroups.Add(ug);
            }
            context.SaveChanges();

            var activities = new Activity[]
            {
                new Activity{ApplicationUserID="4", Name="Lorem", Complete=true, FunFactor=2, Difficulty=4, Notes="Sit amet"},
                new Activity{ApplicationUserID="4", Name="Ipsum", Complete=false, FunFactor=3, Difficulty=1, Notes="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras feugiat quis lorem eu elementum. Sed ac purus quis elit sagittis porttitor quis eget dui."},
                new Activity{ApplicationUserID="4", Name="Dolor", Complete=true, FunFactor=4, Difficulty=4, Notes="Mauris ac volutpat eros. Cras sit amet varius libero"},
                new Activity{ApplicationUserID="5", Name="Lorem", Complete=true, FunFactor=1, Difficulty=4, Notes=""},
                new Activity{ApplicationUserID="6", Name="Lorem", Complete=false, FunFactor=2, Difficulty=5, Notes=""},
                new Activity{ApplicationUserID="7", Name="Lorem", Complete=false, FunFactor=0, Difficulty=0, Notes="Duis nec vestibulum ante"},
                new Activity{ApplicationUserID="7", Name="Ipsum", Complete=true, FunFactor=5, Difficulty=2, Notes=""},
            };
            foreach (Activity a in activities)
            {
                context.Activities.Add(a);
            }
            context.SaveChanges();

            var logEntries = new LogEntry[]
            {
                new LogEntry{ActivityID=1, Date=DateTime.Parse("2018-01-01"), TimeSpent=3, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=1, Date=DateTime.Parse("2018-01-02"), TimeSpent=8, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=1, Date=DateTime.Parse("2018-01-03"), TimeSpent=12, Notes=""},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-01"), TimeSpent=1, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-02"), TimeSpent=2, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-03"), TimeSpent=3, Notes=""},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-01"), TimeSpent=6, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-02"), TimeSpent=7, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-03"), TimeSpent=8, Notes=""},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-01"), TimeSpent=10, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-02"), TimeSpent=11, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-03"), TimeSpent=44, Notes=""},
            };
            foreach (LogEntry le in logEntries)
            {
                context.LogEntries.Add(le);
            }
            context.SaveChanges();
        }
    }
}
