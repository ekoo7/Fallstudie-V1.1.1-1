using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.DBModel
{
    public class View_Appointment
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int HousePackageId { get; set; }
        public int MessageId { get; set; }
        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }
    }
}
