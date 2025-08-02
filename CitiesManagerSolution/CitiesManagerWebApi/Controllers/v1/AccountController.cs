using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManagerWebApi.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1.0")] // specify the version of this controller
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IJwtService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            _jwtService = jwtService;
        }

        [HttpGet("IsEmailAlreadyRegistered")]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user == null ? Ok(true) : Ok(false);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid) // can be removed as [ApiController] validates model automatically
            {
                string errors = string.Join(" | ", ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));

                return Problem(errors);
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                PersonName = registerDTO.PersonName
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            AuthenticationResponse response = _jwtService.CreateJwtToken(user);

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiration = response.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) // can be removed as [ApiController] validates model automatically
            {
                string errors = string.Join(" | ", ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));

                return Problem(errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.email);

            if (user == null)
            {
                return Problem("User not found with this email");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.password,
                                                                    isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Problem("Invalid login attempt. Please check your email and password.");
            }

            AuthenticationResponse response = _jwtService.CreateJwtToken(user);

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiration = response.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return Ok(response);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> PostLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
                return BadRequest("Token model cannot be null.");

            string? jwtToken = tokenModel.Token;
            string? refreshToken = tokenModel.RefreshToken;

            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(jwtToken);

            if (principal == null)
            {
                return BadRequest("Invalid JWT token.");
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return BadRequest("invalid refresh token or user data");
            }

            AuthenticationResponse response = _jwtService.CreateJwtToken(user);

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiration = response.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return Ok(response);
        }
    }
}
