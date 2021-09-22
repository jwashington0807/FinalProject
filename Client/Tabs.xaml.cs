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
    /// Interaction logic for Tabs.xaml
    /// </summary>
    public partial class Tabs : Window
    {
        string userName;
        bool isAdmin = false;
        ICommunicator channel;
        string currentFile;

        public Tabs(string username, bool isAdmin)
        {
            InitializeComponent();

            //Set global variables
            this.userName = username;
            this.isAdmin = isAdmin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the URL for the .... client?
            string endpoint = "http://localhost:8080/ICommunicator";

            // Set the User Name Label
            userLabel.Content = "User: " + userName;

            // Load Files
            onLoadFiles();

            //From Service Model DLL
            EndpointAddress baseAddress = new EndpointAddress(endpoint);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, endpoint);
            channel = factory.CreateChannel();

            //PeerListener.cs 
            PeerListener peerListener = new PeerListener();
            Thread rcvThread = new Thread(() =>
            {
                while (true)
                {
                    List<string> res = peerListener.getResult();
                    Dispatcher.Invoke(() =>
                    {
                        foreach (var item in res)
                        {
                            if(item.Contains("[ENDPOINT]"))
                            {
                                string[] ss = item.Split(';');
                                string endpoint1 = ss[2];
                                string file_name = ss[1];

                                txtFileLocSearch.Items.Add(file_name + ";" + endpoint1);
                            }

                            if(item.Contains("Nothing was found"))
                            {
                                txtFileLocSearch.Items.Add(item);
                            }
                        }
                    });
                }
            });

            rcvThread.IsBackground = true;

            peerListener.StartPeerListener(ListenerCreated);
            rcvThread.Start();
        }

        // When the window loads, find all the files in the same directory
        private void onLoadFiles()
        {
            DirectoryInfo d = new DirectoryInfo(@"."); 

            lstFilesCopy.Items.Clear();
            System.IO.FileInfo[] Files = d.GetFiles();

            foreach (var item in Files)
            {
                lstFilesCopy.Items.Add(item.Name);
            }
        }

        private void btn_SignOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void ListenerCreated(string endpoint)
        {
            Dispatcher.Invoke(() => {
                endpointAddress.Content = endpoint;
                endpointAddress.Visibility = Visibility.Visible;
                btn_Copy.IsEnabled = false;
                btn_Download.IsEnabled = false;

            });
        }

        //Click event that handles the searching of indexed files
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            txtFileLocSearch.Items.Clear();
            btn_Download.IsEnabled = true;
            currentFile = txtSearchCriteria1.Text;
            WCF_Peer_Comm.FileInfo fi = new WCF_Peer_Comm.FileInfo(txtSearchCriteria1.Text, (string)chkTitleSearch.IsChecked.ToString(), (string)chkKeysSearch.IsChecked.ToString(), (string)chkDescSearch.IsChecked.ToString(), endpointAddress.Content.ToString());

            channel.Search(fi, (bool)chkTitleSearch.IsChecked, (bool)chkKeysSearch.IsChecked, (bool)chkDescSearch.IsChecked);
        }

        private void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            onLoadFiles();
        }

        //Click event that Indexes the selected file
        private void btn_Index_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(@".");

            System.IO.FileInfo[] Files = d.GetFiles();

            foreach (var item in Files)
            {
                if (item.Name == lstFilesCopy.SelectedItem.ToString())
                {
                    WCF_Peer_Comm.FileInfo fileInfo = new WCF_Peer_Comm.FileInfo(item.Name, txtTitleCopy.Text, txtKeysCopy.Text, txtDescCopy.Text, endpointAddress.Content.ToString());
                    channel.NewFile(fileInfo);

                    //Add Record to Log
                    AddRecordToLog(item.Name, "Indexed");
                }
            }
        }

        //Called function when an event needs to be added to the Logs file
        private void AddRecordToLog(string name, string action)
        {
            channel.LogRecord(userName, name, action);
        }

        //User event that downloads the selected index file
        private void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            string[] ss = txtFileLocSearch.SelectedItem.ToString().Split(';');
            string endpoint = ss[1];
            string file_name = ss[0];

            EndpointAddress baseAddress = new EndpointAddress(endpoint);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IPeerCommunicator> factory
              = new ChannelFactory<IPeerCommunicator>(binding, endpoint);
            IPeerCommunicator p2pchannel = factory.CreateChannel();
            p2pchannel.DownloadRequest(new WCF_Peer_Comm.FileInfo(file_name, String.Empty, String.Empty, String.Empty, endpointAddress.Content.ToString()));

            //Add Record to Log
            AddRecordToLog(file_name, "Downloaded");
            MessageBox.Show("The file has been downloaded");
        }

        //Event that enables metadata entries
        private void lstFilesCopy_Selected(object sender, RoutedEventArgs e)
        {
            btn_Copy.IsEnabled = true;

            txtTitleCopy.IsEnabled = true;
            txtTitleCopy.Background = Brushes.White;
            txtDescCopy.IsEnabled = true;
            txtDescCopy.Background = Brushes.White;
            txtKeysCopy.IsEnabled = true;
            txtKeysCopy.Background = Brushes.White;
        }

        //function that retireves the log records from the server and presents them to the user
        private void btn_AuditFilter_Click(object sender, RoutedEventArgs e)
        {
            lstTransaction.Items.Clear();
            List<string> records = channel.GetAuditResults(userName, txtFileNameAudit.Text, isAdmin);
            
            foreach(string record in records)
            {
                lstTransaction.Items.Add(record);
            }
        }
    }
}
