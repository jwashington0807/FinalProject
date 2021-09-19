using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using WCF_Peer_Comm;
namespace Client
{
    [ServiceContract]
    public interface IPeerCommunicator
    {
        [OperationContract]
        void SearchResult(List<string> result);
        // used only locally so not exposed as service method
        List<string> getResult();
        [OperationContract]
        void DownloadRequest(FileInfo fileInfo);
        /*---< called to open a file on Receiver >-----*/
        [OperationContract]
        bool openFileForWrite(string name);
        /*----< write a block received from Sender >----------*/
        [OperationContract]
        bool writeFileBlock(byte[] block);
        /*----< close file >-----------------------*/
        [OperationContract(IsOneWay = true)]
        void closeFile();

    }

}
