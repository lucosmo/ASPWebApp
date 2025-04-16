using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPWebApp.Data;
using ASPWebApp.Models;
using Microsoft.Extensions.Logging;

namespace ASPWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching list of users.");
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details requested with null id.");
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Showing details for user ID {Id}.", id);
            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            _logger.LogDebug("Opening user creation form.");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User '{Name}' created with ID {Id}.", user.Name, user.Id);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid model state while creating user.");
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit requested with null id.");
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for editing.", id);
                return NotFound();
            }

            _logger.LogDebug("Editing user ID {Id}.", id);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] User user)
        {
            if (id != user.Id)
            {
                _logger.LogError("Mismatch between route ID {Id} and model ID {UserId}.", id, user.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User ID {Id} updated.", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        _logger.LogWarning("Concurrency issue: user ID {Id} not found.", user.Id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Concurrency exception occurred while editing user ID {Id}.", user.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid model state while editing user ID {Id}.", id);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete requested with null id.");
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _logger.LogDebug("Opening delete confirmation for user ID {Id}.", id);
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _logger.LogInformation("User ID {Id} deleted.", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent user ID {Id}.", id);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
