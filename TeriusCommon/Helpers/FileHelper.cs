using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace TeriusCommon.Helpers
{
    public class FileHelper
    {
        static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        static readonly int SaveLog = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["saveLog"]);
        static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public static void WriteLog(string message, string title = "")
        {
            string msg = (title == "" ? "" : ("---" + title + "---")) + message;
            WriteLogToFile(msg);
        }

        public static void WriteReadLog(string message, string title = "")
        {
            string msg = (title == "" ? "" : ("---" + title + "---")) + message;
            WriteLogToFile(msg,"ReadFiles");
        }

        

        public static void WriteLogToFile(string msg,string filePath= "Logs") //写入文件
        {
            if (SaveLog != 1)
            {
                return;
            }
            readWriteLock.EnterWriteLock();
            try
            {

                string path = basePath + filePath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string txt = string.Format("【{0}】{1}\r\n",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), msg);
                File.AppendAllText(path,txt, Encoding.Default);
                //if (!File.Exists(path))
                //{
                //    FileStream fs1 = File.Create(path);
                //    fs1.Close();
                //    Thread.Sleep(10);
                //}
               
                //using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                //{
                //    string txt = string.Format("-------------------{0}-------------------\r\n{1}",
                //        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
                //    sw.WriteLine(txt);
                //    sw.Flush();
                //    sw.Close();
                //}
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
            SpinWait.SpinUntil(() => false, 100);
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        sb.AppendLine(sr.ReadLine());
                    }
                }
            }
            return sb.ToString().Trim();

        }

        static object obMoveFile = new object();
        public static bool MoveFile(string filePath, string newPath)
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
                    return true;
                }
                catch (Exception ex)
                {
                    WriteReadLog("操作文件：" + filePath + "错误，错误信息：" + ex.Message);
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(filePath);
                    if (File.Exists(filePath))
                    {
                        File.Move(filePath, CombineFile(basePath, "ErrorFiles", fileName));
                    }
                  
                }
                return false;
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
