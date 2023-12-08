using Cuba_Staterkit.Data;
using Cuba_Staterkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.Entity.Validation;

namespace Cuba_Staterkit.Controllers
{
    public class MasterController : Controller
    {
        private readonly AppDbContext db;

        public MasterController(AppDbContext dbContext)
        {
            db = dbContext;
        }
        public IActionResult Employee()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.posisi = (from a in db.Position where a.deleted == false orderby a.position_name descending select a).ToList();

            return View();
        }
        public IActionResult Permit()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        
        public IActionResult Position()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public JsonResult GetPosisi(int? page, int? page_size, string sortColumn, string sortOrder, string search)
        {
            var count_data_posisi = (from a in db.Position
                                            where a.deleted == false
                                            select a).Count();

            var data_posisi = (from a in db.Position
                                    where a.deleted == false && 
                                    (string.IsNullOrEmpty(search) || (
                                        (a.position_name ?? "").ToLower().Contains(search.ToLower()) 
                                    ))
                                    select a).ToList();

            if (!String.IsNullOrEmpty(sortColumn))
            {
                if (sortOrder.ToUpper() == "DESC")
                {
                    data_posisi = sortColumn == "position_name" ? data_posisi.OrderByDescending(x => x.position_name).ToList()
                                        : sortColumn == "salary_min" ? data_posisi.OrderByDescending(x => x.salary_min).ToList()
                                        : sortColumn == "salary_max" ? data_posisi.OrderByDescending(x => x.salary_max).ToList()
                                        : data_posisi.ToList();
                }
                else
                {
                    data_posisi = sortColumn == "position_name" ? data_posisi.OrderBy(x => x.position_name).ToList()
                                        : sortColumn == "salary_min" ? data_posisi.OrderBy(x => x.salary_min).ToList()
                                        : sortColumn == "salary_max" ? data_posisi.OrderBy(x => x.salary_max).ToList()
                                        : data_posisi.ToList();
                }
            }
            else
            {
                data_posisi = data_posisi.OrderBy(x => x.position_name).ToList();
            }


            int pageSize = (page_size ?? 10);
            int pageNumber = (page ?? 1);
            var pl = data_posisi.ToPagedList(pageNumber, pageSize);

            return Json(new
            {
                recordsTotal = count_data_posisi,
                recordsFiltered = pl.TotalItemCount,
                pageCount = pl.PageCount,
                currentPage = pageNumber,
                data = pl,
            });
        }

