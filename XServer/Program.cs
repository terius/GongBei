using GongBei.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TeriusCommon.Helpers;

namespace XServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口 
        static Socket serverSocket;
        static void Main(string[] args)
        {
            //服务器IP地址 
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口 
            serverSocket.Listen(10);    //设定最多10个排队连接请求 
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据 
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary> 
        /// 监听客户端连接 
        /// </summary> 
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                //  string msg = "NTCTTP/1.0 NOTIFY\r\nContent-Type:text/xml\r\nContent-Length:1000\r\n\r\n<?xml version=\"1.0\" encoding=\"utf-8\"?><Envelope><Header><MessageName>ctDeviceStatus</MessageName><MessageTime>2018-09-10T00:41:24+08:00</MessageTime></Header><Body><Status>4</Status></Body></Envelope>";
                string msg = CreateSendData();
                clientSocket.Send(Encoding.ASCII.GetBytes(msg));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }

        static string CreateSendData()
        {
            ctConclusionResult data = new ctConclusionResult();
            data.Header = new Header { MessageName = "ctConclusionResult", MessageTime = DateTime.Now };
            data.Body = new ctConclusionResultBody();
            data.Body.Barcode = "jlekrtjerjtkerjtkekrtlerktjklertjkeklrt";
            data.Body.BillNo = "jlekrtjerjtkerjtkekrtlerktjklertjkeklrt";
            data.Body.Department = 1;
            data.Body.JudgeCategory = 5;
            data.Body.JudgeResult = "CCC";
            data.Body.JudgeTime = DateTime.Now;
            data.Body.PRN = "jlekrtjerjtkerjtkekrtlerktjklertjkeklrt";
            data.Body.UserId = "jlekrtjerjtkerjtkekrtlerktjklertjkeklrt";
            var body = XmlHelper.Serializer(data);
            string head = string.Format("NTCTTP/1.0 NOTIFY\r\nContent-Type:text/xml\r\nContent-Length:{0}\r\n\r\n", 1000);
            return head + body;

        }

        /// <summary> 
        /// 接收消息 
        /// </summary> 
        /// <param name="clientSocket"></param> 
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据 
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}
