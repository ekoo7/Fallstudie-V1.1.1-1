using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("attribute")]
    public class Attribute
    {
        [PrimaryKey, AutoIncrement]
        public int attribute_id { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public string image { get; set; }
        public int deleted { get; set; }
        public string modifieddate { get; set; }

        [ForeignKey(typeof(Attribute_Group)), ManyToOne]
        public int attribute_group_id { get; set; }
        public string rootfolder { get; set; }

    }
}
