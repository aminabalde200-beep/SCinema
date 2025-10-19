using Microsoft.AspNetCore.Identity;
using System;

namespace SCinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

     
       

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
