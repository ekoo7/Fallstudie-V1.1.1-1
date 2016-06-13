using Fallstudie.ViewModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public Consultant Consultant { get; set; }
        public Customer Customer { get; set; }

        public string DateFormat
        {
            get { return Date.ToString("dd.MM.yyyy"); }
        }
        public string TimeFormat
        {
            get { return Time.ToString("hh\\:mm"); }
        }
    }
}
