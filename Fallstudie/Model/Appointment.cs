using Fallstudie.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class Appointment
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Consultant { get; set; }
        public Customer Customer { get; set; }
    }
}
