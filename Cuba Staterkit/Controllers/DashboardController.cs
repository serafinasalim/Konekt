using Cuba_Staterkit.Data;
using Cuba_Staterkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.Entity.Validation;

namespace Cuba_Staterkit.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext db;

        public DashboardController(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Index()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult ChangePassword()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var uname = HttpContext.Session.GetString("Username");
            var id_user = (from a in db.Account
                            where a.username == uname
                            select a.user_id).FirstOrDefault();
            
            ViewBag.UserId = id_user;

            return View();
        }

        public JsonResult SaveChangedPass(int id, string oldpass, string newpass)
        {
            int st = 0;

            var oldDB = (from a in db.Account select a.password).FirstOrDefault();
            try
            {
                if (oldpass == oldDB.ToString())
                {
                    t_account tabel = db.Account.Find(id);
                    tabel.password = newpass;
                    db.SaveChanges();
                    st = 1;
                }
                else
                {
                    st = 2;
                }
            }
            catch (DbEntityValidationException ex)
            {

                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);
                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);
                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                st = 0;
            }
            return Json(st);
        }
    }
}
