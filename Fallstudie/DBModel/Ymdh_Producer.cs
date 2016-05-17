using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_producer")]
    public class Ymdh_Producer
    {
        [PrimaryKey, AutoIncrement]
        public int mdh_producer_int { get; set; }

        [ForeignKey(typeof(Ymdh_Person)), ManyToOne]
        public int person_id { get; set; }

        [ForeignKey(typeof(Ymdh_Address)), ManyToOne]
        public int address_id { get; set; }

        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int consultant_user_id { get; set; }

        public string company { get; set; }
    }
}