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
using System.Threading.Tasks;
using Windows.System.Threading;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fallstudie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        Frame a = new Frame();
        public Login()
        {
            this.InitializeComponent();
           
        }

        private async void PassportSignInButton_Click(object sender, RoutedEventArgs e)
        {
            if(UsernameTextBox.Text == "ermin" && PasswordBox.Password == "123")
            {
                a = StartPage.FrameObject.GetObject();
                a.Navigate(typeof(MainPage));
                //ProgressRingLoading.IsActive = await Task.Run(() => LoadPage());
                //TextBlockLoading.Visibility = Visibility;

            }
            else
            {
                ErrorMessage.Text = "Username oder Passwort ist falsch!";
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }

        }
        private bool LoadPage()
        {

            return true;
        }
    }
   
}
