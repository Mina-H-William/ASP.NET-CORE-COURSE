using Microsoft.AspNetCore.Identity;

namespace CitiesManager.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; } // to track when the refresh token expires
    }
}
