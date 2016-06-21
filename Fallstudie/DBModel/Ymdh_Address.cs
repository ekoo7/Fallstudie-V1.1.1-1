using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_address")]
    public class Ymdh_Address
    {
        [PrimaryKey]
        public int mdh_address_id { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string houseno { get; set; }
        public string country { get; set; }
    }
}
