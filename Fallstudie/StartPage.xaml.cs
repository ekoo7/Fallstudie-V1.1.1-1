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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fallstudie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        public static Frame frame1 = new Frame();
        public StartPage()
        {
            this.InitializeComponent();
            frame1 = MyFrameStartPage;
            MyFrameStartPage.Navigate(typeof(Login));
        }
        public static class FrameObject
        {

            //damit man auf die Frame von MainPage zugreifen kann
            public static Frame GetObject()
            {
                return frame1;
            }
        }

        private void MyFrameStartPage_Navigated(object sender, NavigationEventArgs e)
        {
           
        }
    }
}
