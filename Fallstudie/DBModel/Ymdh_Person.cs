using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("ymdh_person")]
    public class Ymdh_Person
    {
        [PrimaryKey, AutoIncrement]
        public int mdh_person { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string sex { get; set; }
        public string birthdate { get; set; }
    }
}
