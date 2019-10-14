using System;
using System.Xml.Serialization;

namespace GongBei.Model
{
    [XmlRoot("Envelope")]
    [Serializable]
    public class ctConclusionResult
    {
        public Header Header { get; set; }
        public ctConclusionResultBody Body { get; set; }
    }
  

    public class ctConclusionResultBody
    {
        public string Barcode { get; set; }
        public string BillNo { get; set; }
        public string PRN { get; set; }
        public string UserId { get; set; }
        public int Department { get; set; }
        public string JudgeResult { get; set; }
        public int? ResultMask { get; set; }
        public int JudgeCategory { get; set; }
        public DateTime JudgeTime { get; set; }
    }
}
