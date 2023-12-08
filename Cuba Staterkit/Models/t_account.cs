using System.ComponentModel.DataAnnotations.Schema;

namespace Cuba_Staterkit.Models
{

    [Table("t_account")]
    public class t_account
    {
        public int user_id { get; set; }
        //foreign key
        public string username { get; set; }
        public t_employee Employee { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public bool inactive { get; set; }
    }
}
