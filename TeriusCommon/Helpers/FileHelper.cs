using System;
using System.IO;
using System.Text;
using System.Threading;

namespace TeriusCommon.Helpers
{
    public class FileHelper
    {
        const int LOCK = 500; //申请读写时间
        const int SLEEP = 100; //线程挂起时间
        static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        private static readonly int SaveLog = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["saveLog"]);

        public static void WriteLog(string msg, string title = "") //写入文件
        {
            if (SaveLog != 1)
            {
                return;
            }
            readWriteLock.EnterWriteLock();
            try
            {

                string path = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                if (!File.Exists(path))
                {
                    FileStream fs1 = File.Create(path);
                    fs1.Close();
                    Thread.Sleep(10);
                }

                using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                {
                    string txt = string.Format("-------------------{0} 【{1}】-------------------\r\n{2}\r\n\r\n",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), title, msg);
                    sw.WriteLine(txt);
                    sw.Flush();
                    sw.Close();
                }
                //  Thread.Sleep(SLEEP);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }


        public static string ReadTxt(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return text;
        }

        public static void MoveFile(FileInfo file,string newPath)
        {
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            file.MoveTo(Path.Combine(newPath, file.Name));
        }
    }
}
