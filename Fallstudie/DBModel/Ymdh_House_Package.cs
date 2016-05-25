using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_house_package")]
    public class Ymdh_House_Package
    {
        [PrimaryKey, AutoIncrement]
        public int house_package_id { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int price { get; set; }

        [ForeignKey(typeof(Ymdh_House_Package_Status)), ManyToOne]
        public int house_package_status { get; set; }

        [ForeignKey(typeof(Ymdh_Producer)), ManyToOne]
        public int producer_id { get; set; }

        [ForeignKey(typeof(Ymdh_Address)), ManyToOne]
        public int address_id { get; set; }

        public int housefloors { get; set; }
    }
}
