

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
        void Update(FileInfo fileInfo);

        [OperationContract]
        void Search(FileInfo fileInfo);

        [OperationContract]
        string Login(string username, string password);

        FileInfo GetMessage();
    }
    [DataContract]
    public class FileInfo
    {
        [DataMember]
        public string Name;

        [DataMember]
        public string Endpoint;

        [DataMember]
        public string Description;

        [DataMember]
        public List<string> Keywords;

        public FileInfo(string newName, string newEndpoint)
        {
            Name = newName;
            Endpoint = newEndpoint;

        }
    }
}
