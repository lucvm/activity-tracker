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

namespace ActivityTracker.Controllers
{
    public class LogEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public LogEntriesController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: LogEntries
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.LogEntries.ToListAsync());
        //}

        // GET: LogEntries/Create
        public IActionResult Create()
        {
            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.ActivityId = HttpContext.Request.Query["activityid"];

            return View();
        }

        // POST: LogEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ActivityID,Date,TimeSpent,Notes")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser student;
                var currentUser = GetCurrentUserAsync().Result;

                if (currentUser.UserType == "S")
                {
                    student = await _context.ApplicationUsers.SingleOrDefaultAsync(s => s.Id == currentUser.Id);
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Account");
                    //var activity = await _context.Activities.SingleOrDefaultAsync(
                    //    a => a.ID == logEntry.ActivityID);
                    //student = await _context.ApplicationUsers.SingleOrDefaultAsync(
                    //    s => s.Id == activity.ApplicationUserID);
                }

                var activity = await _context.Activities.SingleOrDefaultAsync(a => a.ID == logEntry.ActivityID);

                if (activity.ApplicationUserID == Convert.ToString(currentUser.Id))
                {
                    _context.Add(logEntry);
                    await _context.SaveChangesAsync();

                    student.LastActive = logEntry.Date;
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Activities", new { id = logEntry.ActivityID });
                }
                else
                    return RedirectToAction("AccessDenied", "Account");
            }
            ViewBag.ActivityId = logEntry.ActivityID;
            return View(logEntry);
        }

        // GET: LogEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUserAsync().Result;
            var logEntry = await _context.LogEntries.SingleOrDefaultAsync(m => m.ID == id);

            if (logEntry == null)
            {
                return NotFound();
            }
            return View(logEntry);
        }

        // POST: LogEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ActivityID,Date,TimeSpent,Notes")] LogEntry logEntry)
        {
            if (id != logEntry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = GetCurrentUserAsync().Result;
                    var activity = await _context.Activities.SingleOrDefaultAsync(a => a.ID == logEntry.ActivityID);

                    if (activity.ApplicationUserID == Convert.ToString(currentUser.Id))
                    {
                        _context.Update(logEntry);
                        await _context.SaveChangesAsync();
                    }
                    else
                        return RedirectToAction("AccessDenied", "Account");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogEntryExists(logEntry.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Activities", new { id = logEntry.ActivityID });
            }
            return View(logEntry);
        }

        // GET: LogEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUserAsync().Result;
            var logEntry = await _context.LogEntries
                .SingleOrDefaultAsync(m => m.ID == id);
            var activity = await _context.Activities.SingleOrDefaultAsync(a => a.ID == logEntry.ActivityID);

            if (logEntry == null)
            {
                return NotFound();
            }

            if (activity.ApplicationUserID == Convert.ToString(currentUser.Id))
            {
                return View(logEntry);
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // POST: LogEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = GetCurrentUserAsync().Result;
            var logEntry = await _context.LogEntries.SingleOrDefaultAsync(m => m.ID == id);
            var activity = await _context.Activities.SingleOrDefaultAsync(a => a.ID == logEntry.ActivityID);

            if (activity.ApplicationUserID == Convert.ToString(currentUser.Id))
            {
                _context.LogEntries.Remove(logEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Activities", new { id = logEntry.ActivityID });
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        private bool LogEntryExists(int id)
        {
            return _context.LogEntries.Any(e => e.ID == id);
        }
    }
}
