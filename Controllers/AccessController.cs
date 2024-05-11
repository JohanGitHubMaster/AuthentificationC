using Authorisation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authorisation.Controllers
{
   public class AccessController : Controller
   {
      public IActionResult Login()
      {
         ClaimsPrincipal claimUser = HttpContext.User;
         if (claimUser.Identity.IsAuthenticated)
         {
            return RedirectToAction("Index", "Home");
         }
         return View();
      }

      public IActionResult ErrorAccess()
      {
         HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         return View("Login");
      }

      [HttpPost]
      public async Task<IActionResult> Login(VMLogin modelLogin)
      {
         if (modelLogin.Email == "user@example.com" && modelLogin.Password == "123")
         {
            List<Claim> claims = new List<Claim>()
            {
               //new Claim(ClaimTypes.NameIdentifier,modelLogin.Email),
               new Claim(ClaimTypes.Role, "User"),
            };
            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new()
            {
               AllowRefresh = true,
               IsPersistent = modelLogin.KeepLoggedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            return RedirectToAction("Index", "Home");
         }
         ViewData["ValidateMessage"] = "User not found";
         return View();
      }
   }
}
