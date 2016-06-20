using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_message")]
    public class Ymdh_Message
    {
        [PrimaryKey]
        public int message_id { get; set; }
        public string summary { get; set; }
        public string message { get; set; }
        public string message_date { get; set; }
        public string message_type { get; set; }
        
        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int user_id { get; set; }
    }
}
