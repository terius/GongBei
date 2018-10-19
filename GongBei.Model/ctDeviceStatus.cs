using System;
using System.Xml.Serialization;

namespace GongBei.Model
{
    [Serializable]
    [XmlRoot("Envelope")]
    public class ctDeviceStatus
    {
        public Header Header { get; set; }
        public ctDeviceStatusBody Body { get; set; }
    }

    public class ctDeviceStatusBody
    {
        public int Status { get; set; }
    }
}
