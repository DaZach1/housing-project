using Microsoft.AspNetCore.Identity;

namespace HousingAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsManager { get; set; }
    }
}