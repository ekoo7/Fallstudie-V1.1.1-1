using Fallstudie.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Fallstudie.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        ///This is a GITHUB Test



        /// <summary>
        /// PROPERTIES
        /// </summary>

        //Test Variable
        private string msg = "HausKonfigurator";
        public string MSG
        {
            get { return msg; }
            set { msg = value; }
        }

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

        public MainViewModel()
        {
            InitializeButtons();

            customers.Add(new Customer("Max", "Mustermann", 0, 0));
            customers.Add(new Customer("Danielo", "Pizzamento", 1, 2));
            customers.Add(new Customer("Fritz", "Immertoll", 1, 4));

            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus1.png", 1111, "107.000€"));
            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus2.png", 1222, "150.100€"));
            ImagesHouse.Add(new ImageInherit("ms-appx:///Bilder/Haeuser/Haus3.png", 1333, "210.380€"));

            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck1.png", 2111, "80.000€"));
            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck2.png", 2222, "90.000€"));
            ImagesPlot.Add(new ImageInherit("ms-appx:///Bilder/Grundstuecke/Grundstueck3.png", 2333, "99.000€"));
        }

        //Hier werden alle Buttons initialisiert
        private void InitializeButtons()
        {
            ButtonForwardChooseCustomer = new RelayCommand(ButtonForwardChooseCustomerClicked);
            NewCustomerButton = new RelayCommand(NewCustomerButtonMethod);
            ButtonForwardChooseHouse = new RelayCommand(ButtonForwardChooseHouseMethod);
            ButtonForwardChoosePlot = new RelayCommand(ButtonForwardChoosePlotMethod);
        }

        //Hier wird im Schritt 2 das Haus ausgewählt und auf Weiter geklickt
        private async void ButtonForwardChooseHouseMethod()
        {
            if (SelectedHouse != null)
            {
                var dialog = new MessageDialog("Id Haus: " + SelectedHouse.Image1.Name);
                await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt3GrundstückAuswahl));
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
                var dialog = new MessageDialog("Id Grundstück: " + SelectedPlot.Image1.Name);
                await dialog.ShowAsync();
                GetFrame();
                a.Navigate(typeof(Pages.HKPages.Schritt4Grundriss));
            }
            else
            {
                var dialog = new MessageDialog("Bitte wählen Sie ein Grundstück aus.");
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
