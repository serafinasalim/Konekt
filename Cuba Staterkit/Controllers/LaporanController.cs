using Cuba_Staterkit.Data;
using Cuba_Staterkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.Entity.Validation;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;

namespace Cuba_Staterkit.Controllers
{
    public class LaporanController : Controller
    {
        private readonly AppDbContext db;

        public LaporanController(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult LaporanAbsensi()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var uname = HttpContext.Session.GetString("Username");
            var emp = db.Employee.FirstOrDefault(e => e.username == uname);

            return View();
        }

        public class AbsensiPerKaryawan
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeNIM { get; set; }
            public int TotalHadir { get; set; }
            public int TotalTelat { get; set; }
            public int TotalCepat { get; set; }
            public int TotalAbsen { get; set; }
        }

        public JsonResult GetLaporanAbsensi(int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int hari_kerja = CountWorkingDays(year, month);

            var employeeIds = (from a in db.Employee
                                where a.deleted == false
                                select a.employee_id).ToList();

            List<AbsensiPerKaryawan> laporanAbsensi = new List<AbsensiPerKaryawan>();

            foreach (var id in employeeIds)
            {
                var data_hadir = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "H" && a.employee_id == id
                                select a).Count();

                var data_telat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "T" && a.employee_id == id
                                select a).Count();

