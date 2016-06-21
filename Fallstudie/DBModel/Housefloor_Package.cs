using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("housefloor_package")]
    public class Housefloor_Package
    {
        [PrimaryKey]
        public int housefloor_id { get; set; }
        public int price { get; set; }
        public string sketch { get; set; }
        public string modifieddate { get; set; }

        [ForeignKey(typeof(Ymdh_House_Package)), ManyToOne]
        public int house_package_id { get; set; }

        public decimal area { get; set; }
        public string rootfolder { get; set; }
    }
}
