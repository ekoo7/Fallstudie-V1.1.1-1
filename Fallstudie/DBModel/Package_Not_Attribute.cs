using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("package_not_attribute")]
    public class Package_Not_Attribute
    {
        [ForeignKey(typeof(Ymdh_House_Package)), ManyToOne]
        public int house_package_id { get; set; }

        [ForeignKey(typeof(Attribute)), ManyToOne]
        public int attribute_id { get; set; }
    }
}
