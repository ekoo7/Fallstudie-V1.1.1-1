﻿using Fallstudie.DBModel;
using Fallstudie.Model;
using GalaSoft.MvvmLight.Command;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.DocIO.DLS;
using Windows.Storage.Pickers;
using Syncfusion.Pdf.Tables;

namespace Fallstudie.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            InitializeButtons();

            //Datenbank erstellen
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            if (!File.Exists(DbPath))
            {
                Conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath);
                SQLCreateTable();
                SQLInsertAttributeGroup();
            }

            Customers.Clear();
            SQLGetCustomers();

            ProjectsMethod();
            ManageAppointments();

            CreatePdf();
        }

        #region UseCase HouseConfig
        #region PROPERTIES

        #region Variablen
        //Höchste configId auslesen
        public Houseconfig configId { get; set; }
        //SQLite pfad variable
        public string DbPath { get; set; }
        //SQLite verbindung
        public SQLiteConnection Conn { get; set; }
        //Button 1-11 wird sichtbar
        private string buttonIsVisible = "Collapsed";

        public string ButtonIsVisible
        {
            get { return buttonIsVisible; }
            set { buttonIsVisible = value; OnChange("ButtonIsVisible"); }
        }

        //Zusammenfassung des konfigurierten Hauses
        private HouseSummary house;

        public HouseSummary House
        {
            get { return house; }
            set { house = value; OnChange("House"); }
        }


        //Der gesammte Preis
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


        //Frame wird initialisiert - damit wir die Frame, wo alle Pages angezeigt werden haben
        Frame a = new Frame();

        //StackPanel von Schritt2
        StackPanel stackPanelS2 = new StackPanel();
        #endregion

        #region Listen
        //Customer die in der Liste gespeichert sind
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get { return customers; }
            set { customers = value; OnChange("Items"); }
        }


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
            set { numberFloors = value; }
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

        //Liste für die Energiesysteme
        private ObservableCollection<EHSystem> listEnergySystem = new ObservableCollection<EHSystem>();

        public ObservableCollection<EHSystem> ListEnergySystem
        {
            get { return listEnergySystem; }
            set { listEnergySystem = value; }
        }

        //Liste für die Heizungssysteme
        private ObservableCollection<EHSystem> listHeatingSystem = new ObservableCollection<EHSystem>();

        public ObservableCollection<EHSystem> ListHeatingSystem
        {
            get { return listHeatingSystem; }
            set { listHeatingSystem = value; }
        }

        //Liste für Anzahl der Steckdosen
        private ObservableCollection<int> numberSockets = new ObservableCollection<int>();

        public ObservableCollection<int> NumberSockets
        {
            get { return numberSockets; }
            set { numberSockets = value; }
        }

        //Liste für die Kamine
        private ObservableCollection<ImageInherit> listChimneys = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListChimneys
        {
            get { return listChimneys; }
            set { listChimneys = value; }
        }

        //Liste für die Quadratmeter der Pools
        private ObservableCollection<int> numberPoolSizes = new ObservableCollection<int>();

        public ObservableCollection<int> NumberPoolSizes
        {
            get { return numberPoolSizes; }
            set { numberPoolSizes = value; }
        }

        //Liste der SwimmingPools
        private ObservableCollection<ImageInherit> listPools = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListPools
        {
            get { return listPools; }
            set { listPools = value; }
        }

        //Liste der Zäune
        private ObservableCollection<ImageInherit> listFence = new ObservableCollection<ImageInherit>();

        public ObservableCollection<ImageInherit> ListFence
        {
            get { return listFence; }
            set { listFence = value; }
        }

        //Liste für die Farben der Zäune
        private ObservableCollection<ColorPalette> listColorFence = new ObservableCollection<ColorPalette>();

        public ObservableCollection<ColorPalette> ListColorFence
        {
            get { return listColorFence; }
            set { listColorFence = value; }
        }



        #endregion

        #region Selected Items

        // der ausgewählte Kunde in der Kundenliste
        private Customer selectedCustomerr;
        public Customer SelectedCustomerr
        {
            get { return selectedCustomerr; }
            set { selectedCustomerr = value; OnChange("SelectedCustomerr"); }
        }

        //selected Anzahl der Stockwerk
        private int selectedItemFloor;
        public int SelectedItemFloor
        {
            get { return selectedItemFloor; }
            set
            {
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

        //selected Energiesystem
        private EHSystem selectedEnergySystem;
        public EHSystem SelectedEnergySystem
        {
            get { return selectedEnergySystem; }
            set { selectedEnergySystem = value; OnChange("SelectedEnergySystem"); }
        }

        //selected Heizungssystem
        private EHSystem selectedHeatingSystem;
        public EHSystem SelectedHeatingSystem
        {
            get { return selectedHeatingSystem; }
            set { selectedHeatingSystem = value; OnChange("SelectedHeatingSystem"); }
        }

        //selected Anzahl der Steckdosen
        private int selectedSocket;
        public int SelectedSocket
        {
            get { return selectedSocket; }
            set { selectedSocket = value; OnChange("SelectedSocket"); }
        }

        //selected Kamin
        private ImageInherit selectedChimney;
        public ImageInherit SelectedChimney
        {
            get { return selectedChimney; }
            set { selectedChimney = value; OnChange("SelectedChimney"); }
        }

        //selected Größen für die Pools
        private int selectedPoolSize;
        public int SelectedPoolSize
        {
            get { return selectedPoolSize; }
            set { selectedPoolSize = value; OnChange("SelectedPoolSize"); }
        }

        //selected Pool
        private ImageInherit selectedPool;

        public ImageInherit SelectedPool
        {
            get { return selectedPool; }
            set { selectedPool = value; OnChange("SelectedPool"); }
        }

        //selected Zaun
        private ImageInherit selectedFence;
        public ImageInherit SelectedFence
        {
            get { return selectedFence; }
            set { selectedFence = value; OnChange("SelectedFence"); }
        }

        //selected Farbe Zaun
        private ColorPalette selectedColorFence;
        public ColorPalette SelectedColorFence
        {
            get { return selectedColorFence; }
            set { selectedColorFence = value; OnChange("SelectedColorFence"); }
        }


        #endregion

        #region Commands
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
        //Leitet den User zu Schritt 8 -> Energy und Heizung
        public RelayCommand ButtonForwardChooseEnergie { get; set; }
        //Leitet den User zu Schritt 9 -> ElektroInstallaiton und Kamin
        public RelayCommand ButtonForwardChooseAddition { get; set; }
        //Leitet den User zu Schritt 10 -> Außenbereich definieren
        public RelayCommand ButtonForwardChooseOutsideArea { get; set; }
        //Leitet den User zu Schritt 11 -> Zusammenfassung
        public RelayCommand ButtonForwardSummary { get; set; }
        //Leitet den User zu Schritt 2 zurück -> HausAuswahl
        public RelayCommand ButtonEditConfiguration { get; set; }
        //Speichert das konfigurierte Haus in der DB
        public RelayCommand ButtonSaveConfiguration { get; set; }
        //Das konfigurierte Haus wird in ndie Datenbank gespeichert -> Projekt wurde erstellt
        public RelayCommand ButtonCreateProject { get; set; }
        #endregion

        #region Notizen
        private string noteStep2;

        public string NoteStep2
        {
            get { return noteStep2; }
            set { noteStep2 = value; OnChange("NoteStep2"); }
        }
        private string noteStep3;

        public string NoteStep3
        {
            get { return noteStep3; }
            set { noteStep3 = value; OnChange("NoteStep3"); }
        }

        private string noteStep4;

        public string NoteStep4
        {
            get { return noteStep4; }
            set { noteStep4 = value; OnChange("NoteStep4"); }
        }

        private string noteStep5;

        public string NoteStep5
        {
            get { return noteStep5; }
            set { noteStep5 = value; OnChange("NoteStep5"); }
        }

        private string noteStep6;

        public string NoteStep6
        {
            get { return noteStep6; }
            set { noteStep6 = value; OnChange("NoteStep6"); }
        }

        private string noteStep7;

        public string NoteStep7
        {
            get { return noteStep7; }
            set { noteStep7 = value; OnChange("NoteStep7"); }
        }

        private string noteStep8;

        public string NoteStep8
        {
            get { return noteStep8; }
            set { noteStep8 = value; OnChange("NoteStep8"); }
        }

        private string noteStep9;

        public string NoteStep9
        {
            get { return noteStep9; }
            set { noteStep9 = value; OnChange("NoteStep9"); }
        }

        private string noteStep10;

        public string NoteStep10
        {
            get { return noteStep10; }
            set { noteStep10 = value; OnChange("NoteStep10"); }
        }
        #endregion


        #endregion


        //Hier werden alle Buttons initialisiert
        private void InitializeButtons()
        {
            ButtonForwardChooseCustomer = new RelayCommand(ButtonForwardChooseCustomerMethod);
            NewCustomerButton = new RelayCommand(NewCustomerButtonMethod);
            ButtonForwardChooseHouse = new RelayCommand(ButtonForwardChooseHouseMethod);
            ButtonForwardChoosePlot = new RelayCommand(ButtonForwardChoosePlotMethod);
            ButtonForwardChooseWall = new RelayCommand(ButtonForwardChooseWallMethod);
            ButtonForwardChooseRoof = new RelayCommand(ButtonForwardChooseRoofMethod);
            ButtonForwardChooseWindowsDoors = new RelayCommand(ButtonForwardChooseWindowsDoorsMethod);
            ButtonForwardChooseEnergie = new RelayCommand(ButtonForwardChooseEnergieMethod);
            ButtonForwardChooseAddition = new RelayCommand(ButtonForwardChooseAdditionMethod);
            ButtonForwardChooseOutsideArea = new RelayCommand(ButtonForwardChooseOutsideAreaMethod);
            ButtonForwardSummary = new RelayCommand(ButtonForwardSummaryMethod);
            ButtonEditConfiguration = new RelayCommand(ButtonEditConfigurationMehtod);
            ButtonSaveConfiguration = new RelayCommand(ButtonSaveConfigurationMethod);
            ButtonCreateProject = new RelayCommand(ButtonCreateProjectMethod);
        }


        #region ForwardButtons

        //Hier wird weitergeleitet auf Schritt 2
        private async void ButtonForwardChooseCustomerMethod()
        {
            if (selectedCustomerr != null)
            {
                //var dialog = new MessageDialog(SelectedCustomerr.Firstname);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt2HausAuswahl));
                GetStackPanels();

                ImagesHouse.Clear();

                List<Houses> models = SQLGetHouses();
                foreach (var item in models)
                {
                    ImagesHouse.Add(new ImageInherit(item.Source, item.PackageId, item.Description, item.Price, item.Company, item.ZIP, item.City, item.Street, item.HouseNo, item.Country));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Kunden aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird weitergeleitet auf Schritt 3
        private async void ButtonForwardChooseHouseMethod()
        {
            if (SelectedHouse != null)
            {
                //var dialog = new MessageDialog("Id Haus: " + SelectedHouse.Image1.Name);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt3GrundstückAuswahl));
                TotalPrice = SelectedHouse.Price;

                ImagesPlot.Clear();

                //Grundstücke aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(3);

                //Grundstückbilder werden angezeigt
                foreach (var item in models)
                {
                    ImagesPlot.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }

            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Haus aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird weitergeleitet auf Schritt 4
        private async void ButtonForwardChoosePlotMethod()
        {
            if (SelectedHouse != null)
            {
                //var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
                //await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt4Grundriss));
                if (SelectedPlot != null)
                    TotalPrice += SelectedPlot.Price;

                NumberFloors.Clear();

                //Dropdown Stockwerke befüllen
                NumberFloors.Add(0);
                NumberFloors.Add(1);
                NumberFloors.Add(2);
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Grundstück aus.");
                await dialog.ShowAsync();
            }
        }

        //wenn anzahl von stockwerken angeklickt wird -> anzeigen der Grundrisse
        public void ChooseGroundPlots()
        {
            FloorsGroundPlot.Clear();
            //Anzahl der Stockwerke vom ausgewähltem haus aus der DB
            numberOfFloorDB = 1;

            FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundriss/GrundrissErdgeschoss.png", 1111, "Erdgeschoss", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            if (SelectedItemFloor == 1 || SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundriss/GrundrissStock1.png", 1122, "Stockwerk 1", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }
            if (SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundriss/GrundrissStock2.png", 1131, "Stockwerk 2", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }
            OnChange("FloorsGroundPlot");
        }

        //Hier wird weitergeleitet auf Schritt 5
        private void ButtonForwardChooseWallMethod()
        {
            //var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
            //await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt5Wand));
            if (selectedItemFloor != NumberOfFloorDB) TotalPrice += SelectedFloor.Price;

            //ListOutsideWall.Clear();
            ListColorOutsideWall.Clear();
            ListInsideWall.Clear();
            ListColorInsideWall.Clear();

            //Außenwände aus der Datenbank selecten
            List<DBModel.Attribute> models = SQLGetAttribute(5);

            //Außenwände werden angezeigt
            foreach (var item in models)
            {
                ListOutsideWall.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
            }

            //Farben Außenwände aus der Datenbank selecten
            List<DBModel.Attribute> modelsC = SQLGetAttribute(902);

            //Farben Außenwände werden angezeigt
            foreach (var item in modelsC)
            {
                string[] color = item.description.Split(',');
                byte r = Byte.Parse(color[0]);
                byte g = Byte.Parse(color[1]);
                byte b = Byte.Parse(color[2]);
                ListColorOutsideWall.Add(new ColorPalette(r, g, b));
            }

            //Innenwände aus der Datenbank selecten
            List<DBModel.Attribute> models2 = SQLGetAttribute(51);

            //Innenwände werden angezeigt
            foreach (var item in models2)
            {
                ListInsideWall.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
            }

            //Farben Innenwände aus der Datenbank selecten
            List<DBModel.Attribute> models2C = SQLGetAttribute(901);

            //Farben Innenwände werden angezeigt
            foreach (var item in models2C)
            {
                string[] color = item.description.Split(',');
                byte r = Byte.Parse(color[0]);
                byte g = Byte.Parse(color[1]);
                byte b = Byte.Parse(color[2]);
                ListColorInsideWall.Add(new ColorPalette(r, g, b));
            }

        }

        //Hier wird weitergeleitet auf Schritt 6 
        private async void ButtonForwardChooseRoofMethod()
        {
            if (SelectedInsideWall != null && selectedOutsideWall != null)
            {
                TotalPrice += selectedOutsideWall.Price + selectedInsideWall.Price;
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt6Dach));

                ListRoofType.Clear();
                ListRoofMaterial.Clear();

                //Dachtypen aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(6);

                //Dachtypen werden angezeigt
                foreach (var item in models)
                {
                    ListRoofType.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }

                //Dachmaterial aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(61);

                //Dachmaterial werden angezeigt
                foreach (var item in models2)
                {
                    ListRoofMaterial.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }
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
            if (selectedRoofType != null && SelectedRoofMaterial != null)
            {
                TotalPrice += selectedRoofType.Price + SelectedRoofMaterial.Price;
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt7FensterTueren));

                ListWindows.Clear();
                ListColorWindows.Clear();
                ListDoors.Clear();
                ListColorDoors.Clear();

                //Fenster aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(7);

                //Fenster werden angezeigt
                foreach (var item in models)
                {
                    ListWindows.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }

                //Farben Fenster aus der Datenbank selecten
                List<DBModel.Attribute> modelsC = SQLGetAttribute(903);

                //Farben Fenster werden angezeigt
                foreach (var item in modelsC)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorWindows.Add(new ColorPalette(r, g, b));
                }

                //Türen aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(71);

                //Türen werden angezeigt
                foreach (var item in models2)
                {
                    ListDoors.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }

                //Farben Türen aus der Datenbank selecten
                List<DBModel.Attribute> models2C = SQLGetAttribute(904);

                //Farben Türen werden angezeigt
                foreach (var item in models2C)
                {
                    string[] color = item.description.Split(',');
                    byte r = Byte.Parse(color[0]);
                    byte g = Byte.Parse(color[1]);
                    byte b = Byte.Parse(color[2]);
                    ListColorDoors.Add(new ColorPalette(r, g, b));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Dachtyp und ein Dachmaterial aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zum Schritt 8 weitergeleitet
        private async void ButtonForwardChooseEnergieMethod()
        {

            if (SelectedWindow != null && SelectedDoor != null)
            {
                ListEnergySystem.Clear();
                ListHeatingSystem.Clear();

                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt8EnergyHeizung));

                //Energiesystem aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(8);

                //Energiesystem werden angezeigt
                foreach (var item in models)
                {
                    ListEnergySystem.Add(new EHSystem(item.attribute_id, item.description, item.price));
                }

                //Heizungsystem aus der Datenbank selecten
                List<DBModel.Attribute> models2 = SQLGetAttribute(81);

                //Heizungsystem werden angezeigt
                foreach (var item in models2)
                {
                    ListHeatingSystem.Add(new EHSystem(item.attribute_id, item.description, item.price));
                }

                totalPrice += SelectedWindow.Price + SelectedDoor.Price;
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie Fenster und Türen aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier wird zu Schritt 9 weiterleitet
        private async void ButtonForwardChooseAdditionMethod()
        {
            if (SelectedEnergySystem != null && selectedHeatingSystem != null)
            {
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt9Zusatz));
                totalPrice += SelectedEnergySystem.Price + selectedHeatingSystem.Price;

                NumberSockets.Clear();
                ListChimneys.Clear();

                //Steckdosen aus der Datenbank selecten
                List<DBModel.Attribute> modelsS = SQLGetAttribute(12);

                //Steckdosen werden angezeigt
                foreach (var item in modelsS)
                {
                    NumberSockets.Add(int.Parse(item.description));
                }

                //Kamin aus der Datenbank selecten
                List<DBModel.Attribute> models = SQLGetAttribute(9);

                //Kamin werden angezeigt
                foreach (var item in models)
                {
                    ListChimneys.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Energie und ein Heizungssystem aus.");
                await dialog.ShowAsync();
            }
        }


        //Hier wird zu Schritt 10 weitergeleitet
        private void ButtonForwardChooseOutsideAreaMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt10Aussenbereiche));

            NumberPoolSizes.Clear();
            ListPools.Clear();
            ListFence.Clear();
            ListColorFence.Clear();

            //Poolgrößen aus der Datenbank selecten
            List<DBModel.Attribute> modelsP = SQLGetAttribute(11);

            //Poolgrößen werden angezeigt
            foreach (var item in modelsP)
            {
                NumberPoolSizes.Add(int.Parse(item.description));
            }

            //Pools aus der Datenbank selecten
            List<DBModel.Attribute> models = SQLGetAttribute(10);

            //Pools werden angezeigt
            foreach (var item in models)
            {
                ListPools.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
            }

            //Zaun aus der Datenbank selecten
            List<DBModel.Attribute> models2 = SQLGetAttribute(101);

            //Zaun werden angezeigt
            foreach (var item in models2)
            {
                ListFence.Add(new ImageInherit(item.image, item.attribute_id, item.description, item.price));
            }

            //Farben Zaun aus der Datenbank selecten
            List<DBModel.Attribute> modelsC = SQLGetAttribute(904);

            //Farben Zaun werden angezeigt
            foreach (var item in modelsC)
            {
                string[] color = item.description.Split(',');
                byte r = Byte.Parse(color[0]);
                byte g = Byte.Parse(color[1]);
                byte b = Byte.Parse(color[2]);
                ListColorFence.Add(new ColorPalette(r, g, b));
            }

            try
            {
                if (selectedChimney != null)
                    TotalPrice += selectedChimney.Price;
            }
            catch (Exception)
            {

            }
        }

        //Hier wird zu Schritt 11 weitergeleitet
        private void ButtonForwardSummaryMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schrtitt11Zusammenfassung));

            House = new HouseSummary()
            {
                Customer = selectedCustomerr,
                Package = SelectedHouse,
                Plot = SelectedPlot,
                numberOfFloors = SelectedItemFloor,
                GroundPlots = FloorsGroundPlot.ToList(),
                OutsideWall = selectedOutsideWall,
                OutsideWallColor = SelectedColorOutsideWall,
                InsideWall = SelectedInsideWall,
                InsideWallColor = selectedColorInsideWall,
                RoofType = SelectedRoofType,
                RoofMaterial = SelectedRoofMaterial,
                Window = SelectedWindow,
                WindowColor = SelectedColorWindow,
                Door = SelectedDoor,
                DoorColor = selectedColorDoor,
                EnergySystem = SelectedEnergySystem,
                HeatingSystem = selectedHeatingSystem,
                NumberOfSocket = SelectedSocket,
                Chimney = SelectedChimney,
                Pool = selectedPool,
                Poolsize = SelectedPoolSize.ToString() + " m²",
                Fence = selectedFence,
                FenceColor = selectedColorFence
            };
            ButtonIsVisible = "Visible";
        }

        //Hier wird das Haus in die Datenbank gespeichert -> Projekt erstellen
        private async void ButtonCreateProjectMethod()
        {
            //SQLCreateProject();
            var dialog = new MessageDialog("Haus wurde als Projekt erstellt");
            await dialog.ShowAsync();
        }

        //Hier wird weitergeleitet auf Schritt 2 -> Konfiguration bearbeiten
        private void ButtonEditConfigurationMehtod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt2HausAuswahl));
            GetStackPanels();
        }
        //Hier wird die Hauskonfiguration in der DB gespeichert
        private async void ButtonSaveConfigurationMethod()
        {
            throw new NotImplementedException();

            var dialog = new MessageDialog("Haus wurde als konfiguriertes Haus erstellt.");
            await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.pdfErstellenPage));

        }
        #endregion


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


        private void ButtonDrawSketchMethod()
        {
            AsynchMethod();
        }


        #region SQL Befehle

        public void SQLCreateTable()
        {
            Conn.CreateTable<DBModel.Attribute>();
            Conn.CreateTable<Attribute_Group>();
            Conn.CreateTable<Houseconfig>();
            Conn.CreateTable<Houseconfig_Has_Attribute>();
            Conn.CreateTable<Housefloor>();
            Conn.CreateTable<Mdh_User_Usergroup_Map>();
            Conn.CreateTable<Mdh_Usergroups>();
            Conn.CreateTable<Mdh_Users>();
            Conn.CreateTable<Package_Not_Attribute>();
            Conn.CreateTable<Project>();
            Conn.CreateTable<Ymdh_Address>();
            Conn.CreateTable<Ymdh_Appointment>();
            Conn.CreateTable<Ymdh_Appointment_Status>();
            Conn.CreateTable<Ymdh_House_Package>();
            Conn.CreateTable<Ymdh_House_Package_Status>();
            Conn.CreateTable<Ymdh_Message>();
            Conn.CreateTable<Ymdh_Person>();
            Conn.CreateTable<Ymdh_Producer>();
        }

        public void SQLGetCustomers()
        {
            Customers.Clear();
            List<Mdh_Users> model;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                model = (from a in con.Table<Mdh_Users>()
                         from b in con.Table<Mdh_User_Usergroup_Map>()
                         from c in con.Table<Mdh_Usergroups>()
                         where a.id.Equals(b.user_id)
                         && b.group_id.Equals(c.id)
                         && c.title.Equals("Customer")
                         select a).ToList();

                con.Close();
            }
            for (int i = 0; i < model.Count; i++)
            {
                Customers.Add(new Customer(model[i].id, model[i].name, 0, 0));
            }
        }

        //Attribut Gruppen werden erstellt        
        public void SQLInsertAttributeGroup()
        {
            //Conn.Execute("PRAGMA foreign_keys = '1';");
            //Attribut Grundstück
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 3,
                description = "Grundstück",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Grundriss
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 4,
                description = "Grundriss",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Außenwand
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 5,
                description = "Außenwand",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Dachtyp
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 6,
                description = "Dachtyp",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Fenster
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 7,
                description = "Fenster",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Energiesysteme
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 8,
                description = "Energiesysteme",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Zusatz (Kamin)
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 9,
                description = "Kamin",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Außenbereiche (Pool)
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 10,
                description = "Pool",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Innenwand
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 51,
                description = "Innenwand",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Dachmaterial
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 61,
                description = "Dachmaterial",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Türen
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 71,
                description = "Türen",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Heizungsysteme
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 81,
                description = "Heizungsysteme",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Außenbereiche (Zaun)
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 101,
                description = "Zaun",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Poolgröße
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 11,
                description = "Poolgröße",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            //Attribut Steckdosen
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 12,
                description = "Steckdosen",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
        }

        //Projekt wird erstellt
        public void SQLCreateHouseconfig()
        {
            //Houseconfig configId;
            Conn.Insert(new Houseconfig
            {
                price = int.Parse(TotalPrice.ToString()),
                status = "1",
                price_floor = int.Parse(SelectedFloor.Price.ToString()),
                modifieddate = ConvertDateTime(DateTime.Now),
                house_package_id = SelectedHouse.Id,
                consultant_user_id = 1,
                customer_user_id = SelectedCustomerr.Id
            });
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                configId = (con.Table<Houseconfig>().OrderByDescending(u => u.houseconfig_id).FirstOrDefault());
                con.Close();
            }
            for (int i = 0; i < SelectedItemFloor; i++)
            {
                Conn.Insert(new Housefloor
                {
                    price = int.Parse(SelectedFloor.Price.ToString()),
                    sketch = FloorsGroundPlot[i].SourceImage,
                    modifieddate = ConvertDateTime(DateTime.Now),
                    houseconfig_id = configId.houseconfig_id,
                    area = i
                });
            }
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedPlot.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedInsideWall.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedOutsideWall.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedRoofType.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedRoofMaterial.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedDoor.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedWindow.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedEnergySystem.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedHeatingSystem.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedPool.Id,
                amount = 1,
                special = SelectedPoolSize.ToString(),
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedFence.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedFence.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = configId.houseconfig_id,
                attribute_id = SelectedFence.Id,
                amount = 1,
                special = "",
                modifieddate = ConvertDateTime(DateTime.Now)
            });
        }
        public void SQLCreateProject()
        {
            DateTime d = DateTime.Now;
            Conn.Insert(new Project
            {
                startdate = ConvertDateTime(DateTime.Now),
                enddate = ConvertDateTime(d.AddYears(1)),
                invoice = "Rechnung 1",
                status = "0",
                description = "",
                modifieddate = ConvertDateTime(DateTime.Now),
            });
        }

        //Get Attribute aus der Joomla/Frontend Datenbank
        public List<DBModel.Attribute> SQLGetAttribute(int t)
        {
            List<DBModel.Attribute> models;

            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                models = (from c in con.Table<DBModel.Attribute>()
                          where c.attribute_group_id.Equals(t)
                          select c).ToList();

                con.Close();
            }
            return models;
        }

        public List<Houses> SQLGetHouses()
        {
            List<Houses> house;
            List<Ymdh_House_Package> package;
            List<Ymdh_Producer> producer;
            List<Ymdh_Address> address;

            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                house = (from a in con.Table<Ymdh_House_Package>()
                         from b in con.Table<Ymdh_Producer>()
                         from c in con.Table<Ymdh_Address>()
                         where a.producer_id.Equals(b.mdh_producer_id)
                         && a.address_id.Equals(c.mdh_address_id)
                         select new Houses
                         {
                             PackageId = a.house_package_id,
                             Source = a.image,
                             Description = a.description,
                             Price = a.price,
                             Company = b.company,
                             ZIP = c.ZIP,
                             City = c.City,
                             Street = c.Street,
                             HouseNo = c.houseno,
                             Country = c.country
                         }).ToList();

                package = (from b in con.Table<Ymdh_House_Package>()
                           select b).ToList();

                producer = (from c in con.Table<Ymdh_Producer>()
                            select c).ToList();

                address = (from c in con.Table<Ymdh_Address>()
                           select c).ToList();


                /*string comp = "";
                string zip = "";
                string city = "";
                string street = "";
                string houseNo = "";
                string country = "";
                for (int i = 0; i < package.Count; i++)
                {

                        foreach (var item in producer)
                        {
                            if (package[i].producer_id == item.mdh_producer_id)
                            {
                                comp = item.company;
                            }
                        }
                        foreach (var item in address)
                        {
                            if (package[i].address_id == item.mdh_address_id)
                            {
                                zip = item.ZIP;
                                city = item.City;
                                street = item.Street;
                                houseNo = item.houseno;
                                country = item.country;
                            }
                        }

                    house[i] = new Houses()
                    {
                        PackageId = package[i].house_package_id,
                        Source = package[i].image,
                        Description = package[i].description,
                        Price = package[i].price,
                        Company = comp,
                        ZIP = zip,
                        City = city,
                        Street = street,
                        HouseNo = houseNo,
                        Country = country
                    };
                }
                */
                con.Close();
            }
            return house;
        }

        public string ConvertDateTime(DateTime dt)
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
        }


        #endregion



        #endregion

        #region UseCaseProjects
        //Liste vo Alle Porjecte Geladen werden
        #region Properties
        private ObservableCollection<Projects> listProjects = new ObservableCollection<Projects>();
        public ObservableCollection<Projects> ListProjects
        {
            get { return listProjects; }
            set { listProjects = value; }
        }

        //selected Project
        private Projects selectedProject;
        public Projects SelectedProject
        {
            get { return selectedProject; }
            set { selectedProject = value; OnChange("SelectedProject"); }
        }
        #endregion

        #region Methods
        public void ProjectsMethod()
        {


            foreach (var item in SQLGetProject())
            {
                ListProjects.Add(new Projects()
                {
                    Id = item.project_id,
                    StartDate = ConvertDateTime(DateTime.Parse(item.startdate)),
                    EndDate = ConvertDateTime(DateTime.Parse(item.enddate)),
                    State = item.status,
                    Description = item.description,
                    House = SQLGetHouseconfig(item.project_id)
                });
            }
            
        }
        #endregion

        #region SQL

        public List<Project> SQLGetProject()
        {
            List<Project> p;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                p = (from c in con.Table<Project>()
                     select c).ToList();

                con.Close();
            }
            return p;
        }

        #endregion

        #endregion

        #region UseCase ManageAppointment
        #region Properties

        //Datum auswählen 
        private DateTime dateAppointment;

        public DateTime DateAppointment
        {
            get { return dateAppointment; }
            set { dateAppointment = value; OnChange("DateAppointment"); }
        }

        //Zeit auswählen
        private TimeSpan timeAppoitment;

        public TimeSpan TimeAppoitment
        {
            get { return timeAppoitment; }
            set { timeAppoitment = value; OnChange("TimeAppoitment"); }
        }

        //MinYear
        private DateTime dateTimeNow = DateTime.Now;

        public DateTime DateTimeNow
        {
            get { return dateTimeNow; }
            set { dateTimeNow = value; OnChange("DateTimeNow"); }
        }


        private ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();

        public ObservableCollection<Appointment> Appointments
        {
            get { return appointments; }
            set { appointments = value; }
        }

        private ObservableCollection<Consultant> consultants = new ObservableCollection<Consultant>();

        public ObservableCollection<Consultant> Consultants
        {
            get { return consultants; }
            set { consultants = value; }
        }


        //Selected
        private Appointment selectedAppointment;

        public Appointment SelectedAppointment
        {
            get { return selectedAppointment; }
            set { selectedAppointment = value; OnChange("SelectedAppointment"); }
        }


        private Consultant selectedConsultant;

        public Consultant SelectedConsultant
        {
            get { return selectedConsultant; }
            set { selectedConsultant = value; OnChange("SelectedConsultant"); }
        }


        public RelayCommand AddNewAppointmentButton { get; set; }
        public RelayCommand ButtonBackToAppointmentPage { get; set; }
        public RelayCommand ButtonSaveAppointment { get; set; }
        public RelayCommand EditAppointmentButton { get; set; }
        public RelayCommand ButtonSaveEditedAppointment { get; set; }
        public RelayCommand DeleteAppointmentButton { get; set; }

        #endregion

        #region Methods
        //In dieser Methode werden alle Buttons die für den ManageAppointment UseCase gebraucht werden
        //initialisiert
        public void ManageAppointments()
        {
            AddNewAppointmentButton = new RelayCommand(AddNewAppointmentButtonMethod);
            ButtonBackToAppointmentPage = new RelayCommand(ButtonBackToAppointmentPageMethod);
            ButtonSaveAppointment = new RelayCommand(ButtonSaveAppointmentMethod);
            EditAppointmentButton = new RelayCommand(EditAppointmentButtonMethod);
            ButtonSaveEditedAppointment = new RelayCommand(ButtonSaveEditedAppointmentMethod);
            DeleteAppointmentButton = new RelayCommand(DeleteAppointmentButtonMethod);

            LoadAppointments();
        }

        private async void DeleteAppointmentButtonMethod()
        {
            if (SelectedAppointment != null)
            {
                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    var p = (from a in con.Table<Ymdh_Appointment>()
                             where a.appointment_id.Equals(SelectedAppointment.Id)
                             select a).ToList();
                    p[0].appointment_status_id = 0;

                    MessageDialog msgDialog = new MessageDialog("Sind Sie sicher, dass Sie diesen Termin (" + SelectedAppointment.DateFormat + " " + SelectedAppointment.Time +
                        " " + SelectedAppointment.Customer.Name + SelectedAppointment.Consultant.Name + ") löschen wollen?");
                    UICommand yesCmd = new UICommand("Ja");
                    msgDialog.Commands.Add(yesCmd);
                    UICommand noCmd = new UICommand("Nein");
                    msgDialog.Commands.Add(noCmd);
                    IUICommand cmd = await msgDialog.ShowAsync();
                    if (cmd == yesCmd)
                    {
                        //Update funktion
                        con.Update(new Ymdh_Appointment()
                        {
                            appointment_id = p[0].appointment_id,
                            appointment_status_id = p[0].appointment_status_id,
                            consultant_user_id = p[0].consultant_user_id,
                            from_ = p[0].from_,
                            house_package_id = p[0].house_package_id,
                            message_id = p[0].message_id,
                            user_id = p[0].user_id
                        });

                        var dialog = new MessageDialog("Der ausgewählte Termin wurde gelöscht.");
                        await dialog.ShowAsync();
                        Appointments.Clear();
                        LoadAppointments();
                    }

                    con.Close();
                }
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Termin aus.");
                await dialog.ShowAsync();
            }
        }

        //Hier werden alle bereits festgelegten Termine angezeigt
        public void LoadAppointments()
        {
            List<Ymdh_Appointment> model;
            List<string> froms_;
            List<string> customerNamen;
            List<string> consultantNamen;

            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                //holt alle Termine
                model = (from a in con.Table<Ymdh_Appointment>()
                         where a.appointment_status_id.Equals(1)
                         orderby a.from_
                         select a).ToList();

                //holt das Datum + Uhrzeit
                froms_ = (from a in con.Table<Ymdh_Appointment>()
                          where a.appointment_status_id.Equals(1)
                          orderby a.from_
                          select a.from_).ToList();

                //holt die Kundennamen
                customerNamen = (from a in con.Table<Mdh_Users>()
                                 from b in con.Table<Mdh_User_Usergroup_Map>()
                                 from c in con.Table<Ymdh_Appointment>()
                                 from d in con.Table<Mdh_Usergroups>()
                                 where b.group_id.Equals(1)
                                 && a.id.Equals(b.user_id)
                                 && a.id.Equals(c.user_id)
                                 && c.appointment_status_id.Equals(1)
                                 && b.group_id.Equals(d.id)
                                 && d.title.Equals("Customer")
                                 orderby c.from_
                                 select a.name).ToList();

                //holt die consultant namen
                consultantNamen = (from a in con.Table<Mdh_Users>()
                                   from b in con.Table<Mdh_User_Usergroup_Map>()
                                   from c in con.Table<Ymdh_Appointment>()
                                   from d in con.Table<Mdh_Usergroups>()
                                   where a.id.Equals(c.consultant_user_id)
                                   && a.id.Equals(b.user_id)
                                   && b.group_id.Equals(d.id)
                                   && d.title.Equals("Consultant")
                                   && c.appointment_status_id.Equals(1)
                                   orderby c.from_
                                   select a.name).ToList();

                con.Close();
            }

            //füllt die Terminliste und wird angezeigt
            for (int i = 0; i < froms_.Count; i++)
            {
                string[] temp = froms_[i].Split(' ');
                string time = temp[1].Substring(0, 5);
                Appointments.Add(new Appointment()
                {
                    Id = model[i].appointment_id,
                    Date = DateTime.Parse(temp[0]),
                    Time = TimeSpan.Parse(time),
                    Customer = new Customer(model[i].user_id, customerNamen[i], 0, 0),
                    Consultant = new Consultant() { Id = model[i].consultant_user_id, Name = consultantNamen[i] }
                });
            }
        }

        private async void EditAppointmentButtonMethod()
        {
            if (selectedAppointment != null)
            {

                GetFrame();
                a.Navigate(typeof(Pages.TerminePages.TerminBearbeiten));

                SQLGetConsultants();

                SelectedCustomerr = SelectedAppointment.Customer;
                SelectedConsultant = SelectedAppointment.Consultant;
                DateAppointment = SelectedAppointment.Date;
                TimeAppoitment = SelectedAppointment.Time;

            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie einen Termin aus.");
                await dialog.ShowAsync();
            }
        }

        private async void ButtonSaveEditedAppointmentMethod()
        {
            //checken ob Kunde und Consultant ausgewählt wurden
            if (SelectedCustomerr != null && SelectedConsultant != null)
            {
                string pattern = "yyyy-MM-dd HH:mm:ss";
                DateTime choosenAppointment = DateAppointment + TimeAppoitment;

                string dt = choosenAppointment.ToString(pattern, CultureInfo.CurrentUICulture);

                //checken ob der termin für den CONSULTANT belegt ist
                List<Ymdh_Appointment> models;

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    models = (from c in con.Table<Ymdh_Appointment>()
                              where c.consultant_user_id.Equals(SelectedConsultant.Id)
                              && c.from_.Equals(dt)
                              && c.appointment_status_id.Equals(1)
                              select c).ToList();

                    con.Close();
                }
                //checken ob der termin für den KUNDEN belegt ist
                List<Ymdh_Appointment> models2;

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    models2 = (from c in con.Table<Ymdh_Appointment>()
                               where c.user_id.Equals(SelectedCustomerr.Id)
                               && c.from_.Equals(dt)
                               && c.appointment_status_id.Equals(1)
                               select c).ToList();

                    con.Close();
                }
                if (models.Count > 0)
                {
                    var dialog1 = new MessageDialog("Sie haben bereits am " + models[0].from_ + " einen Termin!");
                    await dialog1.ShowAsync();
                }
                else if (models2.Count > 0)
                {
                    var dialog1 = new MessageDialog("Sie haben bereits am " + models2[0].from_ + " einen Termin!");
                    await dialog1.ShowAsync();
                }
                else
                {
                    //in die DB speichern
                    using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                    {
                        con.Update(new Ymdh_Appointment()
                        {
                            appointment_id = selectedAppointment.Id,
                            appointment_status_id = 1,
                            consultant_user_id = SelectedConsultant.Id,
                            from_ = dt,
                            user_id = SelectedCustomerr.Id
                        });

                        con.Close();
                    }
                    var dialog1 = new MessageDialog("Ihr Termin für " + dt + " wurde gespeichert!");
                    await dialog1.ShowAsync();
                    ButtonBackToAppointmentPageMethod();
                }
            }
            else
            {
                var dialog1 = new MessageDialog("Bitte wählen Sie einen Kunden und einen Mitarbeiter aus!");
                await dialog1.ShowAsync();
            }
        }

        private async void ButtonSaveAppointmentMethod()
        {
            //checken ob Kunde und Consultant ausgewählt wurden
            if (SelectedCustomerr != null && SelectedConsultant != null)
            {
                string pattern = "yyyy-MM-dd HH:mm:ss";
                DateTime choosenAppointment = DateAppointment + TimeAppoitment;

                string dt = choosenAppointment.ToString(pattern, CultureInfo.CurrentUICulture);

                //checken ob der termin für den CONSULTANT belegt ist
                List<Ymdh_Appointment> models;

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    models = (from c in con.Table<Ymdh_Appointment>()
                              where c.consultant_user_id.Equals(SelectedConsultant.Id)
                              && c.from_.Equals(dt)
                              && c.appointment_status_id.Equals(1)
                              select c).ToList();

                    con.Close();
                }
                //checken ob der termin für den KUNDEN belegt ist
                List<Ymdh_Appointment> models2;

                using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                {
                    models2 = (from c in con.Table<Ymdh_Appointment>()
                               where c.user_id.Equals(SelectedCustomerr.Id)
                               && c.from_.Equals(dt)
                               && c.appointment_status_id.Equals(1)
                               select c).ToList();

                    con.Close();
                }
                if (models.Count > 0 || models2.Count > 0)
                {
                    var dialog1 = new MessageDialog("Sie haben bereits am " + models[0].from_ + " einen Termin!");
                    await dialog1.ShowAsync();
                }
                else
                {
                    if (DateAppointment.DayOfWeek.ToString() == "Sunday")
                    {
                        var dialog1 = new MessageDialog("Dieser Termin ist ein Sonntag! Bitte wählen Sie ein anderes Datum aus.");
                        await dialog1.ShowAsync();
                    }
                    else if (DateAppointment.DayOfWeek.ToString() == "Saturday")
                    {
                        var dialog1 = new MessageDialog("Dieser Termin ist ein Samstag! Bitte wählen Sie ein anderes Datum aus.");
                        await dialog1.ShowAsync();
                    }
                    else
                    {
                        //in die DB speichern
                        using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
                        {
                            con.Insert(new Ymdh_Appointment()
                            {
                                appointment_status_id = 1,
                                consultant_user_id = SelectedConsultant.Id,
                                from_ = dt,
                                user_id = SelectedCustomerr.Id
                            });

                            con.Close();
                        }
                    }
                }
            }
            else
            {
                var dialog1 = new MessageDialog("Bitte wählen Sie einen Kunden und einen Mitarbeiter aus!");
                await dialog1.ShowAsync();
            }
            /*
            if (DateAppointment.DayOfWeek.ToString() == "Sunday")
            {
               
            }
            */
        }

        private void ButtonBackToAppointmentPageMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.TerminePage));
            Appointments.Clear();
            LoadAppointments();
        }



        //Neuen Termin erstellen
        public void AddNewAppointmentButtonMethod()
        {
            SQLGetConsultants();
            DateAppointment = DateTime.Now;
            TimeAppoitment = DateTime.Now.TimeOfDay;
            GetFrame();
            a.Navigate(typeof(Pages.TerminePages.NeuTermin));
        }

        public void SQLGetConsultants()
        {
            Consultants.Clear();
            List<Mdh_Users> model;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                model = (from a in con.Table<Mdh_Users>()
                         from b in con.Table<Mdh_User_Usergroup_Map>()
                         from c in con.Table<Mdh_Usergroups>()
                         where a.id.Equals(b.user_id)
                         && b.group_id.Equals(c.id)
                         && c.title.Equals("Consultant")
                         select a).ToList();

                con.Close();
            }
            for (int i = 0; i < model.Count; i++)
            {
                Consultants.Add(new Consultant()
                {
                    Id = model[i].id,
                    //Firstname = model[i].name,
                    Name = model[i].name,
                });
            }
        }
        #endregion
        #endregion

        #region UseCase CreatePdf

        #region Properties
        public RelayCommand ButtonCreatePdf { get; set; }
        #endregion

        #region Methods

        public void CreatePdf()
        {
            ButtonCreatePdf = new RelayCommand(ButtonCreatePdfMethod);
        }

        private async void ButtonCreatePdfMethod()
        {

            //Create a new document.
            PdfDocument doc = new PdfDocument();

            //Add a page
            PdfPage page = doc.Pages.Add();
            //Create a solid brush
            PdfBrush brush = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));

            //Set the font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 36);

            //Create a header and draw the image.

            System.Drawing.RectangleF bounds = new System.Drawing.RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
            //PdfImage image = new PdfBitmap(@"Logo.png");

            //header.Graphics.DrawImage(image, new PointF(0, 0), new SizeF(100, 50));
            header.Graphics.DrawString("TestHeader", font, brush, new System.Drawing.PointF(0, 0));
            //Add the header at the top.
            doc.Template.Top = header;


            //Create Pdf graphics for the page
            PdfGraphics g = page.Graphics;
            PdfGraphics g1 = page.Graphics;



            //Draw the text
            g.DrawString("Hello world!", font, brush, new System.Drawing.PointF(20, 20));
            g.DrawString("Hello world!2", font, brush, new System.Drawing.PointF(40, 40));



            SaveImage(page);


            //Create a new PdfGrid.

            PdfGrid pdfGrid = new PdfGrid();

            //Add three columns.

            pdfGrid.Columns.Add(4);

            //Add header.
            pdfGrid.Headers.Add(1);
            PdfGridRow pdfGridHeader = pdfGrid.Headers[0];
            pdfGridHeader.Cells[0].Value = "Spezifikation";
            pdfGridHeader.Cells[1].Value = "Auswahl";
            pdfGridHeader.Cells[2].Value = "Preis";
            pdfGridHeader.Cells[2].Value = "Notitz";
            
            
            //Add rows.
            PdfGridRow pdfGridRow = pdfGrid.Rows.Add();
            for (int i = 0; i < 4; i++)
            {
                pdfGridRow.Cells[i].Value = "E01";
            }
            pdfGridRow.Cells[0].Value = "E01";
            pdfGridRow.Cells[1].Value = "Clay";
            pdfGridRow.Cells[2].Value = "$10,000";


            //Create an instance of PdfGridRowStyle

            PdfGridRowStyle pdfGridRowStyle = new PdfGridRowStyle();

            pdfGridRowStyle.BackgroundBrush = PdfBrushes.LightGray;

            pdfGridRowStyle.Font = new PdfStandardFont(PdfFontFamily.Courier, 10);

            pdfGridRowStyle.TextBrush = PdfBrushes.Blue;

            pdfGridRowStyle.TextPen = PdfPens.Black;

            //Set the height

            pdfGrid.Rows[0].Height = 50;

            //Set style for the PdfGridRow.

            pdfGrid.Rows[0].Style = pdfGridRowStyle;

            //Draw the PdfGrid.
            pdfGrid.Draw(page, 10, 10);


            /*
            //Stream s = new FileStream("C:\\Users\\Ermin\\Dropbox\\GitHub\\Fallstudie-V1.1.1-1\\Fallstudie\\Bilder\\2Haeuser\\Haus1.png", FileMode.Open);

            byte[] byteArray = Encoding.UTF8.GetBytes("C:\\Users\\Ermin\\Dropbox\\GitHub\\Fallstudie-V1.1.1-1\\Fallstudie\\Bilder\\2Haeuser\\Haus1.png");
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream1 = new MemoryStream(byteArray);

            PdfImage image = new PdfBitmap(stream1);
            g1.DrawImage(image, 60, 60);
            */


            MemoryStream stream = new MemoryStream();
            await doc.SaveAsync(stream);
            doc.Close(true);
            Save(stream, "GettingStarted.pdf");
        }

        async void SaveImage(PdfPage page)
        {
            PdfGraphics graphics = page.Graphics;

            Stream imageStream = File.OpenRead("Bilder\\2Haeuser\\Haus1.png");
            //Load the image from the disk.

            PdfBitmap image = new PdfBitmap(imageStream);

            //Draw the image

            graphics.DrawImage(image, 0, 0);
        }

        async void Save(Stream stream, string filename)
        {

            stream.Position = 0;

            StorageFile stFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".pdf";
                savePicker.SuggestedFileName = "Sample";
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                stFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
                st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
                MessageDialog msgDialog = new MessageDialog("Do you want to view the Document?", "File has been created successfully.");
                UICommand yesCmd = new UICommand("Yes");
                msgDialog.Commands.Add(yesCmd);
                UICommand noCmd = new UICommand("No");
                msgDialog.Commands.Add(noCmd);
                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd == yesCmd)
                {
                    // Launch the retrieved file
                    bool success = await Windows.System.Launcher.LaunchFileAsync(stFile);
                }
            }
        }
        #endregion

        #region SQL

        public int SQLCustomerCountProject(int i)
        {
            List<Project> p;
            int h;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                p = (from a in con.Table<Houseconfig>()
                     from b in con.Table<Project>()
                     where a.customer_user_id.Equals(i)
                     && b.houseconfig_id.Equals(a.houseconfig_id)
                     select b).ToList();

                h = p.Count;

                con.Close();
            }
            return h;
        }
        public int SQLCustomerCountHouseconfig(int i)
        {
            List<Houseconfig> p;
            int h;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                p = (from a in con.Table<Houseconfig>()
                     where a.customer_user_id.Equals(i)
                     select a).ToList();

                h = p.Count;

                con.Close();
            }
            return h;
        }
        public DBModel.Attribute SQLGetRightAttribute(int attributGroupId, int userId)
        {
            List<DBModel.Attribute> i;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                i = (from b in con.Table<Houseconfig>()
                     from d in con.Table<Houseconfig_Has_Attribute>()
                     from e in con.Table<DBModel.Attribute>()
                     where b.customer_user_id.Equals(userId)
                     && e.attribute_group_id.Equals(attributGroupId)
                     && d.houseconfig_id.Equals(b.houseconfig_id)
                     && d.attribute_id.Equals(e.attribute_id)
                     select e).ToList();

                con.Close();
            }
            if (i.Count > 0)
                return i[0];
            else
                return new DBModel.Attribute();
        }
        public byte SQLSplitColorR(string i)
        {
            if(i != null)
            {
                string[] color = i.Split(',');
                byte r = Byte.Parse(color[0]);
                return r;
            }else
            {
                return 255;
            }
            
        }
        public byte SQLSplitColorG(string i)
        {
            if (i != null)
            {
                string[] color = i.Split(',');
                byte g = Byte.Parse(color[1]);
                return g;
            }
            else
            {
                return 255;
            }
        }
        public byte SQLSplitColorB(string i)
        {
            if (i != null)
            {
                string[] color = i.Split(',');
                byte b = Byte.Parse(color[2]);
                return b;
            }
            else
            {
                return 255;
            }
        }
        public string SQLGetConsultantName(int id)
        {
            string name;
            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {
                name = (from a in con.Table<Mdh_Users>()
                        where a.id.Equals(id)
                        select a.name).Single();
                con.Close();
            }
            return name;
        }
        public void SQLGetHouseconfig()
        {
            List<HouseSummary> house;
            //List<ImageInherit> Gplots;

            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {

                house = (from a in con.Table<Mdh_Users>()
                         from b in con.Table<Houseconfig>()
                         from c in con.Table<Housefloor>()
                         from d in con.Table<Houseconfig_Has_Attribute>()
                         from e in con.Table<DBModel.Attribute>()
                         from f in con.Table<Ymdh_House_Package>()
                         where a.id.Equals(b.customer_user_id)
                         && b.house_package_id.Equals(f.house_package_id)
                         && c.houseconfig_id.Equals(b.houseconfig_id)
                         && d.houseconfig_id.Equals(b.houseconfig_id)
                         && d.attribute_id.Equals(e.attribute_id)
                         select new HouseSummary
                         {
                             Customer = new Customer(a.id, a.name, SQLCustomerCountProject(a.id), SQLCustomerCountHouseconfig(a.id)),
                             Package = new ImageInherit(f.image, f.house_package_id, f.description, f.price),
                             Plot = new ImageInherit(SQLGetRightAttribute(3, a.id).image, SQLGetRightAttribute(3, a.id).attribute_id, SQLGetRightAttribute(3, a.id).description, SQLGetRightAttribute(3, a.id).price),
                             numberOfFloors = 0,
                             GroundPlots = null,
                             OutsideWall = new ImageInherit(SQLGetRightAttribute(5, a.id).image, SQLGetRightAttribute(5, a.id).attribute_id, SQLGetRightAttribute(5, a.id).description, SQLGetRightAttribute(5, a.id).price),
                             OutsideWallColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(902, a.id).description), SQLSplitColorG(SQLGetRightAttribute(902, a.id).description), SQLSplitColorB(SQLGetRightAttribute(902, a.id).description)),
                             InsideWall = new ImageInherit(SQLGetRightAttribute(51, a.id).image, SQLGetRightAttribute(51, a.id).attribute_id, SQLGetRightAttribute(51, a.id).description, SQLGetRightAttribute(51, a.id).price),
                             InsideWallColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(901, a.id).description), SQLSplitColorG(SQLGetRightAttribute(901, a.id).description), SQLSplitColorB(SQLGetRightAttribute(901, a.id).description)),
                             RoofType = new ImageInherit(SQLGetRightAttribute(6, a.id).image, SQLGetRightAttribute(6, a.id).attribute_id, SQLGetRightAttribute(6, a.id).description, SQLGetRightAttribute(6, a.id).price),
                             RoofMaterial = new ImageInherit(SQLGetRightAttribute(61, a.id).image, SQLGetRightAttribute(61, a.id).attribute_id, SQLGetRightAttribute(61, a.id).description, SQLGetRightAttribute(61, a.id).price),
                             Window = new ImageInherit(SQLGetRightAttribute(7, a.id).image, SQLGetRightAttribute(7, a.id).attribute_id, SQLGetRightAttribute(7, a.id).description, SQLGetRightAttribute(7, a.id).price),
                             WindowColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(903, a.id).description), SQLSplitColorG(SQLGetRightAttribute(903, a.id).description), SQLSplitColorB(SQLGetRightAttribute(903, a.id).description)),
                             Door = new ImageInherit(SQLGetRightAttribute(71, a.id).image, SQLGetRightAttribute(71, a.id).attribute_id, SQLGetRightAttribute(71, a.id).description, SQLGetRightAttribute(71, a.id).price),
                             DoorColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(904, a.id).description), SQLSplitColorG(SQLGetRightAttribute(904, a.id).description), SQLSplitColorB(SQLGetRightAttribute(904, a.id).description)),
                             EnergySystem = new EHSystem(SQLGetRightAttribute(8, a.id).attribute_id, SQLGetRightAttribute(8, a.id).description, SQLGetRightAttribute(8, a.id).price),
                             HeatingSystem = new EHSystem(SQLGetRightAttribute(81, a.id).attribute_id, SQLGetRightAttribute(81, a.id).description, SQLGetRightAttribute(81, a.id).price),
                             NumberOfSocket = int.Parse(SQLGetRightAttribute(12, a.id).description),
                             Chimney = new ImageInherit(SQLGetRightAttribute(9, a.id).image, SQLGetRightAttribute(9, a.id).attribute_id, SQLGetRightAttribute(9, a.id).description, SQLGetRightAttribute(9, a.id).price),
                             Pool = new ImageInherit(SQLGetRightAttribute(10, a.id).image, SQLGetRightAttribute(10, a.id).attribute_id, SQLGetRightAttribute(10, a.id).description, SQLGetRightAttribute(10, a.id).price),
                             Poolsize = SQLGetRightAttribute(11, a.id).description + " m²",
                             Fence = new ImageInherit(SQLGetRightAttribute(101, a.id).image, SQLGetRightAttribute(101, a.id).attribute_id, SQLGetRightAttribute(101, a.id).description, SQLGetRightAttribute(101, a.id).price),
                             FenceColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(905, a.id).description), SQLSplitColorG(SQLGetRightAttribute(905, a.id).description), SQLSplitColorB(SQLGetRightAttribute(905, a.id).description))
                         }).ToList();

                con.Close();
            }
        }

        public HouseSummary SQLGetHouseconfig(int id)
        {
            HouseSummary house;
            //List<ImageInherit> Gplots;

            using (SQLiteConnection con = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath))
            {

                house = (from a in con.Table<Mdh_Users>()
                         from b in con.Table<Houseconfig>()
                         from f in con.Table<Ymdh_House_Package>()
                         from g in con.Table<Project>()
                         where a.id.Equals(b.customer_user_id)
                         && a.id.Equals(1)
                         && b.house_package_id.Equals(f.house_package_id)
                         && g.project_id.Equals(id)

                         select new HouseSummary
                         {
                             Customer = new Customer(a.id, a.name, SQLCustomerCountProject(a.id), SQLCustomerCountHouseconfig(a.id)),
                             Consultant = new Consultant(b.consultant_user_id, SQLGetConsultantName(b.consultant_user_id)),
                             Package = new ImageInherit(f.image, f.house_package_id, f.description, f.price),
                             Plot = new ImageInherit(SQLGetRightAttribute(3, a.id).image, SQLGetRightAttribute(3, a.id).attribute_id, SQLGetRightAttribute(3, a.id).description, SQLGetRightAttribute(3, a.id).price),
                             numberOfFloors = 0,
                             GroundPlots = new List<ImageInherit>(),
                             OutsideWall = new ImageInherit(SQLGetRightAttribute(5, a.id).image, SQLGetRightAttribute(5, a.id).attribute_id, SQLGetRightAttribute(5, a.id).description, SQLGetRightAttribute(5, a.id).price),
                             OutsideWallColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(902, a.id).description), SQLSplitColorG(SQLGetRightAttribute(902, a.id).description), SQLSplitColorB(SQLGetRightAttribute(902, a.id).description)),
                             InsideWall = new ImageInherit(SQLGetRightAttribute(51, a.id).image, SQLGetRightAttribute(51, a.id).attribute_id, SQLGetRightAttribute(51, a.id).description, SQLGetRightAttribute(51, a.id).price),
                             InsideWallColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(901, a.id).description), SQLSplitColorG(SQLGetRightAttribute(901, a.id).description), SQLSplitColorB(SQLGetRightAttribute(901, a.id).description)),
                             RoofType = new ImageInherit(SQLGetRightAttribute(6, a.id).image, SQLGetRightAttribute(6, a.id).attribute_id, SQLGetRightAttribute(6, a.id).description, SQLGetRightAttribute(6, a.id).price),
                             RoofMaterial = new ImageInherit(SQLGetRightAttribute(61, a.id).image, SQLGetRightAttribute(61, a.id).attribute_id, SQLGetRightAttribute(61, a.id).description, SQLGetRightAttribute(61, a.id).price),
                             Window = new ImageInherit(SQLGetRightAttribute(7, a.id).image, SQLGetRightAttribute(7, a.id).attribute_id, SQLGetRightAttribute(7, a.id).description, SQLGetRightAttribute(7, a.id).price),
                             WindowColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(903, a.id).description), SQLSplitColorG(SQLGetRightAttribute(903, a.id).description), SQLSplitColorB(SQLGetRightAttribute(903, a.id).description)),
                             Door = new ImageInherit(SQLGetRightAttribute(71, a.id).image, SQLGetRightAttribute(71, a.id).attribute_id, SQLGetRightAttribute(71, a.id).description, SQLGetRightAttribute(71, a.id).price),
                             DoorColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(904, a.id).description), SQLSplitColorG(SQLGetRightAttribute(904, a.id).description), SQLSplitColorB(SQLGetRightAttribute(904, a.id).description)),
                             EnergySystem = new EHSystem(SQLGetRightAttribute(8, a.id).attribute_id, SQLGetRightAttribute(8, a.id).description, SQLGetRightAttribute(8, a.id).price),
                             HeatingSystem = new EHSystem(SQLGetRightAttribute(81, a.id).attribute_id, SQLGetRightAttribute(81, a.id).description, SQLGetRightAttribute(81, a.id).price),
                             NumberOfSocket = int.Parse(SQLGetRightAttribute(12, a.id).description),
                             Chimney = new ImageInherit(SQLGetRightAttribute(9, a.id).image, SQLGetRightAttribute(9, a.id).attribute_id, SQLGetRightAttribute(9, a.id).description, SQLGetRightAttribute(9, a.id).price),
                             Pool = new ImageInherit(SQLGetRightAttribute(10, a.id).image, SQLGetRightAttribute(10, a.id).attribute_id, SQLGetRightAttribute(10, a.id).description, SQLGetRightAttribute(10, a.id).price),
                             Poolsize = SQLGetRightAttribute(11, a.id).description + " m²",
                             Fence = new ImageInherit(SQLGetRightAttribute(101, a.id).image, SQLGetRightAttribute(101, a.id).attribute_id, SQLGetRightAttribute(101, a.id).description, SQLGetRightAttribute(101, a.id).price),
                             FenceColor = new ColorPalette(SQLSplitColorR(SQLGetRightAttribute(905, a.id).description), SQLSplitColorG(SQLGetRightAttribute(905, a.id).description), SQLSplitColorB(SQLGetRightAttribute(905, a.id).description))

                         }).Single();

                con.Close();
            }
            return house;
        }

        #endregion

            #endregion

            #region GeneralFunctions
            /// <summary>
            /// In dieser Region werden allgemeine Funktionien gepseichert
            /// </summary>

            // Resetet alle Listen und selected Items
        public void ResetProperties()
        {
            //Variablen
            //HausKonfigurator
            House = new HouseSummary();
            TotalPrice = 0;
            NumberOfFloorDB = 0;
            //Appointments
            DateAppointment = new DateTime();
            TimeAppoitment = new TimeSpan();
            DateTimeNow = new DateTime();

            //Listen
            //HausKonfigurator
            Customers.Clear();
            ImagesHouse.Clear();
            imagesPlot.Clear();
            NumberFloors.Clear();
            FloorsGroundPlot.Clear();
            ListOutsideWall.Clear();
            ListColorOutsideWall.Clear();
            ListInsideWall.Clear();
            ListColorInsideWall.Clear();
            ListRoofType.Clear();
            ListRoofMaterial.Clear();
            ListWindows.Clear();
            ListColorWindows.Clear();
            ListDoors.Clear();
            ListColorDoors.Clear();
            ListEnergySystem.Clear();
            ListHeatingSystem.Clear();
            NumberSockets.Clear();
            ListChimneys.Clear();
            NumberPoolSizes.Clear();
            ListPools.Clear();
            ListFence.Clear();
            ListColorFence.Clear();
            //Appintments
            Appointments.Clear();
            Consultants.Clear();

            //SelectedItems
            //HausKonfigurator
            SelectedCustomerr = new Customer();
            SelectedItemFloor = 0;
            SelectedHouse = new ImageInherit();
            SelectedPlot = new ImageInherit();
            SelectedFloor = new ImageInherit();
            SelectedOutsideWall = new ImageInherit();
            SelectedColorOutsideWall = new ColorPalette();
            SelectedInsideWall = new ImageInherit();
            SelectedColorInsideWall = new ColorPalette();
            SelectedRoofType = new ImageInherit();
            SelectedRoofMaterial = new ImageInherit();
            SelectedWindow = new ImageInherit();
            SelectedColorWindow = new ColorPalette();
            SelectedDoor = new ImageInherit();
            SelectedColorDoor = new ColorPalette();
            SelectedEnergySystem = new EHSystem();
            SelectedHeatingSystem = new EHSystem();
            SelectedSocket = 0;
            SelectedChimney = new ImageInherit();
            SelectedPoolSize = 0;
            SelectedPool = new ImageInherit();
            SelectedFence = new ImageInherit();
            SelectedColorFence = new ColorPalette();
            //Appintments
            selectedAppointment = new Appointment();
            SelectedConsultant = new Consultant();

        }
        #endregion

        #region Test Methoden






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
        #endregion
    }
}
