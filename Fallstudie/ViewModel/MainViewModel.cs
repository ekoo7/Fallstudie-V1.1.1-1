using Fallstudie.Model;
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
        #region PROPERTIES

        #region Variablen
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

        #endregion

        #endregion

        public MainViewModel()
        {
            InitializeButtons();

            customers.Add(new Customer("Max", "Mustermann", 0, 0));
            customers.Add(new Customer("Danielo", "Pizzamento", 1, 2));
            customers.Add(new Customer("Fritz", "Immertoll", 1, 4));
        }

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
                
                //Grundstückbilder werden angezeigt
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck1.png", 2111, 80000));
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck2.png", 2222, 90000));
                ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/3Grundstuecke/Grundstueck3.png", 2333, 99000));
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
                if(SelectedPlot != null)
                TotalPrice += SelectedPlot.Price;

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

        //Hier wird weitergeleitet auf Schritt 5
        private void ButtonForwardChooseWallMethod()
        {
            //var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
            //await dialog.ShowAsync();
            GetFrame();
            a.Navigate(typeof(Pages.HKPages.Schritt5Wand));
            if (selectedItemFloor != NumberOfFloorDB) TotalPrice += SelectedFloor.Price;

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

                listWindows.Clear();
                ListColorWindows.Clear();
                ListDoors.Clear();
                listColorDoors.Clear();

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
                listHeatingSystem.Clear();

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
                NumberSockets.Clear();
                ListChimneys.Clear();

                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt9Zusatz));

                totalPrice += SelectedEnergySystem.Price + selectedHeatingSystem.Price;

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

                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Schiebetür", 100));
                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Schiebetür", 200));
                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Schiebetür", 300));
                ListChimneys.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Schiebetür", 400));



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

            ListPools.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Rund", 100));
            ListPools.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Zwei Rundpoole", 200));
            ListPools.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Eckig", 100));

            ListFence.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Zaun 1", 100));
            ListFence.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Zaun 2", 100));
            ListFence.Add(new ImageInherit("ms-appx:///Bilder/7FensterTueren/Schiebetür.png", 1234, "Zaun 3", 300));

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

        //wenn anzahl von stockwerken angeklickt wird -> anzeigen der Grundrisse
        public void ChooseGroundPlots()
        {
            FloorsGroundPlot.Clear();
            //Anzahl der Stockwerke vom ausgewähltem haus aus der DB
            numberOfFloorDB = 1;

            FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundirss/GrundrissErdgeschoss.png", 1111, "Erdgeschoss", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            if (SelectedItemFloor == 1 || SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundirss/GrundrissStock1.png", 1122,  "Stockwerk 1", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }
            if(SelectedItemFloor == 2)
            {
                FloorsGroundPlot.Add(new ImageInherit("ms-appx:///Bilder/4Grundirss/GrundrissStock2.png", 1131, "Stockwerk 2", new RelayCommand(ButtonDrawSketchMethod), SelectedItemFloor, numberOfFloorDB));
            }
            OnChange("FloorsGroundPlot");
        }
        private void ButtonDrawSketchMethod()
        {
            AsynchMethod();
        }



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
