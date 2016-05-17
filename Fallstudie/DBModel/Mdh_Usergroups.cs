using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("mdh_usergroups")]
    public class Mdh_Usergroups
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int parent_id { get; set; }
        public int lft { get; set; }
        public int rgt { get; set; }
        public string title { get; set; }
    }
}
