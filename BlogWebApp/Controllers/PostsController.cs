using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Controllers
{
    public class PostsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Injecting 
        public PostsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> AddComment([Bind("PostId, CommentContent")] PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                Comment comment = new Comment();
                comment.Content = postViewModel.CommentContent;
                Post post = await _context.Post.SingleAsync(x => x.Id == postViewModel.PostId);
                comment.Post = post;
                DateTime dt = DateTime.Now;
                comment.DateTime = dt;

                var userId = _userManager.GetUserId(User);
                comment.Author = await _userManager.FindByIdAsync(userId);
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details/" + postViewModel.PostId);
            }
            return View(postViewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<PostViewModel> postViewModels = new List<PostViewModel>();
            var applicationDbContext = _context.Post.Include(p => p.Author);
            List<Post> posts = await applicationDbContext.ToListAsync();

            foreach (Post post in posts)
            {
                PostViewModel postViewModel = GetViewFromPost(post);
                postViewModels.Add(postViewModel);
            }

            return View(postViewModels);
        }

        private PostViewModel GetViewFromPost(Post post)
        {

            PostViewModel postViewModel = new PostViewModel();
            postViewModel.Post = post;
            IEnumerable<Comment> PostComments = _context.Comment.Include(m => m.Author)
                .Where(x => x.Post.Id == post.Id);
            postViewModel.Comments = PostComments;

            return postViewModel;
        }




        // GET: Posts/Details/5
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            var userId = _userManager.GetUserId(User);

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author).Include(c => c.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (post == null)
            {
                return NotFound();
            }
            else
            {
                PostViewModel postViewModel = GetViewFromPost(post);
                return View(postViewModel);
            }

        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.DateTime = DateTime.Now;
                var userId = _userManager.GetUserId(User);
                post.Author = await _userManager.FindByIdAsync(userId);
                _context.Post.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,AuthorId,DateTime")] Post post)
        {
            var userId = _userManager.GetUserId(User);

            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Post.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
