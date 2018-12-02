using System;

namespace TeriusCommon.Helpers
{
    public class ConvertHelper
    {
        public static DateTime? ToSafeNullableDateTime(object ob)
        {
            if (ob == null)
            {
                return null;
            }

            DateTime dtTemp = DateTime.Parse("2000-01-01");
            if (DateTime.TryParse(ob.ToString(), out dtTemp))
            {
                return dtTemp;
            }
            return null;
        }

        public static decimal? ToSafeNullabledecimal(object ob)
        {
            if (ob == null)
            {
                return null;
            }

            decimal itemp = 0;
            if (decimal.TryParse(ob.ToString(), out itemp))
            {
                return itemp;
            }
            return null;
        }

        public static string ToString(string[][] strTwo)
        {
            if (strTwo ==null)
            {
                return "";
            }
            string str = "";
            foreach (string[] item in strTwo)
            {
                foreach (string item2 in item)
                {
                    if (str == "")
                    {
                        str = item2;
                    }
                    else
                    {
                        str += "," + item2;
                    }
                }
            }
            return str;
        }


    }
}
