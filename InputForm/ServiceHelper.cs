using System.Data;
using TeriusCommon.Helpers;

namespace InputForm
{
    public class ServiceHelper
    {
        static ServiceReference1.GBCustoms_EHS_WebServiceSoapClient client = new ServiceReference1.GBCustoms_EHS_WebServiceSoapClient();
        public static DataSet GetData(string voyageNo, string userName, string pwd, string code)
        {
            string[][] err = new string[][] { };
            //991601021011001
            var data = client.GBCustoms_ServiceEntrance(voyageNo, userName, pwd, code, out err);
            //  string file = "d:\\20181201.xml";
            // data.WriteXml(file);
            if (err != null && err.Length > 0)
            {
                string errStr = ConvertHelper.ToString(err);
                if (!string.IsNullOrWhiteSpace(errStr))
                {
                    FileHelper.WriteLog(errStr);
                }

            }
            return data;
        }
    }
}
