using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("houseconfig")]
    public class Houseconfig
    {
        [PrimaryKey]
        public int houseconfig_id { get; set; }
        public int price { get; set; }
        public string status { get; set; }
        public int price_floor { get; set; }
        public string modifieddate { get; set; }

        
        [ForeignKey(typeof(Ymdh_House_Package)), ManyToOne]
        public int house_package_id { get; set; }
        
        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int consultant_user_id { get; set; }

        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int customer_user_id { get; set; }
    }
}
