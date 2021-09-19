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

            string endpoint = "http://localhost:8080/ICommunicator";

            WCF_Peer_Comm.Receiver recvr;
           
            recvr = new WCF_Peer_Comm.Receiver();
            recvr.CreateRecvChannel(endpoint);

           
            Console.WriteLine("Running");
            Console.ReadKey();
            

        }
    }
}
