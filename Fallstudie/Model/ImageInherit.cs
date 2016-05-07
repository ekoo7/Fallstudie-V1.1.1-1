using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
namespace Fallstudie.Model
{
    public class ImageInherit
    {
        public Image Image1 { get; set; }
        public BitmapImage BitmapImage1 { get; set; }
        public Uri Uri1 { get; set; }
        public string SourceImage { get; set; }
        public string Preis { get; set; }
        public ImageInherit(string source, int id, string preis)
        {
            Image1 = new Image();
            //Image1.Width = 40;
            //Image1.Height = 40;
            BitmapImage1 = new BitmapImage();
            Uri1 = new Uri(source, UriKind.Absolute);
            BitmapImage1.UriSource = Uri1;
            Image1.Source = BitmapImage1;
            Image1.Name = id.ToString();
            SourceImage = source;
            Preis = preis;
        }
    }
}
