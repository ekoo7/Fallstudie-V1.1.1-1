using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("houseconfig_has_attribute")]
    public class Houseconfig_Has_Attribute
    {
        [ForeignKey(typeof(Houseconfig)), ManyToOne]
        public int houseconfig_id { get; set; }
        [ForeignKey(typeof(Attribute)), ManyToOne]
        public int attribute_id { get; set; }

        public int amount { get; set; }
        public string special { get; set; }
        public DateTime modifieddate { get; set; }
    }
}
