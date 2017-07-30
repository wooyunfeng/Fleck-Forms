using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting;
using Fleck;
using System.Net.Sockets;
using System.Net;  

namespace Fleck_Forms
{
    class Engine : Comm
    {
        private static StreamWriter PipeWriter { get; set; }
        private static bool bLock { get; set; }
        Process pProcess;
        Boolean bEngineRun;
        DateTime EngineRunTime;
        public NewMsg currentMsg;
        public Queue OutputEngineQueue;
        object container = null;
        Producer producer = null;
        HttpChannel _channel;
        ConcurrentQueue<NewMsg> queueContainer;
        public bool bJson { get; set; }
        public List<Consumer> customerlist { get; set; }
        Thread myTCPThread;

        public int getMsgQueueCount()
        {
            int count = 0;

            count = queueContainer.Count;

            return count;
        }

        public void OutputEngineQueueEnqueue(string[] line, bool save = false)
        {
            if (OutputEngineQueue == null || line == null || line.Length == 0)
            {
                return;
            }
            if (save)
            {
                WriteInfo(line.ToString());
            }
            lock (OutputEngineQueue)
            {
                OutputEngineQueue.Enqueue(line);
            }
        }

        public object OutputEngineQueueDequeue()
        {
            if (OutputEngineQueue == null)
            {
                return null;
            }
            lock (OutputEngineQueue)
            {
                return (string [])OutputEngineQueue.Dequeue();
            }
        }

        public void Start()
        {
            OutputEngineQueue = new Queue();
            queueContainer = new ConcurrentQueue<NewMsg>();
            producer = new Producer(queueContainer);
            bLock = false;
            currentMsg = null;
            //启动引擎线程
            Init();        
            //启动OnTCP
            OnTCP();
        }

        //查找进程、结束进程
        public void checkwerfault()
        {
            Process[] pro = Process.GetProcesses();//获取已开启的所有进程
            //遍历所有查找到的进程
            for (int i = 0; i < pro.Length; i++)
            {
                //判断此进程是否是要查找的进程
                if (pro[i].ProcessName.ToString().ToLower() == "werfault")
                {
                    pro[i].Kill();//结束进程
                }
            }
        }        

        public void OnOpen(IWebSocketConnection socket)
        {
            var role = new Role(socket);
            SQLite_Login(role.GetAddr());
            user.Add(role);
        }

        public void OnClose(IWebSocketConnection socket)
        {
            var role = user.GetAt(socket);
            SQLite_Logout(role.GetAddr());
            user.Remove(socket);
        }

        public void OnMessage(IWebSocketConnection socket, string message)
        {
            string strAddr = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort.ToString();
            string[] param = { DateTime.Now.ToLongTimeString(), strAddr, message };
            SQLite_InsertCommand(param);

            //过滤命令
            if (message.IndexOf("queryall") != -1)
            {
                NewMsg msg = new NewMsg(socket, message);

                string str = DealQueryallMessage(msg.GetCommand());
                string[] msgs = { strAddr, str };
                OutputEngineQueueEnqueue(msgs);
                socket.Send(str);
            }
            else if (message.IndexOf("position") != -1)
            {
                DealPositionMessage(socket, message);
            }
        }

        private void DealPositionMessage(IWebSocketConnection socket, string message)
        {
            //记录每个用户的消息队列
            NewMsg msg = new NewMsg(socket,message);
            producer.Product(msg);
        }

        internal object getUserCount()
        {
            return user.allRoles.Count;
        }

        private static byte[] result = new byte[4096];  
        private static int myProt = 8885;   //端口  
        static Socket serverSocket;  

        private void OnTCP()  
        {
            customerlist = new List<Consumer>();     
            //服务器IP地址  
            IPAddress ip = IPAddress.Parse("118.190.46.210");  
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            //Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());  
            //通过Clientsoket发送数据  
            myTCPThread = new Thread(ListenClientConnect);  
            myTCPThread.Start();  
        }  
  
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private  void ListenClientConnect()  
        {  
            while (true)  
            {
                Socket clientSocket = serverSocket.Accept();  
                Consumer consumer = new Consumer(queueContainer, clientSocket);
                Thread.Sleep(100);
                consumer.Consume();                
                Thread receiveThread = new Thread(ReceiveMessage);  
                receiveThread.Start(clientSocket);
                consumer.receiveThread = receiveThread;
                customerlist.Add(consumer);
            }  
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    string info = Encoding.ASCII.GetString(result, 0, receiveNumber);

                    foreach (var r in customerlist.ToList())
                    {
                        if (r.socket == myClientSocket)
                        {
                            if (info == "exit")
                            {
                                customerlist.Remove(r);
                                break;
                            }
                            currentMsg = (NewMsg)r.currentMsg;
                            reciveRemoting(info);
                            r.Recive(info);
                            break;
                        }
                    }
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
        
        private void reciveRemoting(object info)
        {
            string line = info.ToString();
            string[] sArray = line.Split(' ');
            if (currentMsg != null)
            {
                currentMsg.Send(line);
                if (line.IndexOf("bestmove") != -1)
                {
                    string[] msgs = { currentMsg.GetAddr(), line };
                    OutputEngineQueueEnqueue(msgs);
                    SQLite_UpdateCommand(1, line, currentMsg.GetAddr(), currentMsg.GetMessage());
                }
            }                                    
        }


        internal void Close()
        {
            if (myTCPThread != null)
            {
                serverSocket.Close();
                myTCPThread.Abort();
            }
            foreach (var r in customerlist.ToList())
            {
                r.receiveThread.Abort();
                r.consumethread.Abort();
                r.bRun = false;
            }
        }
    }
}
