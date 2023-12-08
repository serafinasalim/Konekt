using System.ComponentModel.DataAnnotations.Schema;

namespace Cuba_Staterkit.Models
{

    [Table("t_employee")]
    public class t_employee
    {
        public int employee_id { get; set; }
        public List<t_attendance> Attendance { get; set; }
        public string employee_name { get; set; }
        public int position_id { get; set; }
        public t_position Position { get; set; }
        public string username { get; set; }
        public t_account Account { get; set; }
        public decimal base_salary { get; set; }
        public bool deleted { get; set; }
    }
}
