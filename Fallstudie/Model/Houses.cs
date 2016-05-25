using Fallstudie.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class Houses
    {
        public int PackageId { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string Country { get; set; }
    }
}
