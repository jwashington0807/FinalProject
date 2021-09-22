


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
        void Search(FileInfo fileInfo, bool isName, bool isKey, bool isDesc);

        [OperationContract]
        void LogRecord(string user, string name, string action);

        [OperationContract]
        string Login(string username, string password);

        [OperationContract]
        List<string> GetAuditResults(string user, string text, bool isAdmin);

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
        public string File;

        [DataMember]
        public string Description;

        [DataMember]
        public string Keywords;

        public FileInfo(string newName, string fileName, string keys, string description, string newEndpoint)
        {
            Name = newName;
            Endpoint = newEndpoint;
            File = fileName;
            Keywords = keys;
            Description = description;

        }
    }
}
