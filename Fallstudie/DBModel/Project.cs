using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;

namespace Fallstudie.DBModel
{
    [Table("project")]
    public class Project
    {
        [PrimaryKey]
        public int project_id { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string invoice { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string modifieddate { get; set; }

        [ForeignKey(typeof(Houseconfig)), ManyToOne]
        public int houseconfig_id { get; set; }

        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int customer_user_id { get; set; }

    }
}
