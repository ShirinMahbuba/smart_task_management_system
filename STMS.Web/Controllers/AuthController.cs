using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using STMS.Models;
using STMS.Repos;
using System.Security.Claims;

namespace STMS.Web.Controllers
{
    public class AuthController(UserRepo userRepo) : Controller
    {
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var result = userRepo.Login(model.Email, model.Password);

            if (result.HasError || result.Data == null)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  result.Data.Name),
                new Claim(ClaimTypes.Role,  result.Data.Role),
                new Claim("Email",          result.Data.Email),
                new Claim("UserId",         result.Data.ID.ToString())
            };

            await HttpContext.SignInAsync("StmsAuth",
                new ClaimsPrincipal(new ClaimsIdentity(claims, "StmsAuth")));

            if (result.Data.Role.ToLower() == "admin")
                return RedirectToAction("Index", "Admin");
            if (result.Data.Role.ToLower() == "employee")
                return RedirectToAction("Index", "Dashboard");
                if (result.Data.Role.ToLower() == "manager")
                return RedirectToAction("Index", "Manager");
            else
                return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var result = userRepo.Register(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        public IActionResult ResetPassword()
        {
            return View(new PasswordResetModel());
        }

        [HttpPost]
        public IActionResult ResetPassword(PasswordResetModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var result = userRepo.ResetPassword(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Password reset successful! Please login.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("StmsAuth");
            return RedirectToAction("Login");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}
