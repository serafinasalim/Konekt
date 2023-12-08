using Cuba_Staterkit.Data;
using Cuba_Staterkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.Entity.Validation;

namespace Cuba_Staterkit.Controllers
{
    public class SettingsController : Controller
    {
        private readonly AppDbContext db;

        public SettingsController(AppDbContext dbContext)
        {
            db = dbContext;
        }
        public IActionResult Users()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public JsonResult GetUser(int? page, int? page_size, string sortColumn, string sortOrder, string search)
        {
           var count_data_user = (from a in db.Account
                                    join b in db.Employee on a.username equals b.username
                                    where a.inactive == false
                                    select a).Count();

           var data_user = (from a in db.Account
                            join b in db.Employee on a.username equals b.username
                            where a.inactive == false && 
                            (string.IsNullOrEmpty(search) || (
                                (b.employee_name ?? "").ToLower().Contains(search.ToLower()) ||
                                (a.username ?? "").ToLower().Contains(search.ToLower()) 
                                // (b.position_name ?? "").ToLower().Contains(search.ToLower())
                            ))
                            select new
                            {
                                a.user_id,
                                a.username,
                                b.employee_id,
                                b.employee_name,
                                a.role,
                                a.password
                            }).ToList();

           if (!String.IsNullOrEmpty(sortColumn))
           {
               if (sortOrder.ToUpper() == "DESC")
               {
                   data_user = sortColumn == "employee_name" ? data_user.OrderByDescending(x => x.employee_name).ToList()
                                       // : sortColumn == "salary_min" ? data_posisi.OrderByDescending(x => x.salary_min).ToList()
                                       // : sortColumn == "salary_max" ? data_posisi.OrderByDescending(x => x.salary_max).ToList()
                                       : data_user.ToList();
               }
               else
               {
                   data_user = sortColumn == "employee_name" ? data_user.OrderBy(x => x.employee_name).ToList()
                                       // : sortColumn == "salary_min" ? data_posisi.OrderBy(x => x.salary_min).ToList()
                                       // : sortColumn == "salary_max" ? data_posisi.OrderBy(x => x.salary_max).ToList()
                                       : data_user.ToList();
               }
           }
           else
           {
               data_user = data_user.OrderBy(x => x.employee_name).ToList();
           }


           int pageSize = (page_size ?? 10);
           int pageNumber = (page ?? 1);
           var pl = data_user.ToPagedList(pageNumber, pageSize);

           return Json(new
           {
               recordsTotal = count_data_user,
               recordsFiltered = pl.TotalItemCount,
               pageCount = pl.PageCount,
               currentPage = pageNumber,
               data = pl,
           });
        }

        public class Select2DTOString // as select2 is formed like id and text so we used DTO   
        {
            public int id { get; set; }
            public string text { get; set; }
        }

        public JsonResult GetDropdownNim(string term)
        {
            List<Select2DTOString> resultList = new List<Select2DTOString>();
            Select2DTOString resultData;

            // Cek jika term kosong, maka tidak perlu melakukan filter tambahan
            if (string.IsNullOrEmpty(term))
            {
                var allData = (from a in db.Employee
                                join b in db.Account on a.username equals b.username into cekAcc
                                from b in cekAcc.DefaultIfEmpty()
                                where b == null && a.deleted == false
                                orderby a.employee_name ascending
                                select new
                                {
                                    a.username,
                                    a.employee_id,
                                    a.employee_name
                                }).ToList();

                foreach (var item in allData)
                {
                    resultData = new Select2DTOString();
                    resultData.id = item.employee_id;
                    resultData.text = item.employee_name + " " + "( " + item.username + " )";
                    resultList.Add(resultData);
                }
            }
            else
            {
                var filteredData = (from a in db.Employee
                                    join b in db.Account on a.username equals b.username into cekAcc
                                    from b in cekAcc.DefaultIfEmpty()
                                    where b == null
                                    && (a.employee_name.ToLower().Contains(term.ToLower()) || a.username.ToLower().Contains(term.ToLower()))
                                    orderby a.employee_name ascending
                                    select new
                                    {
                                        a.username,
                                        a.employee_id,
                                        a.employee_name
                                    }).ToList();

                foreach (var item in filteredData)
                {
                    resultData = new Select2DTOString();
                    resultData.id = item.employee_id;
                    resultData.text = item.employee_name + " " + "( " + item.username + " )";
                    resultList.Add(resultData);
                }
            }

            return Json(new { resultList });
        }

        public JsonResult SimpanUser()
        {
            int st = 0;
            string tombol = HttpContext.Request.Form["tombol"];
            

            if(tombol == "1")
            {
                int id_user = Convert.ToInt16(HttpContext.Request.Form["nim_user"]);
                var emp = (from a in db.Employee where a.employee_id == id_user select a).FirstOrDefault();
                string uname = emp.username;
                string akses = HttpContext.Request.Form["role_user"];
                string pass = HttpContext.Request.Form["pass_user"];

                try
                {
                    t_account tabel = new t_account();
                    tabel.username = uname;
                    tabel.password = pass;
                    tabel.role = akses;
                    tabel.inactive = false;

                    db.Account.Add(tabel);

                    db.SaveChanges();
                    st = 1;
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
            }
            else if(tombol == "2")
            {
                //edit
                try
                {
                    int id = Convert.ToInt16(HttpContext.Request.Form["id_user"]);
                    string akses2 = HttpContext.Request.Form["role_user2"];
                    string pass2 = HttpContext.Request.Form["pass_user2"];
                    t_account tabel = db.Account.Find(id);
                    tabel.password = pass2;
                    tabel.role = akses2;
                    tabel.inactive = false;

                    db.SaveChanges();
                    st = 2;
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
            }
            return Json(st);
        }

        public JsonResult HapusUser()
        {
            int st = 0;
            try
            {
                int id = Convert.ToInt16(HttpContext.Request.Form["id"]);
                t_account tabel = db.Account.Find(id);
                tabel.inactive = true;

                db.SaveChanges();
                st = 1;
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
