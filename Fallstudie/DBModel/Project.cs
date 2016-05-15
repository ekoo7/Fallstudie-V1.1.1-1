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
        [PrimaryKey, AutoIncrement]
        public int project_id { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string invoice { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public DateTime modifieddate { get; set; }

        [ForeignKey(typeof(Houseconfig)), ManyToOne]
        public int houseconfig_id { get; set; }

    }
}
