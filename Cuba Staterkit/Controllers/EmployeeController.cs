using Cuba_Staterkit.Data;
using Cuba_Staterkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.Entity.Validation;

namespace Cuba_Staterkit.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext db;

        public EmployeeController(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Attendance()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var uname = HttpContext.Session.GetString("Username");
            var emp = db.Employee.FirstOrDefault(e => e.username == uname);

            ViewBag.id = emp.employee_id;

            return View();
        }
        
        public List<Tuple<DateTime, string>> GetDateWithDayForMonth(int year, int month)
        {
            List<Tuple<DateTime, string>> dateWithDayList = new List<Tuple<DateTime, string>>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Menggunakan Tuple untuk menyimpan pasangan tanggal dan hari
                Tuple<DateTime, string> dateWithDay = new Tuple<DateTime, string>(date, date.ToString("dddd"));
                dateWithDayList.Add(dateWithDay);
            }

            return dateWithDayList;
        }

        public class AbsensiDTO
        {
            public DateTime jam_masuk { get; set; }
            public DateTime jam_keluar { get; set; }
            public int idk { get; set; }
            public string status { get; set; }
        }

        public List<Tuple<DateTime, string, AbsensiDTO>> GetAbsensi(int year, int month, int id)
        {
            List<Tuple<DateTime, string, AbsensiDTO>> dateWithDayAndAbsensiList = new List<Tuple<DateTime, string, AbsensiDTO>>();

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var absensiInfos = db.Attendance
                .Where(a => (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)))
                .Where(a => a.employee_id == id)
                .Select(a => new AbsensiDTO
                {
                    idk = a.employee_id,
                    jam_masuk = a.clock_in,
                    jam_keluar = a.clock_out,
                    status = a.status ?? ""
                })
                .ToList();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Ambil data absensi untuk tanggal tertentu dari koleksi yang sudah diambil sebelumnya
                var absensiInfo = absensiInfos
                    .Where(a => (a.jam_masuk.Date <= date && a.jam_keluar.Date >= date.AddDays(1)) ||
                                (a.jam_masuk.Date == date))
                    .FirstOrDefault();
                string dayOfWeek = date.ToString("dddd");

                // Tambahkan ke dalam Tuple
                Tuple<DateTime, string, AbsensiDTO> dateWithDayAndAbsensi = new Tuple<DateTime, string, AbsensiDTO>(date, dayOfWeek, absensiInfo);
                dateWithDayAndAbsensiList.Add(dateWithDayAndAbsensi);
            }

            return dateWithDayAndAbsensiList;
        }

        public JsonResult ClockIn(int id)
        {
            int st = 0;
            DateTime datenow = DateTime.Now;

            var absenHari = (from a in db.Attendance 
                            where a.tgl.Date == datenow.Date &&
                            a.employee_id == id
                            select a.tgl).FirstOrDefault();

            if(absenHari == DateTime.MinValue)
            {
                try
                {
                    t_attendance tabel = new t_attendance();
                    tabel.employee_id = id;
                    tabel.clock_in = datenow;
                    tabel.tgl = datenow.Date;
                    tabel.status = "H";

                    db.Attendance.Add(tabel);

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
            else{
                st = 2;
            }
            return Json(st);
        }
        public class AttendanceData
        {
            public int AttendanceId { get; set; }
            public int EmployeeId { get; set; }
            public DateTime Tgl { get; set; }
            public DateTime ClockIn { get; set; }
            public DateTime? ClockOut { get; set; }
            // Tambahkan properti lain sesuai kebutuhan
        }
        public JsonResult ClockOut(int id)
        {
            int st = 0;
            bool jam_out = false;
            DateTime datenow = DateTime.Now;

            var absenHari = (from a in db.Attendance 
                            where a.tgl.Date == datenow.Date &&
                            a.employee_id == id
                            select a.tgl).FirstOrDefault();
            
            var raw = (from a in db.Attendance
                       where a.tgl.Date == datenow.Date &&
                       a.employee_id == id
                       select a.clock_out).FirstOrDefault();

            if (raw == DateTime.MinValue) 
            {
                jam_out = true;
            }

            if(absenHari != null && jam_out == true)
            {
                try
                {
                    var id_att = (from a in db.Attendance
                                  where a.tgl.Date == datenow.Date &&
                                  a.employee_id == id
                                  select new{
                                      AttendanceId = a.attendance_id,
                                      EmployeeId = a.employee_id,
                                      Tgl = a.tgl,
                                      ClockIn = a.clock_in,
                                      ClockOut = a.clock_out,
                                  }).FirstOrDefault();
                    DateTime? clockout = id_att.ClockOut;
                    int id_ubah = id_att.AttendanceId;

                    try
                    {
                        var attendanceToUpdate = db.Attendance.Find(id_ubah);
                        if (attendanceToUpdate != null)
                        {
                            attendanceToUpdate.clock_out = DateTime.Now;
                            db.SaveChanges();
                            var masuk = (from a in db.Attendance
                                         where a.tgl.Date == datenow.Date &&
                                         a.employee_id == id
                                         select a.clock_in).FirstOrDefault();
                            var keluar = (from a in db.Attendance
                                          where a.tgl.Date == datenow.Date &&
                                          a.employee_id == id
                                          select a.clock_out).FirstOrDefault();

                            DateTime clock_in = Convert.ToDateTime(masuk);
                            DateTime clock_out = Convert.ToDateTime(keluar);
                            attendanceToUpdate.status = StatusKehadiran(clock_in, clock_out);

                            db.SaveChanges();
                            st = 1;
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
            else if (absenHari == null)
            {
                st = 2;
            }
            else if (jam_out == false)
            {
                st = 3;
            }
            else
            {
                st = 0;
            }
            return Json(st);
        } 

        public string StatusKehadiran(DateTime clock_in, DateTime clock_out)
        {
            DateTime masuk = clock_in;
            DateTime keluar = clock_out;
            string stat = "";

            if (masuk.TimeOfDay <= new TimeSpan(9, 0, 0) &&  keluar.TimeOfDay >= new TimeSpan(17, 0, 0))
            {
                return stat = "H";
            }
            else if (masuk.TimeOfDay > new TimeSpan(9, 0, 0) && keluar.TimeOfDay >= new TimeSpan(17, 0, 0))
            {
                return stat = "T";
            }
            else if (masuk.TimeOfDay <= new TimeSpan(9, 0, 0) && keluar.TimeOfDay < new TimeSpan(17, 0, 0))
            {
                return stat = "C";
            }
            else if (masuk.TimeOfDay > new TimeSpan(9, 0, 0) && keluar.TimeOfDay < new TimeSpan(17, 0, 0))
            {
                return stat = "C";
            }

            return stat;
        }

        public IActionResult Salary()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var uname = HttpContext.Session.GetString("Username");
            var emp = db.Employee.FirstOrDefault(e => e.username == uname);

            ViewBag.id = emp.employee_id;
            return View();
        }

        public class SalaryData
        {
            public decimal TotalGaji { get; set; }
            public int? TotalHadir { get; set; }
            public int? TotalTelat { get; set; }
            public int? TotalCepat { get; set; }
            public int? TotalAbsen { get; set; }
            public decimal PotTelat { get; set; }
            public decimal PotCepat { get; set; }
            public decimal PotAbsen { get; set; }
            public decimal GajiBersih { get; set; }
        }

        public JsonResult GetSalary(int year, int month, int id)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int hari_kerja = CountWorkingDays(year, month);

            var data_hadir = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                      (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "H" && a.employee_id == id // Gantilah dengan status yang diinginkan
                                select a).Count();

            var data_telat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                      (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "T" && a.employee_id == id  // Gantilah dengan status yang diinginkan
                                select a).Count();
            
            var data_cepat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                      (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "C" && a.employee_id == id  // Gantilah dengan status yang diinginkan
                                select a).Count();
            
            var data_gaji = (from a in db.Employee where a.employee_id == id select a.base_salary).FirstOrDefault();

            var data_absen = hari_kerja - data_hadir - data_telat - data_cepat;

            //PERHITUNGAN
            decimal multiplier = 50000m;

            var gaji_hari = data_gaji / hari_kerja;

            var gaji_bersih = data_gaji - (data_telat * multiplier) - (data_cepat * multiplier) - (data_absen * gaji_hari);

            var salaryData = new SalaryData
            {
                TotalGaji = data_gaji,
                TotalHadir = data_hadir,
                TotalTelat = data_telat,
                TotalCepat = data_cepat,
                TotalAbsen = data_absen,
                PotTelat = data_telat * multiplier,
                PotCepat = data_cepat * multiplier,
                PotAbsen = data_absen * gaji_hari,
                GajiBersih = gaji_bersih
            };

            // Mengembalikan objek atau struktur data
            return Json(salaryData);
        }

        public int CountWorkingDays(int year, int month)
        {
            int workingDays = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(year, month, day);
                
                // Memeriksa apakah hari tersebut bukan Sabtu atau Minggu
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }
    }
}
