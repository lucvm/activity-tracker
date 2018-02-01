using System;
using System.Linq;
using ActivityTracker.Models;

namespace ActivityTracker.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var users = new ApplicationUser[]
            {
                // Teachers
                new ApplicationUser{FirstName ="Teacher", Prefix="", LastName="1",
                    Email="teacher@hotmail.com", UserName="teacher@hotmail.com",
                    NormalizedEmail="TEACHER@HOTMAIL.COM", NormalizedUserName="TEACHER@HOTMAIL.COM",
                    SecurityStamp="831e4741-d745-413a-ae08-111a604b4014", UserType ="T", Notes=""},
                new ApplicationUser{Id="2",  FirstName="Jan", Prefix="", LastName="Janssen", Email="jan@hotmail.com", UserType="T"},
                new ApplicationUser{Id="3", FirstName="Piet", Prefix="van", LastName="Dijk", Email="piet@gmail.com", UserType="T"},
                // Students
                new ApplicationUser{FirstName="Student", LastName="1", TeacherID="1", Email="student@hotmail.com", UserType="S"},
                new ApplicationUser{FirstName="Henk", Prefix="van de", LastName="Waard", TeacherID="1", Email="henk@hotmail.com", UserType="S", Notes="Lorem ipsum dolor sit amet"},
                new ApplicationUser{FirstName="Marie", LastName="Smit", TeacherID="1", Email="marie@live.net", UserType="S"},
                new ApplicationUser{FirstName="Freek", Prefix="de", LastName="Jong", TeacherID="2", Email="freek@gmail.com", UserType="S"},
                new ApplicationUser{FirstName="Cornelia", LastName="Heiniken", TeacherID="2", Email="corrie@hotmail.com", UserType="S"},
                new ApplicationUser{FirstName="Piet", LastName="Heijn", TeacherID="3", Email="p.heijn@gmail.com", UserType="S",},
                new ApplicationUser{FirstName="Roos", LastName="Bloem", TeacherID="3", Email="roos@gmail.com", UserType="S"},
            };
            var index = 1;
            foreach (ApplicationUser u in users)
            {
                u.Id = index.ToString(); index++;
                u.SecurityStamp = "831e4741-d745-413a-ae08-111a604b4014";
                u.PasswordHash = "AQAAAAEAACcQAAAAEFpdyIhZDeA6u6hUdeT+8BaJ8CvoCllT9xxr4VNvEa8GJMRRAaZOCZDh8D6jti3cIQ=="; // password: 1234567
                u.UserName = u.Email;
                u.NormalizedEmail = u.Email.ToUpper();
                u.NormalizedUserName = u.Email.ToUpper();
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
                new Activity{ApplicationUserID="5", Name="Lorem", Complete=true, FunFactor=1, Difficulty=4},
                new Activity{ApplicationUserID="6", Name="Lorem", Complete=false, FunFactor=2, Difficulty=5},
                new Activity{ApplicationUserID="7", Name="Lorem", Complete=false, FunFactor=0, Difficulty=0, Notes="Duis nec vestibulum ante"},
                new Activity{ApplicationUserID="7", Name="Ipsum", Complete=true, FunFactor=5, Difficulty=2},
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
                new LogEntry{ActivityID=1, Date=DateTime.Parse("2018-01-03"), TimeSpent=12},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-01"), TimeSpent=1, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-02"), TimeSpent=2, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=2, Date=DateTime.Parse("2018-01-03"), TimeSpent=3},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-01"), TimeSpent=6, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-02"), TimeSpent=7, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=3, Date=DateTime.Parse("2018-01-03"), TimeSpent=8},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-01"), TimeSpent=10, Notes="Lorem ipsum"},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-02"), TimeSpent=11, Notes="Dolor sit amet"},
                new LogEntry{ActivityID=5, Date=DateTime.Parse("2018-01-03"), TimeSpent=44},
            };
            foreach (LogEntry le in logEntries)
            {
                context.LogEntries.Add(le);
            }
            context.SaveChanges();
        }
    }
}
