using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.UI.ModelsDTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signInManager)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
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

            await _signInManager.SignInAsync(user, isPersistent: false);
            // isPersistent (remember me)
            // if true -> means cookie will persist even after browser is closed so user will stay logged in
            // if false -> means cookie will be deleted when browser is closed so user will be logged out

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
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

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login), "Account");
        }
    }
}
