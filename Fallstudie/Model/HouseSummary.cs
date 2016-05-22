using Fallstudie.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallstudie.Model
{
    public class HouseSummary
    {
        public Customer Customer { get; set; }
        public Consultant Consultant { get; set; }

        public ImageInherit Package { get; set; }

        //Grundstück
        public ImageInherit Plot { get; set; }

        //anzahl der Stockwerke
        public int numberOfFloors { get; set; }

        //Grundrisse
        public List<ImageInherit> GroundPlots { get; set; }


        //Wände
        public ImageInherit OutsideWall { get; set; }
        public ColorPalette OutsideWallColor { get; set; }

        public ImageInherit InsideWall { get; set; }
        public ColorPalette InsideWallColor { get; set; }


        //Dach
        public ImageInherit RoofType { get; set; }
        public ImageInherit RoofMaterial { get; set; }

        //Fenster und Türen
        public ImageInherit Window { get; set; }
        public ColorPalette WindowColor { get; set; }

        public ImageInherit Door { get; set; }
        public ColorPalette DoorColor { get; set; }

        //Energiesystem && Heating System
        public EHSystem EnergySystem { get; set; }
        public EHSystem HeatingSystem { get; set; }

        //steckdose pro Raum && Kamin
        public int NumberOfSocket { get; set; }
        public ImageInherit Chimney { get; set; }


        //Pool und Zaun und ZaunFarbe
        public ImageInherit Pool { get; set; }
        public string Poolsize { get; set; }
        public ImageInherit Fence { get; set; }
        public ColorPalette FenceColor { get; set; }

    }
}
