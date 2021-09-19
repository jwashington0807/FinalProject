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
        int role = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtUserName.Text) || String.IsNullOrEmpty(txtPassword.Password))
                {
                    MessageBox.Show("Input can't be blank");
                }
                else
                {
                    if (Login())
                    {
                        Tabs twdw = new Tabs(txtUserName.Text);
                        twdw.Show();
                        this.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                MessageBox.Show(error);
            }
        }

        public bool Login()
        {
            string line = String.Empty;

            StreamReader file = new StreamReader(@"C:\Users\JT\source\repos\Project4Prototype\Project4Prototype\Project4Prototype\Administration\passwd.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] ss = line.Split(':');

                if (txtUserName.Text == ss[0])
                {
                    if (txtPassword.Password == ss[1])
                    {
                        role = ss[2];
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Your password is incorrect");

                        return false;
                    }
                }
            }

            file.Close();

            MessageBox.Show("The username or password that you have entered is incorrect");

            return false;
        }
    }
}
