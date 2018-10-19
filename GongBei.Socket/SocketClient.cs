using GongBei.Infrastructure;
using GongBei.Model;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeriusCommon.Helpers;

namespace GongBei.Socket
{
    public class SocketClient
    {
        readonly string serverIP = System.Configuration.ConfigurationManager.AppSettings["serverIP"];
        readonly int serverPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["serverPort"]);
        readonly int heartTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["heartTime"]);
        static EasyClient client;
        public Action<ReceieveMessage> OnReceiveData;
        bool isConnected = false;
        Thread headThread;
        Stopwatch watch;
        public SocketClient()
        {
            client = new EasyClient();
            client.Initialize(new MyReceiveFilter(), (response) =>
            {
                OnReceiveData(response);
            });

        }

        private string CreatHead(int length)
        {
            string head = "NTCTTP/1.0 NOTIFY\r\nContent-Type:text/xml\r\nContent-Length:" + length + "\r\n\r\n";
            return head;
        }

        private void CreateHeart()
        {
            
            try
            {
                ctHeartBeatRequest heart = new ctHeartBeatRequest();
                watch = Stopwatch.StartNew();
                while (true)
                {
                    if (watch.Elapsed.TotalSeconds > heartTime)
                    {
                        Console.WriteLine("心跳包获取超时，开始重连");
                        ConnectToServer().Wait(TimeSpan.FromSeconds(10));
                    }
                    if (isConnected)
                    {
                        heart.Header.MessageTime = DateTime.Now;
                        Send(heart).Wait();
                    }
                    //else
                    //{
                    //    ConnectToServer().Wait(TimeSpan.FromSeconds(10));
                    //}
                    Thread.Sleep(30000);
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteLog(ex.ToString());
            }


        }

        public void Start()
        {
            ConnectToServer().Wait(TimeSpan.FromSeconds(10));
            SendHeart();
        }

        private async Task ConnectToServer()
        {
            isConnected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(serverIP), serverPort));
            Console.WriteLine("连接Socket" + (isConnected ? "成功！" : "失败！稍后会自动重连"));
        }

        private void SendHeart()
        {
            try
            {
                while (true)
                {
                    if (headThread == null ||
                        headThread.ThreadState == System.Threading.ThreadState.Unstarted ||
                        headThread.ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        headThread = new Thread(new ThreadStart(CreateHeart));
                        headThread.Start();
                        break; ;
                    }
                    Thread.Sleep(100);
                }

            }
            catch (Exception ex)
            {
                FileHelper.WriteLog(ex.ToString());
                throw ex;
            }
        }

        public void RestartTime()
        {
            watch.Restart();
        }


        public bool CheckIsConnected()
        {
            //if (!client.IsConnected)
            //{
            //    await ConnectToServer();
            //}
            return isConnected;
        }

        public async Task<bool> Send(string message)
        {
            isConnected = client.IsConnected;
            if (isConnected)
            {
                var m = CreatHead(message.Length) + message;
                client.Send(Encoding.UTF8.GetBytes(m));
                FileHelper.WriteLog(m, "发送数据");
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> Send<T>(T t) where T : class, new()
        {
            string message = XmlHelper.Serializer(t);
            return await Send(message);
        }

        class MyReceiveFilter : FixedHeaderReceiveFilter<ReceieveMessage>
        {
            int bodyLength;
            public MyReceiveFilter() : base(60)
            {
            }

            public override ReceieveMessage ResolvePackage(IBufferStream bs)
            {
                ReceieveMessage res = new ReceieveMessage();
                //var h = new byte[64];
                //bs.Read(h, 0, 64);
                //var mmm = Encoding.UTF8.GetString(h);
                bs.Skip(64);
                var bodyBytes = new byte[bodyLength];
                bs.Read(bodyBytes, 0, bodyLength);
                res.Length = bodyLength;
                res.DataStr = Encoding.UTF8.GetString(bodyBytes);
                res.ReceiveDataType = GetReceiveType(res.DataStr);
                return res;
            }

            private ReceiveDataType GetReceiveType(string data)
            {
                var value = XmlHelper.GetNodeValue(data, "MessageName");
                if (string.IsNullOrWhiteSpace(value))
                {
                    return ReceiveDataType.None;

                }
                value = value.ToLower();
                switch (value)
                {
                    case "ctbarcodemessage":
                        return ReceiveDataType.ctBarcodeMessage;
                    case "ctconclusionresult":
                        return ReceiveDataType.ctConclusionResult;
                    case "ctdevicestatus":
                        return ReceiveDataType.ctDeviceStatus;
                    case "ctheartbeatresponse":
                        return ReceiveDataType.ctHeartBeatResponse;
                    default:
                        break;
                }
                return ReceiveDataType.None;
            }

            protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
            {
                var headBytes = new byte[length];
                bufferStream.Read(headBytes, 0, length);
                var str = Encoding.UTF8.GetString(headBytes);
                bodyLength = int.Parse(str.Substring(str.LastIndexOf(":") + 1).Replace("\r","").Replace("\n",""));

                return bodyLength + 4;
            }
        }


    }



    public class ReceieveMessage : IPackageInfo
    {
        public int Length { get; set; }
        public string DataStr { get; set; }

        public ReceiveDataType ReceiveDataType { get; set; }
    }
}
