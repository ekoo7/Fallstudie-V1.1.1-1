using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("attribute_group")]
    public class Attribute_Group
    {
        [PrimaryKey]
        public int attribute_group_id { get; set; }
        public string description { get; set; }
        public DateTime modifieddate { get; set; }
    }
}
