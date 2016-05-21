using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class Projects
    {
        public int Id { get; set; }
        public HouseSummary House { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Invoice { get; set; }

        public Projects()
        {

        }

    }
}
