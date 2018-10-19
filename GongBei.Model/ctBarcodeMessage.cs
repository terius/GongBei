using System;
using System.Xml.Serialization;

namespace GongBei.Model
{
    [XmlRoot("Envelope")]
    [Serializable]
    public class ctBarcodeMessage
    {
        public ctBarcodeMessage()
        {
            Header = new Header();
            Body = new Body();
            Header.MessageName = "ctBarcodeMessage";
            Header.MessageTime = DateTime.Now;
        }
        public Header Header { get; set; }
        public Body Body { get; set; }
    }
    public class Header
    {
        public  string MessageName { get; set; }
        public DateTime MessageTime { get; set; }
    }

    public class Body
    {
        public string Barcode { get; set; }
        public string BillNo { get; set; }
    }

   
}
