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
using Newtonsoft.Json;


namespace Fleck_Forms
{
    class Engine 
    {
        Producer producer;
        ConcurrentQueue<NewMsg> inputContainer;
        ConcurrentQueue<string[]> outputContainer;
        public Comm comm;
        public List<Consumer> customerlist { get; set; }
       
        bool bRun = true;

        public int getMsgQueueCount()
        {
            int count = 0;

            count = inputContainer.Count;

            return count;
        }

        public int getOutputContainerCount()
        {
            return outputContainer.Count;
        }

        public void OutputEngineQueueEnqueue(string[] line, bool save = false)
        {
            if (save)
            {
                comm.WriteInfo(line.ToString());
            }
            outputContainer.Enqueue(line);
        }

        public object OutputEngineQueueDequeue()
        {
            string[] re;
            outputContainer.TryDequeue(out re);
            return re;
        }

        public void Start()
        {
            inputContainer = new ConcurrentQueue<NewMsg>();
            outputContainer = new ConcurrentQueue<string[]>();
            producer = new Producer(inputContainer);
            comm = new Comm();
            comm.Init();
            //启动OnTCP,处理引擎信息
            OnTCP();
        }

        public void OnOpen(IWebSocketConnection socket)
        {
            var role = new Role(socket);
            comm.user.Add(role);
        }

        public void OnClose(IWebSocketConnection socket)
        {
            var role = comm.user.GetAt(socket);
            if (role != null)
            {
                comm.user.Remove(socket);
            }           
        }


        private void DealQueryallMessage(NewMsg msg)
        {
            string strQueryall = comm.DealQueryallMessage(msg.GetBoard());
            if (strQueryall != "" && strQueryall != "unknown")
            {
                string sendmsg = msg.Send(strQueryall);
            }
        }

        private void DealMoveMessage(NewMsg msg)
        {

            var role_from = new Role(msg.connection);
            foreach (var role in comm.user.allRoles.ToList())
            {
                if (msg.connection != role.connection)
                {
                    role.Send(msg.message);
                }                
            }
        }

        private void DealOpenBookMessage(NewMsg msg)
        {
            List<string> list = (List<string>)comm.redis.getOpenBook(msg.zobristKey);
            string result = "";
            foreach (var value in list)
            {
                result += value+"|";
            }
            msg.SendOpenbook(result);
        }

        private void DealPositionMessage(NewMsg msg)
        {
            if (msg.GetCommand() == null)
            {
                comm.WriteInfo(msg.GetAddr() + "  " + msg.GetMessage());
                return;
            }

            //查库
            if (comm.bRedis)
            {
                if (comm.getItemFromList(msg))
                {
                    string sendmsg = comm.getbestmoveFromList(msg);
                    string[] msgs = { msg.GetAddr(), "reids", sendmsg };
                    OutputEngineQueueEnqueue(msgs);
                    return;
                }
            }

            producer.Product(msg);

        }

        internal object getUserCount()
        {
            return comm.user.allRoles.Count;
        }
      
        private static byte[] result = new byte[4096];
        //private static int myProt = 8885;   //端口  
        static Socket serverSocket;

        private void OnTCP()
        {
            customerlist = new List<Consumer>();
            //服务器IP地址  
            IPAddress ip = IPAddress.Parse(Setting.tcpServerAddress);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, Setting.tcpServerPort));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            //Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());  
            //通过Clientsoket发送数据  
            Thread thread = new Thread(ListenClientConnect);
            thread.Start();
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private void ListenClientConnect()
        {
            while (bRun)  
            {
                try
                {
                    Socket clientSocket = serverSocket.Accept();
                    Consumer consumer = new Consumer(inputContainer, outputContainer, comm, clientSocket);
                    consumer.Start();
                    customerlist.Add(consumer);
                }
                catch (System.Exception ex)
                {
                    string[] msgs = { "", "", ex.Message };
                    OutputEngineQueueEnqueue(msgs);
                }                
            }  
        }

        public void Close()
        {
            if (customerlist != null)
            {
                foreach (var customer in customerlist.ToList())
                {
                    customer.reset();
                    customerlist.Remove(customer);
                }
            }
            serverSocket.Close();
            bRun = false;
        }


        internal void OnMessage(NewMsg msg)
        {
            string message = msg.GetMessage();
            string[] param = { DateTime.Now.ToLongTimeString(), msg.GetAddr(), message };
            //过滤命令
            try
            {
                if (message.IndexOf("queryall") != -1)
                {
                    DealQueryallMessage(msg);
                }
                else if (message.IndexOf("position") != -1)
                {
                    DealPositionMessage(msg);                    
                }
                else if (message.IndexOf("move") != -1)
                {
                    DealMoveMessage(msg);
                }
                else if (message.IndexOf("openbook") != -1)
                {
                    DealOpenBookMessage(msg);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }       
    }
}
