using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Project4Prototype
{
    class Program
    {
      
        static void Main(string[] args)
        {
            //sets the endpoint for clients
            string endpoint = "http://localhost:8080/ICommunicator";

            // starts new thread using the endpoint
            WCF_Peer_Comm.Receiver recvr;
            recvr = new WCF_Peer_Comm.Receiver();
            recvr.CreateRecvChannel(endpoint);

            Console.WriteLine("Running");
            Console.ReadKey();
        }
    }
}
