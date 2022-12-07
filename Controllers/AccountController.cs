using FlowerSite.Data;
using FlowerSite.Models;
using FlowerSite.Models.IdentityModels;
using FlowerSite.Services;
using FlowerSite.ViewModels;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowerSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailManager;



        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailManager = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            var user = HttpContext.User;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByNameAsync(model.Username);
            if(existUser != null)
            {
                ModelState.AddModelError("", "Username təkrarlana bilməz");
                return View();
            }

            var user = new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = model.Username,
            };

            var result = await _userManager.CreateAsync(user, model.ConfirmPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(); 
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existUser = await _userManager.FindByNameAsync(model.Username);

                if (existUser == null)
                {
                    ModelState.AddModelError("", "Username is not correct");
                    return View();
                }

                //var user = new User
                //{
                //    UserName = model.Username
                //};
                var result = await _signInManager.PasswordSignInAsync(existUser, model.Password, false, false);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Bloklandınız");
                    return View();
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Username or password invalid");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

      
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Email Yazılmalıdır");
                return View();
            }

            var existUser = await _userManager.FindByEmailAsync(model.Email);

            if(existUser == null)
            {
                ModelState.AddModelError("", "Bele email mövcud deyil");
                return View(); 
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(existUser);

            var restLink = Url.Action(nameof(ResetPassword), "Account", new {email=model.Email, token}, Request.Scheme,Request.Host.ToString());

            var emailRequest = new RequestEmail
            {
                ToEmail = model.Email,
                Body = restLink,
                Subject = "Reset link"
            };

            await _emailManager.SendEmailAsync(emailRequest);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ResetPassword(string email, string token)
        {
            return View( new ResetPasswordViewModel
            {
                Email = email,
                Token = token,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Düzgün doldurun!");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return BadRequest();

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(login));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
                return View();
        }

    }
}
