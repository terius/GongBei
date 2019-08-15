using System;
using System.IO;
using System.Runtime.CompilerServices;
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
        static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public static void WriteLog(
        string message,
        string title = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("【" + fileName + " --- " + memberName + " ---- " + lineNumber + "】" + (title == "" ? "" : title));
            sb.AppendLine(message);
            WriteLog2(sb.ToString());
        }

        private static void WriteLog2(string msg) //写入文件
        {
            if (SaveLog != 1)
            {
                return;
            }
            readWriteLock.EnterWriteLock();
            try
            {

                string path = basePath + "Logs";
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
                    string txt = string.Format("-------------------{0}-------------------\r\n{1}\r\n\r\n",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
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
            // string text = File.ReadAllText(fileName);
            //return text;
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        sb.AppendLine(sr.ReadLine());
                    }
                }
            }
            return sb.ToString();

        }

        static object obMoveFile = new object();
        public static void MoveFile(string filePath, string newPath)
        {
            lock (obMoveFile)
            {
                try
                {
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    File.Move(filePath, CombineFile(newPath, Path.GetFileName(filePath)));
                }
                catch (Exception ex)
                {
                    WriteLog("操作文件：" + filePath + "错误，错误信息：" + ex.Message);
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(filePath);
                    File.Move(filePath, CombineFile(basePath, "ErrorFiles", fileName));
                }
            }

        }

        public static string CombineFile(string path, params string[] files)
        {
            string p = path;
            foreach (var file in files)
            {
                p = p.TrimEnd('\\') + "\\" + file;
            }
            return p;
        }

        public static string GetFileNameNotExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
