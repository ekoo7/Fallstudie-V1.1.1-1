﻿using Fallstudie.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Fallstudie.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        /// <summary>
        /// PROPERTIES
        /// </summary>

        //Preis für den gesammten
        private double totalPrice = 0;

        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; OnChange("TotalPrice"); }
        }

        //Anzahl der Stockwerke aus der DB
        private int numberOfFloorDB;

        public int NumberOfFloorDB
        {
            get { return numberOfFloorDB; }
            set { numberOfFloorDB = value; }
        }



        //Test Variable
        private string msg = "HausKonfigurator";
        public string MSG
        {
            get { return msg; }
            set { msg = value; }
        }


        /// <summary>
        /// List Items
        /// Hier werden die Properties für die Listen festgelegt
        /// </summary>
        

        //Customer die in der Liste gespeichert sind
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get { return customers; }
            set { customers = value; OnChange("Items"); }
        }

        // der ausgewählte Kunde in der Kundenliste
        private Customer selectedCustomerr;
        public Customer SelectedCustomerr
        {
            get { return selectedCustomerr; }
            set { selectedCustomerr = value; OnChange("SelectedCustomerr"); }
        }

        //Frame wird initialisiert - damit wir die Frame, wo alle Pages angezeigt werden haben
        Frame a = new Frame();

        //StackPanel von Schritt2
        StackPanel stackPanelS2 = new StackPanel();

        //Liste für Häuse aus der DB
        private ObservableCollection<ImageInherit> imagesHouse = new ObservableCollection<ImageInherit>();
        public ObservableCollection<ImageInherit> ImagesHouse
        {
            get { return imagesHouse; }
            set { imagesHouse = value; OnChange("ImagesHouse"); }
        }
        //Liste für Grundstücke
        private ObservableCollection<ImageInherit> imagesPlot = new ObservableCollection<ImageInherit>();
        public ObservableCollection<ImageInherit> ImagesPlot
        {
            get { return imagesPlot; }
            set { imagesPlot = value; OnChange("ImagesPlot"); }
        }

        //Die Anzahl der Stockwerke
        private ObservableCollection<int> numberFloors = new ObservableCollection<int>();

        public ObservableCollection<int> NumberFloors
        {
            get { return numberFloors; }
            set {
                numberFloors = value;
            }
        }

        //Die Liste für die Grundrisse der Stockwerke
        private ObservableCollection<ImageInherit> floorsGroundPlot = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> FloorsGroundPlot
        {
            get { return floorsGroundPlot; }
            set { floorsGroundPlot = value; }
        }

        //Liste Außenwände
        private ObservableCollection<ImageInherit> listOutsideWall = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListOutsideWall
        {
            get { return listOutsideWall; }
            set { listOutsideWall = value; }
        }

        //Liste für Farben der Außenwände
        private ObservableCollection<ColorPalette> listColorOutsideWall = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorOutsideWall
        {
            get { return listColorOutsideWall; }
            set { listColorOutsideWall = value; }
        }


        //Liste für die Innenwände
        private ObservableCollection<ImageInherit> listInsideWall = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListInsideWall
        {
            get { return listInsideWall; }
            set { listInsideWall = value; }
        }
        //Liste für die Farben der Innenwände
        private ObservableCollection<ColorPalette> listColorInsideWall = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorInsideWall
        {
            get { return listColorInsideWall; }
            set { listColorInsideWall = value; }
        }

        //Liste für die Dachtypen
        private ObservableCollection<ImageInherit> listRoofType = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListRoofType
        {
            get { return listRoofType; }
            set { listRoofType = value; }
        }

        //Liste fpr das Dachmaterial
        private ObservableCollection<ImageInherit> listRoofMaterial = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListRoofMaterial
        {
            get { return listRoofMaterial; }
            set { listRoofMaterial = value; }
        }

        //Liste für die Fenster
        private ObservableCollection<ImageInherit> listWindows = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListWindows
        {
            get { return listWindows; }
            set { listWindows = value; }
        }

        //Liste für die Farben der Fenster
        private ObservableCollection<ColorPalette> listColorWindows = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorWindows
        {
            get { return listColorWindows; }
            set { listColorWindows = value; OnChange("ListColorWindows"); }
        }


        //Liste für die Türen
        private ObservableCollection<ImageInherit> listDoors = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListDoors
        {
            get { return listDoors; }
            set { listDoors = value; }
        }

        //Liste für Farben der Türen
        private ObservableCollection<ColorPalette> listColorDoors = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorDoors
        {
            get { return listColorDoors; }
            set { listColorDoors = value; OnChange("ListColorDoors"); }
        }



        /// <summary>
        /// Selected Items
        /// Hier werden die Properties für die Selected Items festgelegt
        /// </summary>

        //selected Anzahl der Stockwerk
        private int selectedItemFloor;

        public int SelectedItemFloor
        {
            get { return selectedItemFloor; }
            set {
                selectedItemFloor = value;
                
                ChooseGroundPlots();
                selectedFloor = FloorsGroundPlot[0];
            }
        }


        //selected Haus
        private ImageInherit selectedHouse;
        public ImageInherit SelectedHouse
        {
            get { return selectedHouse; }
            set { selectedHouse = value; OnChange("SelectedHouse"); }
        }
        //selected Grundstück
        private ImageInherit selectedPlot;
        public ImageInherit SelectedPlot
        {
            get { return selectedPlot; }
            set { selectedPlot = value; OnChange("SelectedPlot"); }
        }

        //selected Grundriss der Stockwerke
        private ImageInherit selectedFloor;
        public ImageInherit SelectedFloor
        {
            get { return selectedFloor; }
            set { selectedFloor = value; OnChange("SelectedFloor"); }
        }

        //selected Außenwand
        private ImageInherit selectedOutsideWall;

        public ImageInherit SelectedOutsideWall
        {
            get { return selectedOutsideWall; }
            set { selectedOutsideWall = value; OnChange("SelectedOutsideWall"); }
        }
            
        //selected Farben Außenwände
        private ColorPalette selectedColorOutsideWall;

        public ColorPalette SelectedColorOutsideWall
        {
            get { return selectedColorOutsideWall; }
            set { selectedColorOutsideWall = value; OnChange("SelectedColorOutsideWall"); }
        }

        //selected Innside
        private ImageInherit selectedInsideWall;

        public ImageInherit SelectedInsideWall
        {
            get { return selectedInsideWall; }
            set { selectedInsideWall = value; OnChange("SelectedInsideWall"); }
        }

        //selected Farbe Innenwände
        private ColorPalette selectedColorInsideWall;

        public ColorPalette SelectedColorInsideWall
        {
            get { return selectedColorInsideWall; }
            set { selectedColorInsideWall = value; OnChange("SelectedColorInsideWall"); }
        }

        //selected Dach Typ
        private ImageInherit selectedRoofType;

        public ImageInherit SelectedRoofType
        {
            get { return selectedRoofType; }
            set { selectedRoofType = value; OnChange("SelectedRoofType"); }
        }

        //selected Dach Typ
        private ImageInherit selectedRoofMaterial;

        public ImageInherit SelectedRoofMaterial
        {
            get { return selectedRoofMaterial; }
            set { selectedRoofMaterial = value; OnChange("SelectedRoofMaterial"); }
        }

        //Selected Fenster
        private ImageInherit selectedWindow;

        public ImageInherit SelectedWindow
        {
            get { return selectedWindow; }
            set { selectedWindow = value; OnChange("SelectedWindow"); }
        }

        //selected Farbe für Fenster
        private ColorPalette selectedColorWindow;

        public ColorPalette SelectedColorWindow
        {
            get { return selectedColorWindow; }
            set { selectedColorWindow = value; OnChange("SelectedColorWindow"); }
        }


        //Selected Tür
        private ImageInherit selectedDoor;

        public ImageInherit SelectedDoor
        {
            get { return selectedDoor; }
            set { selectedDoor = value; OnChange("SelectedDoor"); }
        }

        //selected Farbe für Türen
        private ColorPalette selectedColorDoor;

        public ColorPalette SelectedColorDoor
        {
            get { return selectedColorDoor; }
            set { selectedColorDoor = value; OnChange("SelectedColorDoor"); }
        }


        /// <summary>
        /// COMMANDS
        /// </summary>

        //Leitet den User zu Schritt 2
        public RelayCommand ButtonForwardChooseCustomer { get; set; }
        //Leitet den User zu CreateCustomerPage
        public RelayCommand NewCustomerButton { get; set; }
        //Leitet den User zu Schritt 3
        public RelayCommand ButtonForwardChooseHouse { get; set; }
        //Leitet den User zu Schritt 4
        public RelayCommand ButtonForwardChoosePlot { get; set; }
        //Leitet den User zu Schritt 5 -> Wand
        public RelayCommand ButtonForwardChooseWall { get; set; }
        //Leitet den User zu Schritt 6 -> Dach
        public RelayCommand ButtonForwardChooseRoof { get; set; }
        //Leitet den User zu Schritt 7 -> Fenster und Türen
        public RelayCommand ButtonForwardChooseWindowsDoors { get; set; }

        public MainViewModel()
        {
            InitializeButtons();

            customers.Add(new Customer("Max", "Mustermann", 0, 0));
            customers.Add(new Customer("Danielo", "Pizzamento", 1, 2));
            customers.Add(new Customer("Fritz", "Immertoll", 1, 4));

            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1111, 107000));
            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1222, 150000));
            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1333, 210000));

            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck1.png", 2111, 80000));
            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck2.png", 2222, 90000));
            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck3.png", 2333, 99000));

            NumberFloors.Add(0);
            NumberFloors.Add(1);
            NumberFloors.Add(2);

            

        }

        //Hier werden alle Buttons initialisiert
        private void InitializeButtons()
        {
            ButtonForwardChooseCustomer = new RelayCommand(ButtonForwardChooseCustomerClicked);
            NewCustomerButton = new RelayCommand(NewCustomerButtonMethod);
            ButtonForwardChooseHouse = new RelayCommand(ButtonForwardChooseHouseMethod);
            ButtonForwardChoosePlot = new RelayCommand(ButtonForwardChoosePlotMethod);
            ButtonForwardChooseWall = new RelayCommand(ButtonForwardChooseWallMethod);
            ButtonForwardChooseRoof = new RelayCommand(ButtonForwardChooseRoofMethod);
            ButtonForwardChooseWindowsDoors = new RelayCommand(ButtonForwardChooseWindowsDoorsMethod);


        }

        //Hier wird im Schritt 2 das Haus ausgewählt und auf Weiter geklickt
        private async void ButtonForwardChooseHouseMethod()
        {
            if (SelectedHouse != null)
            {
                //var dialog = new MessageDialog("Id Haus: " + SelectedHouse.Image1.Name);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt3GrundstückAuswahl));
                TotalPrice = SelectedHouse.Price;
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Haus aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird im Schritt 3 das Grundstück ausgewählt und auf Weiter geklickt
        private async void ButtonForwardChoosePlotMethod()
        {
            if (SelectedHouse != null)
            {
                //var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt4Grundriss));
                if(SelectedPlot != null)
                TotalPrice += SelectedPlot.Price;
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Grundstück aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird weitergeleitet auf Schritt 5
        private void ButtonForwardChooseWallMethod()
        {
                //var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt5Wand));
            if (selectedItemFloor != NumberOfFloorDB) TotalPrice += SelectedFloor.Price;

            //außenwände befüllen
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Holzwand", 1000));
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1234, "Ziegel", 2000));
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1234, "Beton", 3000));

            //farben der außenwände festlegen
            ListColorOutsideWall.Add(new ColorPalette(Colors.Red));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Blue));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Yellow));
            ListColorOutsideWall.Add(new ColorPalette(Colors.GreenYellow));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Khaki));


            //innenwände wrden befüllt
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck1.png", 1234, "Holzwand", 1000));
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck2.png", 1234, "Rigipswand", 2000));
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck3.png", 1234, "Schallschutzsteinwand", 3000));

            //farben für die Innenwände festlegen
            ListColorInsideWall.Add(new ColorPalette(Colors.Green));
            ListColorInsideWall.Add(new ColorPalette(Colors.White));
            ListColorInsideWall.Add(new ColorPalette(Colors.Red));
            ListColorInsideWall.Add(new ColorPalette(Colors.Yellow));
            ListColorInsideWall.Add(new ColorPalette(Colors.Blue));
        }

        //Hier wird weitergeleitet auf Schritt 6 
        private async void ButtonForwardChooseRoofMethod()
        {
            
            if (SelectedInsideWall != null && selectedOutsideWall != null)
            {
                TotalPrice += selectedOutsideWall.Price + selectedInsideWall.Price;
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt6Dach));

                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Pultdach", 2500));
                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1111, "Satteldach", 3000));
                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 121234, "Flachdach", 1000));


                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Ziegel", 50));
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1111, "Beton", 18));
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 121234, "Aluminium", 90));
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 121234, "Faserzementplatten", 30));

            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie den Typen der Außen- und Innenwand aus.");
                await dialog.ShowAsync();
            }

        }

        //Hier wird weitergeleitet auf Schritt 7
        private async void ButtonForwardChooseWindowsDoorsMethod()
        {
            if(selectedRoofType != null && SelectedRoofMaterial != null)
            {
                TotalPrice += selectedRoofType.Price + SelectedRoofMaterial.Price;
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt7FensterTueren));

                //Fenster befüllen
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Rund", 100));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1234, "Trapez", 200));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1234, "Einteilig", 300));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Schwing", 100));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1234, "Zweiteilig", 200));

                //farben der Fenster festlegen
                ListColorWindows.Add(new ColorPalette(Colors.Blue));
                ListColorWindows.Add(new ColorPalette(Colors.White));
                ListColorWindows.Add(new ColorPalette(Colors.Black));
                ListColorWindows.Add(new ColorPalette(Colors.Gray));
                ListColorWindows.Add(new ColorPalette(Colors.Brown));


                //https://www.google.at/search?q=t%C3%BCrtypen&espv=2&biw=1234&bih=697&site=webhp&tbm=isch&imgil=4tESwMuVZodcAM%253A%253BYQbax1ae5UEJHM%253Bhttp%25253A%25252F%25252Fschoener-bauen24.de%25252Fzimmertueren%25252Fweisslack-tueren%25252Fcomo%25252Fweisslack-profiltueren-como-ckl1-3.html&source=iu&pf=m&fir=4tESwMuVZodcAM%253A%252CYQbax1ae5UEJHM%252C_&usg=__kky6-adiiiEX9ZKiLXa76VjB9OY%3D&ved=0ahUKEwiqgdySwMrMAhVBXRQKHRsnAOsQyjcIJg&ei=8TMvV-rEG8G6UZvOgNgO#imgdii=meTtrzgnHqxTwM%3A%3BmeTtrzgnHqxTwM%3A%3BW6PTc72DPJYIxM%3A&imgrc=meTtrzgnHqxTwM%3A


                //Türen befüllen
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Schiebetür", 100));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1234, "Villa", 200));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1234, "Funktionstür", 300));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1234, "Glas", 100));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1234, "Holz", 200));

                //Türen ben der Fenster festlegen
                ListColorDoors.Add(new ColorPalette(Colors.Brown));
                ListColorDoors.Add(new ColorPalette(Colors.RosyBrown));
                ListColorDoors.Add(new ColorPalette(Colors.SaddleBrown));
                ListColorDoors.Add(new ColorPalette(Colors.SandyBrown));
                ListColorDoors.Add(new ColorPalette(Colors.White));
                ListColorDoors.Add(new ColorPalette(Colors.Gray));




            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie Fenster und Türen aus.");
                await dialog.ShowAsync();
            }
        }


        //Hier wird die Frame die in MainPage angezeigt wird geladen
        private void GetFrame()
        {
            a = MainPage.FrameObject.GetObject();
        }

        //Hier werden alle StackPanels inilitalisiert
        private void GetStackPanels()
        {
            stackPanelS2 = Pages.HKPages.Schritt2HausAuswahl.StackObjectSchritt2.GetObject();
        }

        //Hier wird der User zur CreateNewCustomer geleitet
        private void NewCustomerButtonMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.KundenPages.CreateCustomerPage));
        }

        //Hier wird angezegt, ob der User einen Customer selected hat oder nicht
        private async void ButtonForwardChooseCustomerClicked()
        {
            if (selectedCustomerr != null)
            {
                //var dialog = new MessageDialog(SelectedCustomerr.Firstname);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt2HausAuswahl));
                GetStackPanels();

                /*
                Image image1 = new Image();
                image1.MaxWidth = 100;
                BitmapImage bitmapImage = new BitmapImage();
                Uri uri = new Uri("ms-appx:///Assets/StoreLogo.png");
                bitmapImage.UriSource = uri;
                image1.Source = bitmapImage;

                Image image2 = new Image();
                image2.MaxWidth = 100;
                BitmapImage bitmapImage2 = new BitmapImage();
                Uri uri2 = new Uri("ms-appx:///Assets/StoreLogo.png");
                bitmapImage2.UriSource = uri2;
                image2.Source = bitmapImage2;
                image2.Name = "id1234223";
                */

                //var uriSource = new Uri(@"/FallstudieMenu;Fallstudie/Assets/StoreLogo.png", UriKind.Relative);
                //stackPanelS2.Children.Add(image1);
                //stackPanelS2.Children.Add(image2);
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Kunden aus.");
                await dialog.ShowAsync();
            }
        }

        //wenn anzahl von stockwerken angeklickt wird -> anzeigen der Grundrisse
        public void ChooseGroundPlots()
        {
            FloorsGroundPlot.Clear();
            //Anzahl der Stockwerke vom ausgewähltem haus aus der DB
            numberOfFloorDB = 1;

            FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck1.png", 1111, "Erdgeschoss", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            if (SelectedItemFloor == 1 || SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck2.png", 1122,  "Stockwerk 1", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }
            if(SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck3.png", 1131, "Stockwerk 2", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }

            

            OnChange("FloorsGroundPlot");
        }
        private void ButtonDrawSketchMethod()
        {
            AsynchMethod();
        }



        /// <summary>
        /// Test Sachen
        /// </summary>
        /// 

        //MessageBox wird angezeigt -> zum Testen
        private async void AsynchMethod()
        {
            var dialog = new MessageDialog("TestMethode wurde aufgerufen");
            await dialog.ShowAsync();
        }

        //Command zum anclicken einer liste
        //http://stackoverflow.com/questions/18257516/how-to-pass-listbox-selecteditem-as-command-parameter-in-a-button
        /*private RelayCommand someComboBoxCommand;
        public RelayCommand SomeComboBoxCommand
        {
            get { return someComboBoxCommand; }
            set { someComboBoxCommand = value; }
        }
        */
        //Initialisierung
        //SomeComboBoxCommand = new RelayCommand(TestMethode);

        //Test Methode
        /*
        private async void TestMethode()
        {
            var dialog = new MessageDialog(selectedCustomerr.Firstname);
            await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.KundenPage));
        }
        */
    }
}
