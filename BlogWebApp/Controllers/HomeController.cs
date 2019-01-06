using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using BlogWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Returns a list of all posts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _context.Post.Include(m => m.Author).ToListAsync();
            List<PostViewModel> postViews = new List<PostViewModel>();

            foreach (Post post in posts)
            {
                PostViewModel postViewModel = GetViewFromPost(post);
                postViews.Add(postViewModel);
            }
            postViews.Reverse();

            return View(postViews);
        }

        // Returns an instance of a post view object
        private PostViewModel GetViewFromPost(Post post)
        {

            PostViewModel postViewModel = new PostViewModel();
            postViewModel.Post = post;
            IEnumerable<Comment> PostComments = _context.Comment.Include(m => m.Author)
                .Where(x => x.Post.Id == post.Id);
            postViewModel.Comments = PostComments;

            return postViewModel;
        }


        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
