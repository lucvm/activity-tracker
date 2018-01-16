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
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();

            var groups = await _context.Groups.Where(g => g.OwnerID == currentUser.Id).ToListAsync();

            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var @group = await _context.Groups
                .SingleOrDefaultAsync(m => m.ID == id);

            if (@group == null)
            {
                return NotFound();
            }

            if (currentUser.Id == @group.OwnerID)
                return View(@group);
            else
                return Unauthorized();
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Group @group)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var currentUserId = currentUser?.Id;
                @group.OwnerID = currentUserId;

                if (currentUser.Id == @group.OwnerID)
                {
                    _context.Add(@group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Groups");
                }
                else
                    return Unauthorized();
            }
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var @group = await _context.Groups.SingleOrDefaultAsync(m => m.ID == id);

            if (@group == null)
            {
                return NotFound();
            }

            if (currentUser.Id == @group.OwnerID)
                return View(@group);
            else
                return Unauthorized();
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,OwnerID,Name")] Group @group)
        {
            if (id != @group.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await GetCurrentUserAsync();

                    if (currentUser.Id == @group.OwnerID)
                    {
                        _context.Update(@group);
                        await _context.SaveChangesAsync();
                    }
                    else
                        return Unauthorized();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.ID))
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
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var @group = await _context.Groups
                .SingleOrDefaultAsync(m => m.ID == id);
            if (@group == null)
            {
                return NotFound();
            }

            if (currentUser.Id == @group.OwnerID)
                return View(@group);
            else
                return Unauthorized();
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var @group = await _context.Groups.SingleOrDefaultAsync(m => m.ID == id);

            if (currentUser.Id == @group.OwnerID)
            {
                _context.Groups.Remove(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
                return Unauthorized();
        }

        private bool GroupExists(string id)
        {
            return _context.Groups.Any(e => e.ID == id);
        }
    }
}
