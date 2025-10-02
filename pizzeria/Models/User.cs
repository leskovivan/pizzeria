using Microsoft.AspNetCore.Identity;

namespace pizzeria.Models
{
    public class User : IdentityUser
    {
        public string? City { get; set; }
        public string? Address { get; set; }
    }
}
