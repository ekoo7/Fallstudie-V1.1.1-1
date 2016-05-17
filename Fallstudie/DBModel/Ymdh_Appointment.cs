using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_appointment")]
    public class Ymdh_Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int appointment_id { get; set; }
        public string from_ { get; set; }

        [ForeignKey(typeof(Ymdh_Appointment_Status)), ManyToOne]
        public int appointment_status_id { get; set; }

        [ForeignKey(typeof(Ymdh_House_Package)), ManyToOne]
        public int house_package_id { get; set; }
        
        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int consultant_user_id { get; set; }

        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int user_id { get; set; }
        
        [ForeignKey(typeof(Ymdh_Message)), ManyToOne]
        public int message_id { get; set; }
        
    }
}
