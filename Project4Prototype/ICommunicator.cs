


using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

namespace WCF_Peer_Comm
{
    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract]
        void NewFile(FileInfo fileInfo);
        // used only locally so not exposed as service method

        [OperationContract]
        void Search(FileInfo fileInfo);

        FileInfo GetMessage();
    }
    [DataContract]
    public class FileInfo
    {
        [DataMember]
        public string Name;

        [DataMember]
        public string Endpoint;

        public FileInfo(string newName, string newEndpoint)
        {
            Name = newName;
            Endpoint = newEndpoint;
        }
    }
}
