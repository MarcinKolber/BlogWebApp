using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        
        [Display(Name = "Content of a post")]
        public string Content { get; set; }

        [Display(Name = "Creation time")]
        public DateTime DateTime { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author {get; set;}
        public List<Comment> Comments { get; set; }
    }
}
