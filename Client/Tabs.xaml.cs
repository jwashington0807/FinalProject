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

            this.userName = username;
            this.isAdmin = isAdmin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the URL for the .... client?
            string endpoint = "http://localhost:8080/ICommunicator";

            // Set the User Name Label
            userLabel.Content = "User: " + userName;

            // Security for Admin
            if(!isAdmin)
            {
                auditTab.Visibility = Visibility.Hidden;
            }

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
                            txtFileLocSearch.Items.Add(item);
                        }
                    });
                }
            });

            rcvThread.IsBackground = true;

            peerListener.StartPeerListener(ListenerCreated);
            rcvThread.Start();
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

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            txtFileLocSearch.Items.Clear();
            btn_Download.IsEnabled = true;
            currentFile = txtSearchCriteria1.Text;
            WCF_Peer_Comm.FileInfo fi = new WCF_Peer_Comm.FileInfo(txtSearchCriteria1.Text, endpointAddress.Content.ToString());
            channel.Search(fi);
        }

        private void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(@"."); //Assuming Test is your Folder

            System.IO.FileInfo[] Files = d.GetFiles(); //Getting Text files
            foreach (var item in Files)
            {
                WCF_Peer_Comm.FileInfo fileInfo = new WCF_Peer_Comm.FileInfo(item.Name, endpointAddress.Content.ToString());
                channel.NewFile(fileInfo);
                lstFilesCopy.Items.Add(item.Name);
            }
        }

        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(@"."); //Assuming Test is your Folder

            System.IO.FileInfo[] Files = d.GetFiles(); //Getting Text files
            foreach (var item in Files)
            {
                lstFilesCopy.Items.Add("Indexing: " + item.Name);
                WCF_Peer_Comm.FileInfo fileInfo = new WCF_Peer_Comm.FileInfo(item.Name, endpointAddress.Content.ToString());
                channel.NewFile(fileInfo);
                lstFilesCopy.Items.Add("Indexing Compelte for: " + item.Name);
            }
        }

        private void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            string endpoint = txtFileLocSearch.SelectedItem.ToString();
            EndpointAddress baseAddress = new EndpointAddress(endpoint);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IPeerCommunicator> factory
              = new ChannelFactory<IPeerCommunicator>(binding, endpoint);
            IPeerCommunicator p2pchannel = factory.CreateChannel();
            p2pchannel.DownloadRequest(new WCF_Peer_Comm.FileInfo(currentFile, endpointAddress.Content.ToString()));
        }

        private void lstFilesCopy_Selected(object sender, RoutedEventArgs e)
        {


        }
    }
}
