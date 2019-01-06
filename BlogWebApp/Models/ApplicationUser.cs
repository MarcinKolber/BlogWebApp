using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {

        [MinLength(1), MaxLength(1024)]
        public string Notes { get; set; }
    }
}
