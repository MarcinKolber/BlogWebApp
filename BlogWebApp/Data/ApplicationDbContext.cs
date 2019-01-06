using System;
using System.Collections.Generic;
using System.Text;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {



        public DbSet<BlogWebApp.Models.Post> Post { get; set; }
        public DbSet<BlogWebApp.Models.Comment> Comment { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)

        {
        }


    }
}
