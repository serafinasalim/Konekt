using Cuba_Staterkit.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Cuba_Staterkit.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db;

        public AccountController(AppDbContext dbContext)
        {
            db = dbContext;
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                // Redirect to the dashboard if already logged in
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }
        public async Task<JsonResult> CheckAccount()
        {
            var st = 0;
            var nim = HttpContext.Request.Form["Username"].ToString();
            var pass = HttpContext.Request.Form["Password"].ToString();

            var user = (from a in db.Account
                        where a.username == nim
                        && a.password == pass
                        select a).FirstOrDefault();

            if (user != null)
            {
                // Buat klaim untuk identitas pengguna
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.Role, user.role), // Sesuaikan dengan struktur data Anda
                };

                // Buat identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Buat principal
                var principal = new ClaimsPrincipal(identity);

                // SignInAsync untuk menetapkan pengguna yang telah diautentikasi
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Username", user.username);

                st = 1;
            }
            else
            {
                st = 0;
            }
            return Json(st);
        }

        public IActionResult Logout()
        {
            // Clear authentication-related session values
            HttpContext.Session.SetString("IsAuthenticated", "false");
            HttpContext.Session.Remove("Username");

            // Redirect to the login page
            return RedirectToAction("Login");
        }

    }


}
