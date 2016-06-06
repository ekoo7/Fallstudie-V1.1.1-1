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
        private string startDate;

        public string StartDate
        {
            get {
                string[] startDateSplit = startDate.Split(' ');
                startDate = startDateSplit[0];
                return startDate;
            }
            set { startDate = value; }
        }

        private string endDate;

        public string EndDate
        {
            get
            {
                string[] endDateSplit = endDate.Split(' ');
                endDate = endDateSplit[0];
                return endDate;
            }
            set { endDate = value; }
        }


        /// <summary>
        /// Projekstauts kann 5 Werte Annhmen
        /// 1. Projektvorbereitung
        /// 2. Projektplanung
        /// 3. Ausführungsvorbereitung
        /// Die Ausführungs- und Detailplanung der Objekt und Fachplaner wird überprüft, koordiniert und begleitet, sowie die erforderlichen Entscheidungen der Auftraggeber vorbereitet und herbeigeführt. 
        /// 4. Projektausführung
        /// 5. Projektabschluss
        /// 6. Abgeschlossen
        /// </summary>
        public string State { get; set; }

        public int StateValue
        {
            get {

                double x = DaysTilComletion;
                if (x >= 292) return 0;
                if (x >= 219 && x < 292) return 20;
                if (x >= 146 && x < 219) return 40;
                if (x >= 73 && x < 146) return 60;
                if (x > 0 && x < 73) return 80;
                else return 100;

            }
        }
        private string stateDescription;

        public string StateDescription
        {
            get {
                
                if (DaysTilComletion >= 340) return "Projektvorbereitung";
                if (DaysTilComletion >= 300 && DaysTilComletion < 340) return "Projektplanung";
                if (DaysTilComletion >= 290 && DaysTilComletion < 300) return "Ausführungsvorbereitung";
                if (DaysTilComletion >= 30 && DaysTilComletion < 290) return "Projektausführung";
                if (DaysTilComletion > 0 && DaysTilComletion < 30) return "Projektabschluss";
                else return "Abgeschlossen";
            }
            set {
                stateDescription = value;
            }
        }


        //Tage bis zu Fertigstellung
        public int DaysTilComletion
        {
            get {

                DateTime sd = DateTime.Parse(StartDate);
                DateTime ed = DateTime.Parse(EndDate);
                TimeSpan days;
                //Tage bis zu fertigstellung
                if (DateTime.Now > sd)
                {
                    days = ed - DateTime.Now;
                }
                else
                {
                    days = ed - sd;
                }
                
                
                
                int x = (int)days.TotalDays;
                return x;
            }
        }


        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Invoice { get; set; }

        public Projects()
        {

        }

    }
}
