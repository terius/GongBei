using System;
using System.Xml.Serialization;

namespace GongBei.Model
{
    [XmlRoot("Envelope")]
    [Serializable]
    public class ctSortingResult
    {
        public ctSortingResult()
        {
            Header = new Header2();
            Body = new Body2();
            Header.MessageName = "ctSortingResult";
            Header.MessageTime = DateTime.Now;
        }
        public Header2 Header { get; set; }
        public Body2 Body { get; set; }
    }
    public class Header2
    {
        public  string MessageName { get; set; }
        public DateTime MessageTime { get; set; }
    }

    public class Body2
    {
        public string Barcode { get; set; }
        public string BillNo { get; set; }
        public string Result { get; set; }
    }

   
}
