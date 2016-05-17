using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    [Table("mdh_users")]
    public class Mdh_Users
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int block { get; set; }
        public int sendEmail { get; set; }
        public DateTime register_date { get; set; }
        public DateTime lastvisitDate { get; set; }
        public string activation { get; set; }
        public string Params { get; set; }
        public DateTime lastResetTime { get; set; }
        public int resetCount { get; set; }
        public string otpKey { get; set; }
        public string otep { get; set; }
        public int requireReset { get; set; }
    }
}
