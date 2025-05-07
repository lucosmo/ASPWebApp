using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPWebApp.Data;
using ASPWebApp.Models;


namespace ASPWebApp.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(AppDbContext context, ILogger<ArticlesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all articles.");
            var appDbContext = _context.Articles.Include(a => a.Author);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details called with null id.");
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Showing details for article ID {Id}.", id);
            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            _logger.LogDebug("Opening Create article form.");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,UserId")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Article '{Title}' created by user {UserId}.", article.Title, article.UserId);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid model state while creating article.");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", article.UserId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit called with null id.");
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found for editing.", id);
                return NotFound();
            }

            _logger.LogDebug("Editing article ID {Id}.", id);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", article.UserId);
            return View(article);
        }

        // POST: Articles/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,UserId")] Article article)
        {
            if (id != article.Id)
            {
                _logger.LogError("Route ID {Id} does not match article ID {ArticleId}.", id, article.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Article ID {Id} updated successfully.", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        _logger.LogWarning("Concurrency error: article ID {Id} does not exist.", article.Id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Concurrency error while editing article ID {Id}.", article.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid model state during article edit ID {Id}.", id);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", article.UserId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete called with null id.");
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _logger.LogDebug("Opening delete confirmation for article ID {Id}.", id);
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                _logger.LogInformation("Article ID {Id} deleted.", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent article ID {Id}.", id);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

        // GET: Articles/Search
        public async Task<IActionResult> Search(string query)
        {
            _logger.LogInformation("Search requested with query: '{Query}'", query);

            var results = string.IsNullOrWhiteSpace(query)
                ? new List<Article>()
                : await _context.Articles
                    .Include(a => a.Author)
                    .Where(a => a.Body.Contains(query))
                    .ToListAsync();

            _logger.LogInformation("Search found {Count} results for query: '{Query}'", results.Count, query);
            ViewData["Query"] = query;
            return View(results);
        }
    }

}
