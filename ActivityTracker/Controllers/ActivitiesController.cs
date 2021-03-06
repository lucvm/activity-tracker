﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityTracker.Data;
using ActivityTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            ViewBag.CurrentUserType = currentUser.UserType;

            string studentId;
            if (currentUser.UserType == "S")
            {
                studentId = currentUser.Id;
                ViewBag.studentId = studentId;
            }
            else
            {
                try
                {
                    var student = GetAllStudents().Where(u => u.Id == id).ToList()[0];
                    studentId = id;
                    ViewBag.studentId = id;
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
                    activity.Notes = activity.Notes.Split(new[] { '.', '\r', '\n' }).FirstOrDefault();
                }
            }

            ViewBag.Activities = activities;

            return View();
        }

        // GET: Activities/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser.UserType == "S")
            {
                ViewBag.CurrentUserType = "S";
            }
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
                return RedirectToAction("Index", new { id = activity.ApplicationUserID });
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

            var logEntries = GetAllLogEntries().Where(le => le.ActivityID == id).
                OrderByDescending(le => le.Date).ToList();

            ViewBag.LogEntries = logEntries;

            try
            {
                ViewBag.StartDate = logEntries.Min(x => x.Date).ToString("dd-MM-yyyy");
                ViewBag.TimeSpent = logEntries.Sum(x => x.TimeSpent);
                ViewBag.LastActivity = logEntries.Max(x => x.Date).ToString("dd-MM-yyyy");
            }
            catch (InvalidOperationException)
            {
                // do nothing
            }

            var activity = await _context.Activities
                .Include(a => a.Student)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (activity == null)
            {
                return NotFound();
            }

            if (Activity.AuthorizeActivityUser(activity, currentUser))
                return View(activity);
            else
                return RedirectToAction("AccessDenied", "Account");
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
            var currentUser = await GetCurrentUserAsync();
            string teacherId = "";

            if (currentUser.UserType == "T")
            {
                teacherId = _context.ApplicationUsers
                    .Where(au => au.Id == activity.ApplicationUserID)
                    .FirstOrDefault()
                    .TeacherID;
            }

            if (activity.ApplicationUserID == currentUser.Id ||
                teacherId == currentUser.Id)
            {
                if (ModelState.IsValid)
                {
                    return await NewMethod(activity, currentUser);
                }
                return View(activity);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        private async Task<IActionResult> NewMethod(Activity activity, ApplicationUser currentUser)
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
            if (currentUser.UserType == "S")
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction("Index", "Activities", new { id = activity.ApplicationUserID });
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

            var currentUser = await GetCurrentUserAsync();

            if (activity.ApplicationUserID == currentUser.Id)
            {
                return View(activity);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.SingleOrDefaultAsync(m => m.ID == id);
            var currentUser = await GetCurrentUserAsync();

            if (activity.ApplicationUserID == currentUser.Id)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ID == id);
        }
    }
}
