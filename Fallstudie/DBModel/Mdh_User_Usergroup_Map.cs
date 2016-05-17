using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("mdh_user_usergroup_map")]
    public class Mdh_User_Usergroup_Map
    {
        [ForeignKey(typeof(Mdh_Users)), ManyToOne]
        public int user_id { get; set; }

        [ForeignKey(typeof(Mdh_Usergroups)), ManyToOne]
        public int group_id { get; set; }
    }
}
