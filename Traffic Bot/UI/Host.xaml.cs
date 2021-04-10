using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Traffic_Bot.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Host : Window
    {

        /// <summary>
        /// Application class handle
        /// </summary>
        public Class.application APPLICATION;

        /// <summary>
        /// Constructor
        /// Called when the Host form is loaded
        /// </summary>
        public Host()
        {
            InitializeComponent();

            // Initialise the application class
            APPLICATION = new Class.application(this);


            // Check that a valid internet connection is available
            // Internet is required for this application to work
            if (APPLICATION.IsInternetAvailable())
            {
                // Load a new instance of the main menu and bring into view
                this.Content = new UI.MainMenu(APPLICATION);
            } else
            {
                // An active internet connection could not be found
                MessageBox.Show("You need to be connected to the internet to use this application!");

                // Close the application
                Application.Current.Shutdown(0);
            }

        }



    }
}