        public JsonResult SimpanPosisi()
        {
            int st = 0;
            string tombol = HttpContext.Request.Form["tombol"];
            string nama = HttpContext.Request.Form["nama_posisi"];
            string temp_max = HttpContext.Request.Form["max_salary"];
            decimal max;
            decimal.TryParse(temp_max, out max);

            if(tombol == "1")
            {
                try
                {
                    t_position tabel = new t_position();
                    tabel.position_name = nama;
                    tabel.salary_min = 0;
                    tabel.salary_max = max;
                    tabel.deleted = false;

                    db.Position.Add(tabel);

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
                    int id = Convert.ToInt16(HttpContext.Request.Form["id_posisi"]);
                    t_position tabel = db.Position.Find(id);
                    tabel.position_name = nama;
                    tabel.salary_min = 0;
                    tabel.salary_max = max;
                    tabel.deleted = false;

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

        public JsonResult HapusPosisi()
        {
            int st = 0;
            try
            {
                int id = Convert.ToInt16(HttpContext.Request.Form["id"]);
                t_position tabel = db.Position.Find(id);
                tabel.deleted = true;

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

        public JsonResult GetKaryawan(int? page, int? page_size, string sortColumn, string sortOrder, string search)
        {
           var count_data_karyawan = (from a in db.Employee
                                       join b in db.Position on a.position_id equals b.position_id
                                       where a.deleted == false
                                       select a).Count();

           var data_karyawan = (from a in db.Employee
                                   join b in db.Position on a.position_id equals b.position_id
                                   where a.deleted == false && 
                                   (string.IsNullOrEmpty(search) || (
                                       (a.employee_name ?? "").ToLower().Contains(search.ToLower()) ||
                                       (a.username ?? "").ToLower().Contains(search.ToLower()) ||
                                       (b.position_name ?? "").ToLower().Contains(search.ToLower())
                                   ))
                                   select new
                                   {
                                       a.employee_id,
                                       a.employee_name,
                                       a.username,
                                       a.base_salary,
                                       b.position_name,
                                       a.position_id
                                   }).ToList();

           if (!String.IsNullOrEmpty(sortColumn))
           {
               if (sortOrder.ToUpper() == "DESC")
               {
                   data_karyawan = sortColumn == "employee_name" ? data_karyawan.OrderByDescending(x => x.employee_name).ToList()
                                       : sortColumn == "username" ? data_karyawan.OrderByDescending(x => x.username).ToList()
                                       : sortColumn == "position_name" ? data_karyawan.OrderByDescending(x => x.position_name).ToList()
                                       : sortColumn == "base_salary" ? data_karyawan.OrderByDescending(x => x.base_salary).ToList()
                                       : data_karyawan.ToList();
               }
               else
               {
                   data_karyawan = sortColumn == "employee_name" ? data_karyawan.OrderBy(x => x.employee_name).ToList()
                                       : sortColumn == "username" ? data_karyawan.OrderBy(x => x.username).ToList()
                                       : sortColumn == "position_name" ? data_karyawan.OrderBy(x => x.position_name).ToList()
                                       : sortColumn == "base_salary" ? data_karyawan.OrderBy(x => x.base_salary).ToList()
                                       : data_karyawan.ToList();
               }
           }
           else
           {
               data_karyawan = data_karyawan.OrderBy(x => x.employee_name).ToList();
           }


           int pageSize = (page_size ?? 10);
           int pageNumber = (page ?? 1);
           var pl = data_karyawan.ToPagedList(pageNumber, pageSize);

           return Json(new
           {
               recordsTotal = count_data_karyawan,
               recordsFiltered = pl.TotalItemCount,
               pageCount = pl.PageCount,
               currentPage = pageNumber,
               data = pl,
           });
        }

        public JsonResult SimpanKaryawan()
        {
            int st = 0;
            string tombol = HttpContext.Request.Form["tombol"];
            string nama = HttpContext.Request.Form["nama_karyawan"];
            string nomor = HttpContext.Request.Form["nim"];
            string pos = HttpContext.Request.Form["posisi_karyawan"];
            string temp_base = HttpContext.Request.Form["base_salary"];
            decimal basis;
            decimal.TryParse(temp_base, out basis);

            int pos_id = Convert.ToInt16(pos);

            var maxPos = (from a in db.Position
                            where a.position_id == pos_id
                            select a.salary_max).FirstOrDefault();
            
            string maxString = maxPos.ToString();
            decimal max;
            decimal.TryParse(maxString, out max);

            if (basis > max)
            {
                st = 3;
            }
            else
            {
                if(tombol == "1")
                {
                    try
                    {
                        t_employee tabel = new t_employee();
                        tabel.employee_name = nama;
                        tabel.username = nomor;
                        tabel.position_id = Convert.ToInt16(pos);
                        tabel.base_salary = basis;
                        tabel.deleted = false;

                        db.Employee.Add(tabel);

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
                        int id = Convert.ToInt16(HttpContext.Request.Form["id_karyawan"]);
                        t_employee tabel = db.Employee.Find(id);
                        tabel.employee_name = nama;
                        tabel.position_id = Convert.ToInt16(pos);
                        tabel.base_salary = basis;
                        tabel.deleted = false;

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
            }
            return Json(st);
        }

        public JsonResult HapusKaryawan()
        {
            int st = 0;
            try
            {
                int id = Convert.ToInt16(HttpContext.Request.Form["id"]);
                t_employee tabel = db.Employee.Find(id);
                tabel.deleted = true;

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
