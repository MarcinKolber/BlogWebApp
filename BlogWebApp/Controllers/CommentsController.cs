using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogWebApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject database context and user manager
        public CommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        // Return a list of comments 
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);

            // Allow admin to see all comments, including other users
            if (await _userManager.IsInRoleAsync(applicationUser, "Admin"))
            {
                var applicationDbContext = _context.Comment.Include(c => c.Author)
                      .Include(c => c.Post);
                return View(await applicationDbContext.ToListAsync());

            }

            // If customer is logged in, then allow them to see only their posts
            else if (await _userManager.IsInRoleAsync(applicationUser, "Customer"))
            {
                var applicationDbContext = _context.Comment.Include(c => c.Author)
                      .Include(c => c.Post).Where(x => x.Author == applicationUser);
                return View(await applicationDbContext.ToListAsync());

            }
            else
            {
                return NotFound();
            }
        }

        // Returns a detailed view of a comment
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var comment = await _context.Comment
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        // Allows to comment on a particular post
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult Create(int? id)
        {
            Comment comment = new Comment();
            if (id == null)
            {
                return NotFound();
            }

            // Get a post to be commented on
            Post post = _context.Post.Include(p => p.Author)
                                     .FirstOrDefault(m => m.Id == id);

            comment.Post = post;

            return View(comment);
        }



        // POST: Comments/Create
        // Creates comment 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Create([Bind("PostId, CommentContent")] PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                Comment comment = new Comment();
                comment.Content = postViewModel.CommentContent;

                // Currently logged user is an author of a new comment
                var userId = _userManager.GetUserId(User);
                Post post = await _context.Post.SingleAsync(x => x.Id == postViewModel.PostId);

                /// Set values of the post
                comment.Post = post;
                comment.Author = await _userManager.FindByIdAsync(userId);

                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(postViewModel);
        }

        // GET: Comments/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            Comment comment = await _context.Comment.Where(x => x.AuthorId == userId)
                    .Include(i => i.Post).Include(a => a.Author)
                    .FirstOrDefaultAsync(i => i.Id == id.Value);

            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // Edits a 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AuthorId,Content,DateTime")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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

            return View(comment);
        }

        // GET: Comments/Delete/5
        // Selects a comment to be deleted
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);

            Comment comment = null;

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);

            if (await _userManager.IsInRoleAsync(applicationUser, "Admin"))
            {
                comment = await _context.Comment
                    .Include(c => c.Author)
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            // If customer is logged in, then allow them to delete only their posts
            else if (await _userManager.IsInRoleAsync(applicationUser, "Customer"))
            {
                comment = await _context.Comment.Where(x => x.AuthorId == userId)
                    .Include(c => c.Author)
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(m => m.Id == id); ;
            }

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        // Deletes a selected post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if a comment exists
        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
