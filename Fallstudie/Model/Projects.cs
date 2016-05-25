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
        public string StartDate { get; set; }
        public string EndDate { get; set; }

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

        //Tage bis zu Fertigstellung
        public double DaysTilComletion
        {
            get {

                DateTime sd = DateTime.Parse(StartDate);
                DateTime ed = DateTime.Parse(EndDate);

                //Tage bis zu fertigstellung
                if (DateTime.Now > sd) return 365;
                TimeSpan days = ed - sd;
                double x = days.TotalDays;
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
