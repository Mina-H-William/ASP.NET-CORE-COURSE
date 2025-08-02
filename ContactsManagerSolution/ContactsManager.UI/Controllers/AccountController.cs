using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.Enums;
using ContactsManager.UI.ModelsDTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    //[AllowAnonymous] // ********Allow anonymous skip all authorization checks********
    [Route("/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signInManager,
                                RoleManager<ApplicationRole> roleManager)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(policy: "NotAuthorized")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(policy: "NotAuthorized")]
        //[ValidateAntiForgeryToken] // to prevent CSRF attacks (work only with Post Request)
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                PersonName = registerDTO.PersonName
            };

            IdentityResult result = await _usermanager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDTO);
            }

            if (registerDTO.UserType == UserTypeOptions.Admin)
            {
                if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypeOptions.Admin.ToString() });

                await _usermanager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
            }
            else
            {
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypeOptions.User.ToString() });

                await _usermanager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            // isPersistent (remember me)
            // if true -> means cookie will persist even after browser is closed so user will stay logged in
            // if false -> means cookie will be deleted when browser is closed so user will be logged out

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        [Authorize(policy: "NotAuthorized")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Authorize(policy: "NotAuthorized")]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password,
                                                                            isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Login", "Invalid email or Password");

                return View(loginDTO);
            }

            // we need to check if the ReturnUrl is local to prevent open redirect attacks
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            ApplicationUser? user = await _usermanager.FindByEmailAsync(loginDTO.Email);

            if (user != null && await _usermanager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
            {
                return RedirectToAction("Index", "Home", new
                {
                    area = "Admin",
                });
            }

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login), "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            ApplicationUser? user = await _usermanager.FindByEmailAsync(email);

            if (user == null)
            {
                // remote validation only accept Json result
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}
