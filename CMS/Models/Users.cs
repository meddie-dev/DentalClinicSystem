using Microsoft.AspNetCore.Identity;

namespace CMS.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
