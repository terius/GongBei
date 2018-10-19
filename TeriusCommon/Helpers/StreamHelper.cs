using System.IO;
using System.Text;

namespace TeriusCommon.Helpers
{
    public class StreamHelper
    {
        public static string StreamToString(Stream stream)
        {
            string rs = "";
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                rs = reader.ReadToEnd();
            }
            return rs;
        }
    }
}
