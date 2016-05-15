using Fallstudie.DBModel;
using Fallstudie.Model;
using GalaSoft.MvvmLight.Command;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

namespace Fallstudie.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region PROPERTIES

        #region Variablen
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

        //Test Variable
        private string msg = "HausKonfigurator";
        public string MSG
        {
            get { return msg; }
            set { msg = value; }
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

        // der ausgewählte Kunde in der Kundenliste
        private Customer selectedCustomerr;
        public Customer SelectedCustomerr
        {
            get { return selectedCustomerr; }
            set { selectedCustomerr = value; OnChange("SelectedCustomerr"); }
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
        //Das konfigurierte Haus wird in die Datenbank gespeichert -> Projekt wurde erstellt
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

        public MainViewModel()
        {
            InitializeButtons();

            Customers.Clear();

            Customers.Add(new Customer("Max", "Mustermann", 0, 0));
            Customers.Add(new Customer("Danielo", "Pizzamento", 1, 2));
            Customers.Add(new Customer("Fritz", "Immertoll", 1, 4));

            //Datenbank erstellen
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db2.sqlite; foreign keys=true;");
            if (!File.Exists(DbPath))
            {
                Conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath);
                SQLCreateTable();
                SQLInsertAttributeGroup();
            }
        }

        #region SQL Befehler
        #region Erstellen der Tabellen
        public void SQLCreateTable()
        {
            Conn.CreateTable<DBModel.Attribute>();
            Conn.CreateTable<Attribute_Group>();
            Conn.CreateTable<Houseconfig>();
            Conn.CreateTable<Houseconfig_Has_Attribute>();
            Conn.CreateTable<Housefloor>();
            Conn.CreateTable<Package_Not_Attribute>();
            Conn.CreateTable<Project>();
            Conn.CreateTable<Ymdh_House_Package>();
            Conn.CreateTable<Ymdh_House_Package_Status>();
        }

        //Attribut Gruppen werden erstellt        
        public void SQLInsertAttributeGroup()
        {
            //Attribut Grundstück
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 3,
                description = "Grundstück",
                modifieddate = DateTime.Now
            });
            //Attribut Wände
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 5,
                description = "Wand",
                modifieddate = DateTime.Now
            });
            //Attribut Dach
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 6,
                description = "Dach",
                modifieddate = DateTime.Now
            });
            //Attribut Fenster & Türen
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 7,
                description = "Fenster und Türen",
                modifieddate = DateTime.Now
            });
            //Attribut Energie- und Heizungsysteme
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 8,
                description = "Fenster und Türen",
                modifieddate = DateTime.Now
            });
            //Attribut Zusatz (Steckdosen/Raum & Kamin)
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 9,
                description = "Steckdosen/Raum und Kamin",
                modifieddate = DateTime.Now
            });
            //Attribut Außenbereiche (Pool & Zaun)
            Conn.Insert(new Attribute_Group
            {
                attribute_group_id = 10,
                description = "Pool und Zaun",
                modifieddate = DateTime.Now
            });
        }
 
        //Projekt wird erstellt
        public void SQLCreateProject()
        {
            Conn.Insert(new Houseconfig
            {
                price = int.Parse(TotalPrice.ToString()),
                status = "1",
                price_floor = int.Parse(SelectedFloor.Price.ToString()),
                modifieddate = DateTime.Now,
                house_package_id = SelectedHouse.Id

            });
            Conn.Insert(new Housefloor
            {
                price = int.Parse(SelectedFloor.Price.ToString()),
                sketch = "sketch",
                modifieddate = DateTime.Now,
                houseconfig_id = 123,
                area = 1
            });
            Conn.Insert(new Project
            {
                startdate = DateTime.Now,
                enddate = DateTime.Now,
                invoice = "Rechnung 1",
                status = "Fertig",
                description = "",
                modifieddate = DateTime.Now,
            });
            Conn.Insert(new Houseconfig_Has_Attribute
            {
                houseconfig_id = 123,
                attribute_id = 1,
                amount = 1,
                special = "",
                modifieddate = DateTime.Now
            });
        }
        #endregion

        #endregion

        #region Alle Methoden

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

                ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/2Haeuser/Haus1.png", 1111, 107000));
                ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/2Haeuser/Haus2.png", 1222, 150000));
                ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/2Haeuser/Haus3.png", 1333, 210000));
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

                //Grundstückbilder werden angezeigt
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck1.png", 2111, 80000));
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck2.png", 2222, 90000));
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck3.png", 2333, 99000));

                
                //Conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
                //SQLiteCommand command = new SQLiteCommand();

                
                
                SQLiteConnection sqConnection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath);;
                string abc="";
                var mySelectQuery = "Select image from Attribute where attribute_id=1";
                //sqConnection.CreateCommand(mySelectQuery, abc);
                //var abc = new DBModel.Attribute(){};
                SQLiteCommand command = sqConnection.CreateCommand(mySelectQuery, "");

                //var ImageDb = sqConnection.Get<DBModel.Attribute>(abc);
                var dialog = new MessageDialog(command.ToString());
                await dialog.ShowAsync();

                //Base64StringToBitmap(ImageDb.ToString());
                

            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Haus aus.");
                await dialog.ShowAsync();
            }
        }
        public async void Base64StringToBitmap(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;

            var dialog = new MessageDialog(memoryStream.ToString());
            await dialog.ShowAsync();
            BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.SetSource(memoryStream);

            memoryStream = null;
            byteBuffer = null;

            //return bitmapImage;
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
                if(SelectedPlot != null)
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

            ListOutsideWall.Clear();
            ListColorOutsideWall.Clear();
            ListInsideWall.Clear();
            ListColorInsideWall.Clear();

            //außenwände befüllen
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/AußenHolz.png", 1234, "Holzwand", 1000));
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/AußenZiegel.png", 1234, "Ziegel", 2000));
            ListOutsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/AußenBeton.png", 1234, "Beton", 3000));

            //farben der außenwände festlegen
            ListColorOutsideWall.Add(new ColorPalette(Colors.Red));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Blue));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Yellow));
            ListColorOutsideWall.Add(new ColorPalette(Colors.GreenYellow));
            ListColorOutsideWall.Add(new ColorPalette(Colors.Khaki));


            //innenwände werden befüllt
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/InnenHolz.png", 1234, "Holzwand", 1000));
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/InnenRigips.png", 1234, "Rigipswand", 2000));
            ListInsideWall.Add(new ImageInherit("ms-appx:///Bilder/5Wand/InnenZiegel.png", 1234, "Ziegelwand", 3000));

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

                ListRoofType.Clear();
                ListRoofMaterial.Clear();

                //Dachtypen werden befüllt
                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Pultdach.png", 1234, "Pultdach", 2500));
                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Satteldach.png", 1111, "Satteldach", 3000));
                ListRoofType.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Flachdach.png", 121234, "Flachdach", 1000));

                //Dachtmaterialien werden befüllt
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Ziegeldach.png", 1234, "Ziegel", 50));
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Aluminiumdach.png", 121234, "Aluminium", 90));
                ListRoofMaterial.Add(new ImageInherit("ms-appx:///Bilder/6Dach/Faserzementplattendach.png", 121234, "Faserzementplatten", 30));
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

                //Fenster befüllen
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Rundfenster.png", 1234, "Rund", 100));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Trapezfenster.png", 1234, "Trapez", 200));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Einteilig.png", 1234, "Einteilig", 300));
                ListWindows.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Zweiteilig.png", 1234, "Zweiteilig", 200));

                //farben der Fenster festlegen
                ListColorWindows.Add(new ColorPalette(Colors.Blue));
                ListColorWindows.Add(new ColorPalette(Colors.White));
                ListColorWindows.Add(new ColorPalette(Colors.Black));
                ListColorWindows.Add(new ColorPalette(Colors.Gray));
                ListColorWindows.Add(new ColorPalette(Colors.Brown));

                //Türen befüllen
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Schiebetür", 100));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Glastür.png", 1234, "Glas", 100));
                ListDoors.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Holztür.png", 1234, "Holz", 200));

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

                ListEnergySystem.Add(new EHSystem("Solar", 1200));
                ListEnergySystem.Add(new EHSystem("Stromnetz", 0));

                ListHeatingSystem.Add(new EHSystem("Gasheizung", 400));
                ListHeatingSystem.Add(new EHSystem("Ölheizung", 200));
                ListHeatingSystem.Add(new EHSystem("Pellet", 100));

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

                NumberSockets.Add(1);
                NumberSockets.Add(2);
                NumberSockets.Add(3);
                NumberSockets.Add(4);
                NumberSockets.Add(5);
                NumberSockets.Add(6);
                NumberSockets.Add(7);
                NumberSockets.Add(8);
                NumberSockets.Add(9);
                NumberSockets.Add(10);

                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/8Kamin/Kamin1.png", 1234, "Kamin1", 100));
                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/8Kamin/Kamin2.png", 1234, "Kamin2", 200));
                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/8Kamin/Kamin3.png", 1234, "Kamin3", 300));

            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Energie und ein Heizungssystem aus.");
                await dialog.ShowAsync();
            }
        }


        //Hier wird zu Schritt 10 weitergeleitet
        private  void ButtonForwardChooseOutsideAreaMethod()
        {
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt10Aussenbereiche));

            NumberPoolSizes.Clear();
            ListPools.Clear();
            ListFence.Clear();
            ListColorFence.Clear();

            NumberPoolSizes.Add(10);
            NumberPoolSizes.Add(15);
            NumberPoolSizes.Add(20);
            NumberPoolSizes.Add(25);
            NumberPoolSizes.Add(30);
            NumberPoolSizes.Add(35);
            NumberPoolSizes.Add(50);

            ListPools.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Pool1.png", 1234, "Rund", 100));
            ListPools.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Pool2.png", 1234, "Zwei Rundpoole", 200));
            ListPools.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Pool3.png", 1234, "Eckig", 100));

            ListFence.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Zaun1.png", 1234, "Zaun 1", 100));
            ListFence.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Zaun2.png", 1234, "Zaun 2", 100));
            ListFence.Add(new ImageInherit("ms-appx:///Bilder/9Aussenbereiche/Zaun3.png", 1234, "Zaun 3", 300));

            ListColorFence.Add(new ColorPalette(Colors.IndianRed));
            ListColorFence.Add(new ColorPalette(Colors.Beige));
            ListColorFence.Add(new ColorPalette(Colors.Black));

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
                EnergySystem = SelectedEnergySystem.Name,
                HeatingSystem = selectedHeatingSystem.Name,
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
