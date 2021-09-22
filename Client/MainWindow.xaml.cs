using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using WCF_Peer_Comm;
using System.IO;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string role = String.Empty;
        bool isAdmin = false;
        ICommunicator channel;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         Click function that will attempt to verify the user.
         */
        private void login_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Attempt to connect to the server
                string endpoint = "http://localhost:8080/ICommunicator";

                // From Service Model DLL
                EndpointAddress baseAddress = new EndpointAddress(endpoint);
                WSHttpBinding binding = new WSHttpBinding();
                ChannelFactory<ICommunicator> factory
                  = new ChannelFactory<ICommunicator>(binding, endpoint);
                channel = factory.CreateChannel();

                // If the textboxes are empty, output validation message
                // Else attempt to login with credentials
                if (String.IsNullOrEmpty(txtUserName.Text) || String.IsNullOrEmpty(txtPassword.Password)) {
                    MessageBox.Show("Input can't be blank");
                }
                else {
                    string result = String.Empty;
                    result = channel.Login(txtUserName.Text, txtPassword.Password);
                    processLogin(result);
                }
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                MessageBox.Show(error);
            }
        }

        private void processLogin(string res)
        {
            if (String.IsNullOrEmpty(res)){
                MessageBox.Show("The username or password that you have entered is incorrect");
            }

            else
            {
                if (res == "1")
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = false;
                }

                // Open the Syracuse XAML file with the user's credential
                Tabs twdw = new Tabs(txtUserName.Text, isAdmin);
                twdw.Show();

                // Close the MainWindow XAML
                this.Close();
            }
        }
    }
}
