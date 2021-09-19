using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCF_Peer_Comm;
using SWTools;
using System.ServiceModel;
using System.IO;
using System.Threading;
namespace Client
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode =InstanceContextMode.PerSession
)]
    class PeerListener : IPeerCommunicator
    {
        FileStream fs = null;
        static BlockingQueue<WCF_Peer_Comm.FileInfo> downloadReqBlockingQ = null;
        static BlockingQueue<List<string>> searchResBlockingQ = null;
        
        ServiceHost service = null;
        public  delegate void ListenerCreated(string localEndPoint);
        public event ListenerCreated OnListenerCreated;

        

        private void CreateRecvChannel(string address)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(PeerListener), baseAddress);
            service.AddServiceEndpoint(typeof(IPeerCommunicator), binding, baseAddress);
            service.Open();
        }
        public void StartPeerListener(Action<string> onListenerCreatedHandler)
        {
           OnListenerCreated += new PeerListener.ListenerCreated(onListenerCreatedHandler);
          
            if (downloadReqBlockingQ == null)
                downloadReqBlockingQ = new BlockingQueue<WCF_Peer_Comm.FileInfo>();
            if (searchResBlockingQ == null)
                searchResBlockingQ = new BlockingQueue<List<string>>();

            Thread startListener = new Thread(startListening);
            startListener.IsBackground = true;
            startListener.Start();
            Thread sendFileThread = new Thread(sendFile);
            sendFileThread.IsBackground = true;
            sendFileThread.Start();

        }
        void sendFile() {
            while (true)
            {
                WCF_Peer_Comm.FileInfo fileInfo = getDownLoadReq();
                string Name = fileInfo.Name;
                string target = fileInfo.Endpoint;
                Thread sendFileThread = new Thread(() => {
                    sendFile(Name, target);
                });
                sendFileThread.IsBackground = true;
                sendFileThread.Start();
            }
        
        }
        void sendFile(string name, string target) {
            string endpoint = target;
            EndpointAddress baseAddress = new EndpointAddress(endpoint);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IPeerCommunicator> factory
              = new ChannelFactory<IPeerCommunicator>(binding, endpoint);
            IPeerCommunicator channel = factory.CreateChannel();


            FileStream fs_local = null;
            long bytesRemaining;

            try
            {
                

                fs_local = File.OpenRead("./"+name);
                bytesRemaining = fs_local.Length;
                channel.openFileForWrite(name);
                while (true)
                {
                    long bytesToRead = Math.Min(1024, bytesRemaining);
                    byte[] blk = new byte[bytesToRead];
                    long numBytesRead = fs_local.Read(blk, 0, (int)bytesToRead);
                    bytesRemaining -= numBytesRead;

                    channel.writeFileBlock(blk);
                    if (bytesRemaining <= 0)
                        break;
                }
                channel.closeFile();
                fs_local.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }




            // channel.SearchResult(res);
        }
        
        void startListening() {
            int localPort = 4040;
            while (true)
            {
                try
                {
                    string endpoint = "http://localhost:" + localPort + "/ICommunicator";
                    CreateRecvChannel(endpoint);
                    // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
                    
                    
                    OnListenerCreated.Invoke(endpoint);
                    return;
                }
                catch (Exception ex)
                {
                    localPort++;
                    if ((localPort - 4040) > 10000)
                    {
                        OnListenerCreated.Invoke("");
                        return;
                    }
                    Console.WriteLine(ex.Message);
                }
            }

        }

       
        public void closeFile()
        {
            fs.Close();
        }

        public void DownloadRequest(WCF_Peer_Comm.FileInfo fileInfo)
        {
            downloadReqBlockingQ.enQ(fileInfo);
        }

        public bool openFileForWrite(string name)
        {
            try
            {
                string writePath = Path.Combine("./", name);
                fs = File.OpenWrite(writePath);
                return true;
            }
            catch (Exception ex)
            {
                //lastError = ex.Message;
                return false;
            }
        }

        public void SearchResult(List<string> result)
        {

            searchResBlockingQ.enQ(result);
        }

        public bool writeFileBlock(byte[] block)
        {
           try
            {
                fs.Write(block, 0, block.Length);
                return true;
            }
            catch (Exception ex)
            {
                //lastError = ex.Message;
                return false;
            }
        }

        public List<string> getResult()
        {
            return searchResBlockingQ.deQ();
        }

        public WCF_Peer_Comm.FileInfo getDownLoadReq()
        {
            return downloadReqBlockingQ.deQ();
        }
    }
}
