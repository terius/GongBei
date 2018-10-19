using System;
using System.Xml.Serialization;

namespace GongBei.Model
{
    [Serializable]
    [XmlRoot("Envelope")]
    public class ctHeartBeatRequest
    {
        public ctHeartBeatRequest()
        {
            Header = new Header();
            Body = new ctHeartBeatRequestBody();
            Header.MessageName = "ctHeartBeatRequest";
            Body.Source = "SS";
            Body.Target = "CT";
        }
        public Header Header { get; set; }
        public ctHeartBeatRequestBody Body { get; set; }
    }

    public class ctHeartBeatRequestBody
    {
        public string Source { get; set; }
        public string Target { get; set; }
    }
}
