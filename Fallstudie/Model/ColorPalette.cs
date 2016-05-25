using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Fallstudie.Model
{
    public class ColorPalette
    {
        public int Id { get; set; }
        public SolidColorBrush Color { get; set; }

        public string Value { get; set; }
        public string A { get; set; }
        public string R { get; set; }
        public string G { get; set; }
        public string B { get; set; }

        public byte R_byte { get; set; }
        public byte G_byte { get; set; }
        public byte B_byte { get; set; }
        public ColorPalette()
        {

        }
        public ColorPalette(Color c)
        {
            Color = new SolidColorBrush(c);
            Value = String.Format("{0} {1} {2}", c.R.ToString(), c.G.ToString(), c.B.ToString());
        }

        public ColorPalette(int id, byte r, byte g, byte b)
        {
            this.Id = id;
            R_byte = r;
            G_byte = g;
            B_byte = b;
            Color c = new Windows.UI.Color();
            c.R = r;
            c.G = g;
            c.B = b;
            Color = new SolidColorBrush(c);
            Value = String.Format("{0} {1} {2}", c.R.ToString(), c.G.ToString(), c.B.ToString());
        }
    }
}
