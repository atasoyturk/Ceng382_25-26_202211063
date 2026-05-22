using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tastemam.Models;
using tastemam.Services;

namespace tastemam.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LogService _logService;
        private readonly EmailService _emailService;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            LogService logService,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logService = logService;
            _emailService = emailService;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _logService.LogAsync("Auth", $"Yeni kullanıcı kaydoldu.", model.Email);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _logService.LogAsync("Auth", $"Başarısız giriş denemesi.", model.Email, "Warning");
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);
            bool requiresTwoFactor = false;    // roles.Contains("Admin") || roles.Contains("Caretaker");

            if (requiresTwoFactor)
            {
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    await _logService.LogAsync("Auth", $"Başarısız giriş denemesi.", model.Email, "Warning");
                    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                    return View(model);
                }

                // 2FA kodu oluştur ve gönder
                var code = new Random().Next(100000, 999999).ToString();
                await _userManager.SetAuthenticationTokenAsync(user, "TwoFactor", "Code", code);
                await _userManager.SetAuthenticationTokenAsync(user, "TwoFactor", "Expiry",
                    DateTime.Now.AddMinutes(10).ToString());

                await _emailService.SendEmailAsync(user.Email, "TasteMam - Doğrulama Kodu",
                    $@"<div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                        <h2 style='color:#C0392B;'>Giriş Doğrulama</h2>
                        <p>Doğrulama kodunuz:</p>
                        <h1 style='letter-spacing:8px; color:#1a1a1a;'>{code}</h1>
                        <p style='color:#888;font-size:0.85rem;'>Bu kod 10 dakika geçerlidir.</p>
                    </div>");

                await _logService.LogAsync("Auth", $"2FA kodu gönderildi.", model.Email);

                TempData["TwoFactorEmail"] = model.Email;
                TempData["TwoFactorRememberMe"] = model.RememberMe;
                return RedirectToAction("TwoFactor");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                await _logService.LogAsync("Auth", $"Kullanıcı giriş yaptı.", model.Email);
                return RedirectToAction("Index", "Home");
            }

            await _logService.LogAsync("Auth", $"Başarısız giriş denemesi.", model.Email, "Warning");
            ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
            return View(model);
        }

        public IActionResult TwoFactor()
        {
            var email = TempData["TwoFactorEmail"] as string;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            TempData.Keep("TwoFactorEmail");
            TempData.Keep("TwoFactorRememberMe");

            var model = new TwoFactorViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model)
        {
            var email = TempData["TwoFactorEmail"] as string;
            var rememberMe = TempData["TwoFactorRememberMe"] as bool? ?? false;

            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return RedirectToAction("Login");

            var storedCode = await _userManager.GetAuthenticationTokenAsync(user, "TwoFactor", "Code");
            var expiryStr = await _userManager.GetAuthenticationTokenAsync(user, "TwoFactor", "Expiry");

            if (storedCode != model.Code)
            {
                await _logService.LogAsync("Auth", $"Hatalı 2FA kodu girildi.", email, "Warning");
                ModelState.AddModelError("", "Geçersiz doğrulama kodu.");
                TempData["TwoFactorEmail"] = email;
                TempData["TwoFactorRememberMe"] = rememberMe;
                return View(model);
            }

            if (DateTime.TryParse(expiryStr, out var expiry) && DateTime.Now > expiry)
            {
                ModelState.AddModelError("", "Doğrulama kodunun süresi dolmuş. Lütfen tekrar giriş yapın.");
                return View(model);
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "TwoFactor", "Code");
            await _userManager.RemoveAuthenticationTokenAsync(user, "TwoFactor", "Expiry");

            await _signInManager.SignInAsync(user, rememberMe);
            await _logService.LogAsync("Auth", $"2FA ile giriş başarılı.", email);

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin") || roles.Contains("Caretaker"))
                return RedirectToAction("Index", "Caretaker");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var email = User.Identity.Name;
            await _signInManager.SignOutAsync();
            await _logService.LogAsync("Auth", "Kullanıcı çıkış yaptı.", email);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}