using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ActivityTracker.Data;
using ActivityTracker.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ActivityTracker.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivitiesController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IQueryable<Activity> GetAllActivities()
        {
            return _context.Activities.AsQueryable();
        }

        public IQueryable<ApplicationUser> GetAllStudents()
        {
            return _context.ApplicationUsers.AsQueryable();
        }

        // GET: Activities
        public async Task<IActionResult> Index(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            string studentId;

            ViewBag.CurrentUserType = currentUser.UserType;

            if (currentUser.UserType == "S")
            {
                studentId = currentUser.Id;
                ViewBag.studentId = studentId;
            }
            else
            {
                try
                {
                    var student = GetAllStudents().Where(au => au.Id == id).ToList()[0];
                    studentId = id;
                    ViewBag.studentId = studentId;
                    ViewBag.StudentName = String.Format("{0} {1} {2}", student.FirstName, student.Prefix, student.LastName);
                    ViewBag.Groups = new List<Group>();
                    var allGroups = await _context.Groups
                        .Where(group => group.OwnerID == currentUser.Id)
                        .Include(group => group.UserGroups)
                            .ThenInclude(userGroup => userGroup.Student)
                        .ToListAsync();
                    foreach (var group in allGroups)
                    {
                        foreach (var userGroup in group.UserGroups)
                        {
                            if (userGroup.Student == student)
                            {
                                ViewBag.Groups.Add(group);
                            }
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            var activities = GetAllActivities().Where(a => a.ApplicationUserID == studentId).OrderBy(a => a.Name).
                OrderBy(a => a.Complete).ToList();

            foreach (var activity in activities)
            {
                if (activity.Notes != null)
                {
                    activity.Notes = activity.Notes.Split(new[] { '.', '\r', '\n' }).FirstOrDefault() + '.';
                }
            }

            ViewBag.Activities = activities;

            return View();
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            ViewBag.StudentId = HttpContext.Request.Query["studentid"];
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ApplicationUserID,Name,Complete,FunFactor,Difficulty,Notes")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                string studentId;

                if (currentUser.UserType == "S")
                {
                    studentId = currentUser.Id;
                    ViewBag.CurrentUserType = "S";
                }
                else if (currentUser.UserType == "T")
                {
                    studentId = HttpContext.Request.Query["id"];
                    ViewBag.CurrentUserType = "T";
                }
                else
                {
                    throw new ApplicationException("Not a valid UserType");
                }

                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id=activity.ApplicationUserID });
            }
            return View(activity);
        }

        public IQueryable<LogEntry> GetAllLogEntries()
        {
            return _context.LogEntries.AsQueryable();
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currentUser = await GetCurrentUserAsync();
            ViewBag.CurrentUser = currentUser;

            ViewBag.LogEntries = GetAllLogEntries().Where(le => le.ActivityID == id).
                OrderByDescending(le => le.Date).ToList();
            var logEntries = (IEnumerable<dynamic>)ViewBag.LogEntries;
            ViewBag.StartDate = logEntries.Min(x => x.Date);
            ViewBag.TimeSpent = logEntries.Sum(x => (float)x.TimeSpent);
            ViewBag.LastActivity = logEntries.Max(x => x.Date);

            var activity = await _context.Activities
                .Include(a => a.Student)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (currentUser.UserType == "S")
            {
                if (activity.ApplicationUserID != currentUser.Id)
                {
                    return StatusCode(401);
                }
            }
            else
            {
                if (currentUser.Id != activity.Student.TeacherID)
                {
                    return StatusCode(401);
                }
            }

            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserID,Name,Complete,FunFactor,Difficulty,Notes")] Activity activity)
        {
            if (id != activity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .SingleOrDefaultAsync(m => m.ID == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.SingleOrDefaultAsync(m => m.ID == id);
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ID == id);
        }
    }
}
