using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            DateTime RefreshTokenExpiration = DateTime.UtcNow
                                                      .AddMinutes(Convert.ToDouble(_configuration["RefreshToken:EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//  JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued at time (Created Time)

                // NameIdentifier is equivalent to Sub
                // so when Sub in JwtRegisteredClaimNames is used, NameIdentifier is not needed as sub will map automatically 
                // to the name identifier 
                //new Claim(ClaimTypes.NameIdentifier, user.Email!),
                new Claim(ClaimTypes.Email, user.Email!), // unique identifier for the user
                new Claim(ClaimTypes.Name, user.PersonName!), // unique identifier for the user
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: expiration,
                    signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthenticationResponse
            {
                PersonName = user.PersonName,
                Email = user.Email,
                Token = token,
                RefreshToken = GenerateRefreshToken(),
                Expiration = expiration,
                RefreshTokenExpiration = RefreshTokenExpiration
            };

        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),

                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],

                ValidateLifetime = false // as we called this when token is expired
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                                                    out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string GenerateRefreshToken()
        {
            Byte[] bytes = new Byte[64];

            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