                var data_cepat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "C" && a.employee_id == id
                                select a).Count();

                var data_karyawan = (from a in db.Employee 
                                    where a.employee_id == id
                                    select a).FirstOrDefault();

                var data_absen = hari_kerja - data_hadir - data_telat - data_cepat;

                // Membuat objek AbsensiPerKaryawan dan menambahkannya ke daftar
                var absensiKaryawan = new AbsensiPerKaryawan
                {
                    EmployeeId      = id,
                    EmployeeName    = data_karyawan.employee_name,
                    EmployeeNIM     = data_karyawan.username,
                    TotalHadir      = data_hadir,
                    TotalTelat      = data_telat,
                    TotalCepat      = data_cepat,
                    TotalAbsen      = data_absen
                };

                laporanAbsensi.Add(absensiKaryawan);
            }
            return Json(laporanAbsensi);
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

        public ActionResult ExportLaporanAbsensi(int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int hari_kerja = CountWorkingDays(year, month);

            var employeeIds = (from a in db.Employee
                                where a.deleted == false
                                select a.employee_id).ToList();

            List<AbsensiPerKaryawan> laporanAbsensi = new List<AbsensiPerKaryawan>();

            foreach (var id in employeeIds)
            {
                var data_hadir = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "H" && a.employee_id == id
                                select a).Count();

                var data_telat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "T" && a.employee_id == id
                                select a).Count();

                var data_cepat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "C" && a.employee_id == id
                                select a).Count();

                var data_karyawan = (from a in db.Employee 
                                    where a.employee_id == id
                                    select a).FirstOrDefault();

                var data_absen = hari_kerja - data_hadir - data_telat - data_cepat;

                // Membuat objek AbsensiPerKaryawan dan menambahkannya ke daftar
                var absensiKaryawan = new AbsensiPerKaryawan
                {
                    EmployeeId      = id,
                    EmployeeName    = data_karyawan.employee_name,
                    EmployeeNIM     = data_karyawan.username,
                    TotalHadir      = data_hadir,
                    TotalTelat      = data_telat,
                    TotalCepat      = data_cepat,
                    TotalAbsen      = data_absen
                };

                laporanAbsensi.Add(absensiKaryawan);
            }

            // Membuat package dan worksheet
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Laporan Absensi");

            // Menambahkan header
            worksheet.Cells["A1"].Value = "Nama Karyawan";
            worksheet.Cells["B1"].Value = "NIM Karyawan";
            worksheet.Cells["C1"].Value = "Total Hadir";
            worksheet.Cells["D1"].Value = "Total Telat";
            worksheet.Cells["E1"].Value = "Total Cepat Pulang";
            worksheet.Cells["F1"].Value = "Total Absen";

            // Menambahkan data ke worksheet
            int row = 2;
            foreach (var absensiKaryawan in laporanAbsensi)
            {
                worksheet.Cells[$"A{row}"].Value = absensiKaryawan.EmployeeName;
                worksheet.Cells[$"B{row}"].Value = absensiKaryawan.EmployeeNIM;
                worksheet.Cells[$"C{row}"].Value = absensiKaryawan.TotalHadir;
                worksheet.Cells[$"D{row}"].Value = absensiKaryawan.TotalTelat;
                worksheet.Cells[$"E{row}"].Value = absensiKaryawan.TotalCepat;
                worksheet.Cells[$"F{row}"].Value = absensiKaryawan.TotalAbsen;
                row++;
            }

            // Menyimpan file Excel ke MemoryStream
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);

            // Mengatur posisi awal stream ke awal
            stream.Position = 0;

            // Memberikan nama file Excel yang akan diunduh
            string excelName = $"LaporanAbsensi_{year}_{month}.xlsx";

            // Memberikan file Excel sebagai respons untuk diunduh
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public IActionResult LaporanGaji()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            // Check if the user is not authenticated, then redirect to the login page
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var uname = HttpContext.Session.GetString("Username");
            var emp = db.Employee.FirstOrDefault(e => e.username == uname);

            return View();
        }

        public class GajiPerKaryawan
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeNIM { get; set; }
            public decimal GajiBulan { get; set; }
            public decimal PotTelat { get; set; }
            public decimal PotCepat { get; set; }
            public decimal PotAbsen { get; set; }
            public decimal TotalGaji { get; set; }
        }

        public JsonResult GetLaporanGaji(int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int hari_kerja = CountWorkingDays(year, month);

            var employeeIds = (from a in db.Employee
                                where a.deleted == false
                                select a.employee_id).ToList();

            List<GajiPerKaryawan> laporanGaji = new List<GajiPerKaryawan>();

            foreach (var id in employeeIds)
            {
                var data_hadir = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "H" && a.employee_id == id
                                select a).Count();

                var data_telat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "T" && a.employee_id == id
                                select a).Count();

                var data_cepat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "C" && a.employee_id == id
                                select a).Count();

                var data_karyawan = (from a in db.Employee 
                                    where a.employee_id == id
                                    select a).FirstOrDefault();

                var data_absen = hari_kerja - data_hadir - data_telat - data_cepat;

                var data_gaji = (from a in db.Employee where a.employee_id == id select a.base_salary).FirstOrDefault();

                //PERHITUNGAN
                decimal multiplier = 50000m;

                var gaji_hari = data_gaji / hari_kerja;

                var gaji_bersih = data_gaji - (data_telat * multiplier) - (data_cepat * multiplier) - (data_absen * gaji_hari);

                var gajiKaryawan = new GajiPerKaryawan
                {
                    EmployeeId  = id,
                    EmployeeName= data_karyawan.employee_name,
                    EmployeeNIM = data_karyawan.username,
                    GajiBulan   = data_karyawan.base_salary,
                    PotTelat    = data_telat * multiplier,
                    PotCepat    = data_cepat * multiplier,
                    PotAbsen    = data_absen * multiplier,
                    TotalGaji   = gaji_bersih
                };

                laporanGaji.Add(gajiKaryawan);
            }
            // Mengembalikan objek atau struktur data
            return Json(laporanGaji);
        }

        public ActionResult ExportLaporanGaji(int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int hari_kerja = CountWorkingDays(year, month);

            var employeeIds = (from a in db.Employee
                                where a.deleted == false
                                select a.employee_id).ToList();

            List<GajiPerKaryawan> laporanGaji= new List<GajiPerKaryawan>();

            foreach (var id in employeeIds)
            {
                var data_hadir = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "H" && a.employee_id == id
                                select a).Count();

                var data_telat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "T" && a.employee_id == id
                                select a).Count();

                var data_cepat = (from a in db.Attendance
                                where (a.clock_in >= startDate && a.clock_in <= endDate.AddDays(1)) ||
                                        (a.clock_out >= startDate && a.clock_out <= endDate.AddDays(1))
                                where a.status == "C" && a.employee_id == id
                                select a).Count();

                var data_karyawan = (from a in db.Employee 
                                    where a.employee_id == id
                                    select a).FirstOrDefault();

                var data_absen = hari_kerja - data_hadir - data_telat - data_cepat;

                var data_gaji = (from a in db.Employee where a.employee_id == id select a.base_salary).FirstOrDefault();

                //PERHITUNGAN
                decimal multiplier = 50000m;

                var gaji_hari = data_gaji / hari_kerja;

                var gaji_bersih = data_gaji - (data_telat * multiplier) - (data_cepat * multiplier) - (data_absen * gaji_hari);

                var gajiKaryawan = new GajiPerKaryawan
                {
                    EmployeeId  = id,
                    EmployeeName= data_karyawan.employee_name,
                    EmployeeNIM = data_karyawan.username,
                    GajiBulan   = data_karyawan.base_salary,
                    PotTelat    = data_telat * multiplier,
                    PotCepat    = data_cepat * multiplier,
                    PotAbsen    = data_absen * multiplier,
                    TotalGaji   = gaji_bersih
                };

                laporanGaji.Add(gajiKaryawan);
            }

            // Membuat package dan worksheet
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Laporan Gaji");

            // Menambahkan header
            worksheet.Cells["A1"].Value = "Nama Karyawan";
            worksheet.Cells["B1"].Value = "NIM Karyawan";
            worksheet.Cells["C1"].Value = "Gaji Pokok";
            worksheet.Cells["D1"].Value = "Pot.Terlambat";
            worksheet.Cells["E1"].Value = "Pot.Pulang Cepat";
            worksheet.Cells["F1"].Value = "Pot.Absen";
            worksheet.Cells["G1"].Value = "TOTAL GAJI";

            // Menambahkan data ke worksheet
            int row = 2;
            foreach (var gajiKaryawan in laporanGaji)
            {
                worksheet.Cells[$"A{row}"].Value = gajiKaryawan.EmployeeName;
                worksheet.Cells[$"B{row}"].Value = gajiKaryawan.EmployeeNIM;
                worksheet.Cells[$"C{row}"].Value = gajiKaryawan.GajiBulan;
                worksheet.Cells[$"D{row}"].Value = gajiKaryawan.PotTelat;
                worksheet.Cells[$"E{row}"].Value = gajiKaryawan.PotCepat;
                worksheet.Cells[$"F{row}"].Value = gajiKaryawan.PotAbsen;
                worksheet.Cells[$"G{row}"].Value = gajiKaryawan.TotalGaji;
                row++;
            }

            // Menyimpan file Excel ke MemoryStream
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);

            // Mengatur posisi awal stream ke awal
            stream.Position = 0;

            // Memberikan nama file Excel yang akan diunduh
            string excelName = $"LaporanGaji_{year}_{month}.xlsx";

            // Memberikan file Excel sebagai respons untuk diunduh
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
