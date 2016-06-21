using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_appointment_status")]
    public class Ymdh_Appointment_Status
    {
        [PrimaryKey]
        public int appointment_status_id { get; set; }
        public string description { get; set; }
    }
}
