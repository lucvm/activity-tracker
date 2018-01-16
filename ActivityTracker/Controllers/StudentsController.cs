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
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public StudentsController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IQueryable<ApplicationUser> GetAllStudents()
        {
            return _context.ApplicationUsers.AsQueryable();
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;

            var students = GetAllStudents().Where(s => s.TeacherID == Convert.ToString(user.Id)).ToList();
            var groups = await _context.Groups.ToListAsync();
            var userGroups = await _context.UserGroups.ToListAsync();
            var logEntries = await _context.LogEntries.ToListAsync();
            foreach (var student in students)
            {
                foreach (var userGroup in userGroups)
                {
                    if (student.Id == userGroup.ApplicationUserID)
                    {
                        student.UserGroups.Add(userGroup);
                    }
                }
            }
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();

            var applicationUser = await _context.ApplicationUsers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            if (applicationUser.TeacherID == currentUser.Id)
                return View(applicationUser);
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await GetCurrentUserAsync();
            ViewBag.userId = currentUser?.Id;

            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherID,UserType,FirstName,Prefix,LastName,Notes,LastActive,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();

                if (applicationUser.TeacherID == currentUser.Id)
                {
                    _context.Add(applicationUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Students");
                }
                else
                    return RedirectToAction("AccessDenied", "Account");
            }
            return View(applicationUser);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var currentUserId = currentUser?.Id;

            ViewBag.CurrentGroups = new List<Group>();

            ViewBag.Groups = _context.Groups.AsQueryable().Where(g => g.OwnerID == currentUserId)
                .Include(g => g.UserGroups)
                .ToList();

            foreach (var group in ViewBag.Groups)
            {
                foreach (var ug in group.UserGroups)
                {
                    if (ug.ApplicationUserID == id)
                    {
                        ViewBag.CurrentGroups.Add(group);
                    }
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            if (currentUserId == applicationUser.TeacherID)
                return View(applicationUser);
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, List<string> groups, [Bind("TeacherID,UserType,FirstName,Prefix,LastName,Notes,LastActive,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await GetCurrentUserAsync();

                    if (currentUser.Id == applicationUser.TeacherID)
                    {
                        _context.Update(applicationUser);

                        _context.Database.ExecuteSqlCommand($"DELETE FROM UserGroup WHERE ApplicationUserID = {id}");
                        await _context.SaveChangesAsync();

                        foreach (var group in groups)
                        {
                            _context.Add(new UserGroup
                            {
                                ApplicationUserID = id,
                                GroupID = group
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                    else
                        return RedirectToAction("AccessDenied", "Account");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
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
            return View(applicationUser);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var applicationUser = await _context.ApplicationUsers
                .SingleOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            if (currentUser.Id == applicationUser.TeacherID)
                return View(applicationUser);
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);

            if (currentUser.Id == applicationUser.TeacherID)
            {
                _context.ApplicationUsers.Remove(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
