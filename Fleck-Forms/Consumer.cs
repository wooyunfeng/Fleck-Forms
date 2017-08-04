using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Fleck_Forms
{
    public class Consumer
    {
        ConcurrentQueue<NewMsg> inputcontainer = null;
        ConcurrentQueue<string[]> outputcontainer = null;
        Comm comm = null;
        NewMsg currentMsg;
        public bool bRun = true;
        bool bdealing = false;

        int dealCount = 0;
        public Socket socket = null;
        public DateTime logintime = new DateTime();
        public DateTime lastdealtime = new DateTime();
        private static byte[] result = new byte[4096];  

        //得到一个容器
        public Consumer(object containerI, object containerO, object comm, Socket socket)
        {
            this.inputcontainer = (ConcurrentQueue < NewMsg > )containerI;
            this.outputcontainer = (ConcurrentQueue < string [] > )containerO;
            this.comm = (Comm)comm;
            this.logintime = DateTime.Now;
            this.socket = socket;
        }

        public string getName()
        {
            try
            {
                return socket.RemoteEndPoint.ToString();
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        public void Start()
        {
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(socket);
        }
        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            string info;
            int receiveNumber;
            while (bRun)
            {
                try
                {
                    //通过clientSocket接收数据  
                    receiveNumber = myClientSocket.Receive(result);
                    info = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    string[] reviceMsg = info.Split('#');
                    foreach (string s in reviceMsg)
                    {
                        if (s.Length > 0)
                        {
                            Recive(s);
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    myClientSocket.Close();
                    break;
                }
            }
        }  

        //定义一个消费的方法
        public void Consumption()
        {
            //消费掉容器中的一个物品
            inputcontainer.TryDequeue(out currentMsg);
            if (currentMsg != null)
            {
                SendToClient(currentMsg.GetMessage());
                dealCount++;
                lastdealtime = DateTime.Now;
                bdealing = true;
            }                           
        }

        public int getDealCount()
        {
            return dealCount;
        }

        public string getlastdealtime()
        {
            return lastdealtime.ToString();
        }

        private bool redisContainsKey(string p)
        {
            throw new NotImplementedException();
        }

        public void SendToClient(string info)
        {
            socket.Send(Encoding.ASCII.GetBytes(info));  
        }

        public int getCount()
        {
            return inputcontainer.Count;
        }

        private bool checkTimeOut()
        {
            if (bdealing)
            {
                DateTime currentTime = System.DateTime.Now;
                TimeSpan span = currentTime.Subtract(lastdealtime);
                if (span.Seconds > 30)
                {
                    bdealing = false;
                    return true;
                }
            }
            return false;
        }
        public void Recive(string message)
        {
            switch (message)
            {
                case "list":
                    {
                        //如果正在处理进行超时判断，否则进行新的请求
                        if (bdealing)
                        {
                            checkTimeOut();
                        }
                        else
                        {
                            Consumption();
                        }                       
                    }
                    break;
                case "exit":
                    {
                        bRun = false;                        
                    }
                    break;
                case "restart":
                    {
                        SendToClient(currentMsg.GetMessage());                        
                    }
                    break;
                default:
                    {
                        currentMsg.Send(message);
                        if (message.IndexOf("bestmove") != -1)
                        {
                            string[] msgs = { currentMsg.GetAddr(), getName(), message };
                            outputcontainer.Enqueue(msgs);
                            comm.SQLite_UpdateCommand(1, message, currentMsg.GetAddr(), currentMsg.GetMessage());
                            bdealing = false;
                        }
                        else
                        {
                            comm.setItemToList(currentMsg.GetCommand(), message);                            
                        }
                    }
                    break;
            } 
        }

        internal bool check()
        {
            if (getName() == "" || dealCount == 0)
            {
                return false;
            }
            return bRun;
        }
    }

    public class Producer
    {
        object container = null;

        //得到一个容器
        public Producer(object container)
        {
            this.container = container;
        }

        //定义一个生产物品的方法装入容器
        public void Product(object message)
        {
            //创建一个新物品装入容器
            ConcurrentQueue<NewMsg> queue = (ConcurrentQueue<NewMsg>)container;
            queue.Enqueue((NewMsg)message);
        }
    }
}
