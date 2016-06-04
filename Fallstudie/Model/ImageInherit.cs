using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
namespace Fallstudie.Model
{
    public class ImageInherit
    {
        public int Id { get; set; }
        public Image Image1 { get; set; }
        public BitmapImage BitmapImage1 { get; set; }
        public Uri Uri1 { get; set; }
        public string SourceImage { get; set; }

        public double Price { get; set; }

        public string PriceFormat
        {
            get {
                return String.Format("{0} €" , Price);
            }

        }

        public string Name { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public RelayCommand ButtonDrawSketch { get; set; }
        public RelayCommand ButtonUploadSketch { get; set; }


        public ImageInherit()
        {

        }

        //konstruktor für Poolsize, Socket
        public ImageInherit(int id, string desc, double price)
        {
            Id = id;
            Description = desc;
            Price = price;
        }

        //Konstruktor für Grundriss
        public ImageInherit(string source, int id, decimal area, RelayCommand btn, RelayCommand btn2, int floors, int floorsDB)
        {

            Image1 = new Image();
            BitmapImage1 = new BitmapImage();
            Uri1 = new Uri(source, UriKind.Absolute);
            BitmapImage1.UriSource = Uri1;
            Image1.Source = BitmapImage1;
            SourceImage = source;
            Id = id;
            Image1.Name = id.ToString();

            if (floors > floorsDB && floors - 1 == floorsDB) Price = 1000;
            else if (floors > floorsDB && floors - 2 == floorsDB) Price = 2000;
            else if (floors < floorsDB && floors + 1 == floorsDB) Price = -1000;
            else if (floors < floorsDB && floors + 2 == floorsDB) Price = -2000;
            else Price = 0;

            if (area == 0)
            {
                Description = "Erdgeschoss";
            }
            else if (area == 1)
            {
                Description = "Stockwerk 1";
            }
            else if (area == 2)
            {
                Description = "Stockwerk 2";
            }

            ButtonDrawSketch = btn;
            ButtonUploadSketch = btn2;
        }

        public ImageInherit(string source, int id, string description, double price)
        {
            Image1 = new Image();
            BitmapImage1 = new BitmapImage();
            Uri1 = new Uri(source, UriKind.Absolute);
            BitmapImage1.UriSource = Uri1;
            Image1.Source = BitmapImage1;
            SourceImage = source;
            Id = id;
            Image1.Name = id.ToString();
            Description = description;
            Price = price;
        }

        //Konstruktor für die Hauspackages
        public ImageInherit(string source, int id, string description, double price, string zip, string city, string street, string houseNo, string country)
        {
            Image1 = new Image();
            BitmapImage1 = new BitmapImage();
            Uri1 = new Uri(source, UriKind.Absolute);
            BitmapImage1.UriSource = Uri1;
            Image1.Source = BitmapImage1;
            SourceImage = source;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = description.Split(delimiter,3);


            Id = id;
            Name = split[0];
            Size = split[1];
            Description = split[2];
            Price = price;
            Zip = zip;
            City = city;
            Address = street + " " + houseNo;
            Country = country;

        }
    }
}
