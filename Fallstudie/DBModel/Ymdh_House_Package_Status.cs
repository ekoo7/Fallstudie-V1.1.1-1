using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_house_package_status")]
    public class Ymdh_House_Package_Status
    {
        [PrimaryKey, AutoIncrement]
        public int house_package_status_id { get; set; }
        public string description { get; set; }
    }
}
