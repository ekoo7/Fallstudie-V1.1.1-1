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
        public SolidColorBrush Color { get; set; }

        public string Value { get; set; }
        public string A { get; set; }
        public string R { get; set; }
        public string G { get; set; }
        public string B { get; set; }

        public ColorPalette(Color c)
        {
            Color = new SolidColorBrush(c);
            Value = String.Format("{0} {1} {2}", c.R.ToString(), c.G.ToString(), c.B.ToString());
        }
    }
}
