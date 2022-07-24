
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace testidentity2.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [MaxLength(50)]
        public string RefreshToken { get; set; }
    }
}
