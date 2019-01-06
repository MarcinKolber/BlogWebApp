using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApp.Models
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public string PostAuthorName {get; set;}

        [Display(Name = "Comment content")]
        [MinLength(2), MaxLength(100)]
        public string CommentContent { get; set; }
        public int PostId { get; set; }
    }
}
