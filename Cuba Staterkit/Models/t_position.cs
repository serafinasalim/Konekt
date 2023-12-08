using System.ComponentModel.DataAnnotations.Schema;

namespace Cuba_Staterkit.Models
{

    [Table("t_position")]
    public class t_position
    {
        public int position_id { get; set; }
        public string position_name { get; set; }
        public List<t_employee> Employee { get; set; }
        public decimal salary_min { get; set; }
        public decimal salary_max { get; set; }
        public bool deleted { get; set; }
    }
}
