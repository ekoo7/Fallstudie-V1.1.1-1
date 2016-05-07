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

namespace Fallstudie.Pages.HKPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Schritt2HausAuswahl : Page
    {
        public static StackPanel stackPanelSchritt2 = new StackPanel();
        public Schritt2HausAuswahl()
        {
           
            this.InitializeComponent();
            stackPanelSchritt2 = StackSchritt2;
        }
        public static class StackObjectSchritt2
        {

            //damit man auf die Frame von MainPage zugreifen kann
            public static StackPanel GetObject()
            {
                return stackPanelSchritt2;
            }
        }
    }
}
