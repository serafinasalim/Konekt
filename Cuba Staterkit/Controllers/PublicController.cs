using Cuba_Staterkit.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cuba_Staterkit.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext _dbContext;

        public PublicController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public JsonResult GetUserInfo()
        {
            var uname = HttpContext.Session.GetString("Username");
            var employee = _dbContext.Employee.FirstOrDefault(e => e.username == uname);
            var acc = _dbContext.Account.FirstOrDefault(e => e.username == uname);
            var pos = _dbContext.Position.FirstOrDefault(e => e.position_id == employee.position_id);

            if (employee != null)
            {
                var result = new
                {
                    Success = true,
                    nama_karyawan = employee.employee_name,
                    id_karyawan = employee.employee_id,
                    hak_akses = acc.role,
                    nama_pos = pos.position_name,
                    nim_user = employee.username
                };
                return Json(result);
            }
            else
            {
                var result = new
                {
                    Success = false,
                    ErrorMessage = "Unknown"
                };
                return Json(result);
            }
        }
    }
}
