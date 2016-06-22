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
        public string Message { get; set; }
        private string messagePackage;

        public string MessagePackage
        {
            
            get {
                if (Message != null) { 
                    string[] splitMessage = Message.Split(' ');
                    try
                    {
                        string[] splitMessagePackage = splitMessage[4].Split(',');
                        messagePackage = splitMessage[3] + ' ' + splitMessagePackage[0];
                    }
                    catch (Exception) { }
                }
                return messagePackage;
            }
            set { messagePackage = value; }
        }


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
