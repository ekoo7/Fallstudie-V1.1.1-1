using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fallstudie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       
        public static Frame frame1 = new Frame();
        public MainPage()
        {
           
            this.InitializeComponent();
            //damit man auf die Frame von MainPage zugreifen kann
            frame1 = MyFrame;
            MyFrame.Navigate(typeof(Pages.Startseite));


        }

        public static class FrameObject{

            //damit man auf die Frame von MainPage zugreifen kann
            public static Frame GetObject()
            {
                return frame1;
            }
        }

        //Hier wird bestimmt welche Page im Frame angezeigt wird
        private void ListBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Startseite.IsSelected)
            {
                MyFrame.Navigate(typeof(Pages.Startseite));
                closeSplitView();
            }
            if (Kunden.IsSelected)
            {
                //BackButton.Visibility = Visibility.Collapsed;
                MyFrame.Navigate(typeof(Pages.KundenPage));
                closeSplitView();
               
            }
            if (HausKonfigurator.IsSelected)
            {
                MyFrame.Navigate(typeof(Pages.HausKonfiguratorPage));
                closeSplitView();

            }
            if (Projekte.IsSelected)
            {
                MyFrame.Navigate(typeof(Pages.ProjektePage));
                closeSplitView();


            }
            if (Termine.IsSelected)
            {
                MyFrame.Navigate(typeof(Pages.TerminePage));
                closeSplitView();

            }
        }

        //Das Öffnen und Schließen vom Menü
        private void MenueButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        private void closeSplitView()
        {
            MySplitView.IsPaneOpen = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if(frame1.CanGoBack)
            frame1.GoBack();
        }
    }
}
