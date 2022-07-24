using System.Security.Claims;

using testidentity2.Data.Models;
using Microsoft.AspNetCore.Identity;
namespace testidentity2.Extensions
{
    public static class GetUserClaimsAsync
    {
        public static async Task<IEnumerable<Claim>> GetClaimsAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            };

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
