using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("temp_table")]
    public class Temp_Table
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public string rootfolder { get; set; }
    }
}
