/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.2 : 01 Nov 11
 * - Removed unintended local declaration of ServiceHost in Receiver's 
 *   CreateReceiveChannel function
 * ver 2.1 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * - added send thread to keep clients from blocking on slow sends
 * - added retries when creating Communication channel proxy
 * - added comments to clarify what code is doing
 * ver 2.0 : 06 Nov 08
 * - added close functions that close the service and receive channel
 * ver 1.0 : 14 Jul 07
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Client;
using System.Threading;
using System.Collections.Concurrent;
using SWTools;
using System.IO;

namespace WCF_Peer_Comm
{
    /////////////////////////////////////////////////////////////
    // Receiver hosts Communication service used by other Peers
    [ServiceBehavior(IncludeExceptionDetailInFaults =true
 )]
    public class Receiver : ICommunicator
    {
        static BlockingQueue<FileInfo> newFileBlockingQ = null;
        static BlockingQueue<FileInfo> searchFileBlockingQ = null;
        static ConcurrentDictionary<string, ConcurrentBag<string>> indexedFiles;
        ServiceHost service = null;
        void showAll()
        {
            foreach (var item in indexedFiles.Keys)
            {
                var temp = indexedFiles[item];
                foreach (var e in temp)
                {
                    Console.WriteLine(e);
                }
            }


        }
        private void SendResponse(FileInfo fileInfo, List<string> res) {
            string endpoint = fileInfo.Endpoint;
            EndpointAddress baseAddress = new EndpointAddress(endpoint);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IPeerCommunicator> factory
              = new ChannelFactory<IPeerCommunicator>(binding, endpoint);
            IPeerCommunicator channel = factory.CreateChannel();
            channel.SearchResult(res);
        }
        private void startSearchHandlerThread() 
        {
            Thread searchThread = new Thread(() => {
                while (true)
                {
                    FileInfo searchReq = GetSearchRequest();
                    Console.WriteLine(searchReq.Name + "-" + searchReq.Endpoint);
                    List<string> res;
                    if (indexedFiles.ContainsKey(searchReq.Name))
                        res = indexedFiles[searchReq.Name].ToList();
                    else
                    {
                        res = new List<string>();
                        res.Add("Nothing was found");
                    }
                    SendResponse(searchReq, res);
                }
            });
            searchThread.Start();
           }
        private void IndexFiles()
        {
            Thread rcvThrd = new Thread(() => {
                while (true)
                {
                    // get message out of receive queue - will block if queue is empty
                    FileInfo rcvdMsg = GetMessage();
                    Console.WriteLine(rcvdMsg.Name + " " + rcvdMsg.Endpoint);
                    if (indexedFiles.ContainsKey(rcvdMsg.Name))
                    {
                        indexedFiles[rcvdMsg.Name].Add(rcvdMsg.Endpoint);

                    }
                    else
                    {
                        ConcurrentBag<string> temp = new ConcurrentBag<string>();
                        temp.Add(rcvdMsg.Endpoint);
                        indexedFiles[rcvdMsg.Name] = temp;
                    }              // call window functions on UI thread
                    showAll();
                }
            });
            rcvThrd.Start();
        }
        public Receiver()
        {
            if (newFileBlockingQ == null)
                newFileBlockingQ = new BlockingQueue<FileInfo>();
            if (searchFileBlockingQ == null)
                searchFileBlockingQ = new BlockingQueue<FileInfo>();
            if (indexedFiles == null)
                indexedFiles = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            IndexFiles();
            startSearchHandlerThread();
        }

        public void Close()
        {
            service.Close();
        }

        //  Create ServiceHost for Communication service

        public void CreateRecvChannel(string address)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
            service.Open();
        }

        // Implement service method to receive messages from other Peers



        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.
        public FileInfo GetSearchRequest() 
        {
            return searchFileBlockingQ.deQ();
        }

        public FileInfo GetMessage()
        {
            return newFileBlockingQ.deQ();
        }

        public void NewFile(FileInfo fileInfo)
        {
            newFileBlockingQ.enQ(fileInfo);
        }

        public void Search(FileInfo fileInfo)
        {
            //if (indexedFiles.ContainsKey(fileName))
            //    return indexedFiles[fileName].ToList();
            //return null;
            searchFileBlockingQ.enQ(fileInfo);
        }
    }
}
