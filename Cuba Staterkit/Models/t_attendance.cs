using System.ComponentModel.DataAnnotations.Schema;

namespace Cuba_Staterkit.Models
{
    [Table("t_attendance")]
    public class t_attendance
    {
        public int attendance_id { get; set; }
        public int employee_id { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime tgl { get; set; }  
        public t_employee Employee { get; set; }
        public DateTime clock_in { get; set; }
        public DateTime clock_out { get; set;}
        public string status { get; set; }
    }
}
