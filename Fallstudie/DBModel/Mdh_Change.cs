using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("mdh_change")]
    public class Mdh_Change
    {
        [PrimaryKey, AutoIncrement]
        public int cid { get; set; }
        public string ctable { get; set; }
        public int id { get; set; }
        public int id2 { get; set; }
        public string change { get; set; }
        public string dt { get; set; }
        public int synced { get; set; }
    }
}
