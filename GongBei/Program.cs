using GongBei.DB;
using GongBei.Model;
using GongBei.Socket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TeriusCommon.Helpers;

namespace GongBei
{
    class Program
    {

        static SocketClient client;
        static readonly string sendPath = System.Configuration.ConfigurationManager.AppSettings["sendPath"];
        static readonly string sendOutPath = System.Configuration.ConfigurationManager.AppSettings["sendOutPath"];
        static readonly string conclusionResultPath = System.Configuration.ConfigurationManager.AppSettings["conclusionResultPath"];
        static readonly int scanTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["scanTime"]);
        static SQLAction sqlAction;
        static string errorPath;

        //private static byte[] result = new byte[1024];
        //static void s2()
        //{

        //    //设定服务器IP地址 
        //    IPAddress ip = IPAddress.Parse("127.0.0.1");
        //    System.Net.Sockets.Socket clientSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    try
        //    {
        //        clientSocket.Connect(new IPEndPoint(ip, 60058)); //配置服务器IP与端口 
        //        Console.WriteLine("连接服务器成功");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("连接服务器失败，请按回车键退出！");
        //        return;
        //    }
        //    //通过clientSocket接收数据 
        //    int receiveLength = clientSocket.Receive(result);
        //    string rmsg = Encoding.UTF8.GetString(result, 0, receiveLength);
        //    Console.WriteLine("接收服务器消息：{0}", rmsg);
        //    //通过 clientSocket 发送数据 
        //    for (int i = 0; i < 10; i++)
        //    {
        //        try
        //        {
        //            Thread.Sleep(1000);    //等待1秒钟 
        //            string sendMessage = "client send Message Hellp" + DateTime.Now;
        //            clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
        //            Console.WriteLine("向服务器发送消息：{0}" + sendMessage);
        //        }
        //        catch
        //        {
        //            clientSocket.Shutdown(SocketShutdown.Both);
        //            clientSocket.Close();
        //            break;
        //        }
        //    }
        //    Console.WriteLine("发送完毕，按回车键退出");
        //    Console.ReadLine();

        //}


        static void Main(string[] args)
        {
            try
            {
                Init();
                ReadFileProcess();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                FileHelper.WriteLog(ex.ToString());
            }

        }



        private static void ReadFileProcess()
        {
            new Thread(() =>
            {
                string barcode = "";
                string billNo = "";
                bool sendOk = false;
                StringBuilder sbMsg = new StringBuilder();
                string[] files = null;
                string errorFile = "";
                string sendOutFilePath = "";
                ctBarcodeMessage sendData = null;
                string readFileString = "";
                while (true)
                {
                    try
                    {
                        if (client.CheckIsConnected())
                        {
                            sendOutFilePath = FileHelper.CombineFile(sendOutPath, DateTime.Now.ToString("yyyy-MM-dd"));
                            files = Directory.GetFiles(sendPath);
                            foreach (string fileFullname in files)
                            {
                                errorFile = fileFullname;
                                sbMsg.Clear();
                                readFileString = FileHelper.ReadTxt(fileFullname);
                                sbMsg.AppendLine("获取到文件：" + fileFullname + " 内容：" + readFileString);
                                barcode = readFileString.Split(',')[1].Trim();
                                billNo = readFileString.Split(',')[0].Trim();
                                sendData = CreateSendData(barcode, billNo);
                                sbMsg.Append("开始发送数据.....");
                                sendOk = false;
                                try
                                {
                                    sendOk = client.Send(sendData).Result;
                                }
                                catch (Exception ex)
                                {
                                    FileHelper.WriteLog("socket错误," + ex.ToString());
                                }
                                if (sendOk)
                                {
                                    FileHelper.MoveFile(fileFullname, sendOutFilePath);
                                }
                                sbMsg.Append(sendOk ? "成功" : "失败");
                                FileHelper.WriteLog(sbMsg.ToString());
                                Thread.Sleep(20);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileHelper.WriteLog(ex.ToString(), "读取错误,读取文件：" + errorFile);
                        if (errorFile != "")
                        {
                            var newPath = FileHelper.CombineFile(errorPath, DateTime.Now.ToString("yyyy-MM-dd"));
                            Thread.Sleep(20);
                            FileHelper.MoveFile(errorFile, newPath);
                        }

                    }

                    Thread.Sleep(scanTime);
                }
            }).Start();

        }

        /// <summary>
        /// 初始化
        /// </summary>
        static void Init()
        {
            Console.WriteLine("系统开始初始化....");
            CheckPath();
            sqlAction = new SQLAction();
            client = new SocketClient();
            client.OnReceiveData = (res) =>
            {
                switch (res.ReceiveDataType)
                {
                    case Infrastructure.ReceiveDataType.None:
                        break;
                    case Infrastructure.ReceiveDataType.ctHeartBeatResponse:
                        client.RestartTime();
                        break;
                    case Infrastructure.ReceiveDataType.ctBarcodeMessage:
                        break;
                    case Infrastructure.ReceiveDataType.ctConclusionResult:
                        SavectConclusionResult(res);
                        break;
                    case Infrastructure.ReceiveDataType.ctDeviceStatus:
                        SavectDeviceStatus(res);
                        break;
                    default:
                        break;
                }
                Console.WriteLine("获取到返回信息：" + res.ReceiveDataType.ToString());
                FileHelper.WriteLog(res.DataStr, "收到数据");
            };
            client.Start();
        }

        private static void CheckPath()
        {
            if (!Directory.Exists(sendPath))
            {
                Directory.CreateDirectory(sendPath);
            }

            if (!Directory.Exists(sendOutPath))
            {
                Directory.CreateDirectory(sendOutPath);
            }

            errorPath = AppDomain.CurrentDomain.BaseDirectory + "ErrorFiles";
            if (!Directory.Exists(errorPath))
            {
                Directory.CreateDirectory(errorPath);
            }
        }

        private static void SavectConclusionResult(ReceieveMessage res)
        {
            SaveToFile(res.DataStr);
            var info = XmlHelper.Deserialize<ctConclusionResult>(res.DataStr);
            sqlAction.SavectConclusionResultData(info);
        }

        private static void SaveToFile(string dataStr)
        {
            var path = FileHelper.CombineFile(conclusionResultPath, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = FileHelper.CombineFile(path, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml");
            XmlHelper.SaveToXMLFile(dataStr, fileName);
        }

        private static void SavectDeviceStatus(ReceieveMessage res)
        {
            var info = XmlHelper.Deserialize<ctDeviceStatus>(res.DataStr);
            sqlAction.SavectDeviceStatusData(info);
        }

        static ctBarcodeMessage CreateSendData(string barcode, string billNo)
        {
            ctBarcodeMessage info = new ctBarcodeMessage();
            info.Body.Barcode = barcode;
            info.Body.BillNo = billNo;
            return info;
        }
    }


}
