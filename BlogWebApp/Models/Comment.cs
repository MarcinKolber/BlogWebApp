using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public virtual Post Post { get; set; }

        public virtual ApplicationUser Author { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        [Display(Name = "Comment content")]
        [MinLength(2), MaxLength(100)]
        public string Content { get; set; }

        [Display(Name = "Added")]
        public DateTime DateTime { get; set; }
    }
}
