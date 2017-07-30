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
        object container = null;
        // 通过 _wh 给工作线程发信号
        EventWaitHandle _wh = new AutoResetEvent(false);
        public string name = "";
        public object currentMsg;
        bool bLock = false;
        public bool bRun = true;

        int dealCount = 0;
        public Socket socket = null;
        public DateTime logintime = new DateTime();
        public DateTime lastdealtime = new DateTime();
        public Thread consumethread { get; set; }
        public Thread receiveThread { get; set; }
        private static byte[] result = new byte[4096];  
        //得到一个容器
        public Consumer(object container, string name)
        {
            this.container = container;
            this.name = name;
            logintime = DateTime.Now;
        }

        //得到一个容器
        public Consumer(object container, Socket socket)
        {
            this.container = container;
            this.name = socket.RemoteEndPoint.ToString();
            logintime = DateTime.Now;
            this.socket = socket;
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
                    Recive(info);
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

        //定义一个消费的方法
        public void Consumption()
        {
            //消费掉容器中的一个物品
            ConcurrentQueue<NewMsg> queue = (ConcurrentQueue<NewMsg>)container;
            NewMsg msg;
            queue.TryDequeue(out msg);
            currentMsg = msg;
//             if (redisContainsKey(currentMsg.GetCommand()))
//             {
//                // OutputEngineQueueEnqueue(currentMsg.GetAddr() + " " + getFromList(currentMsg));
//                 _wh.Set();  // 给工作线程发信号
//             }
//             else
            {
                if (msg != null)
                {
                    SendToClient(msg.GetMessage());
                }                
            }

            dealCount++;
            lastdealtime = DateTime.Now;
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

        private void SendToClient(string info)
        {
            socket.Send(Encoding.ASCII.GetBytes(info));  
        }

        public int getCount()
        {
            ConcurrentQueue<NewMsg> queue = (ConcurrentQueue<NewMsg>)container;
            return queue.Count;
        }

        public void Consume()
        {
            consumethread = new Thread(new ThreadStart(ThreadConsumption));
            consumethread.IsBackground = true;
            consumethread.Start();
        }

        public void Recive(string message)
        {
            NewMsg Msg = (NewMsg)currentMsg;
            Msg.Send(message);
            if (message.IndexOf("bestmove") != -1)
            {
                ThreadSet();
            }
            if (message.IndexOf("restart") != -1)
            {
                consumethread.Abort();
                ThreadSet();
                Thread.Sleep(100);
                Consume();
            }
        }

        public void ThreadConsumption()
        {
            while (bRun)
            {
                //如果容器中有商品就进行消费
                if (getCount() != 0 && !bLock)
                {
                    lock (this)
                    {
                        bLock = true;
                    }
                       
                    //调用方法进行消费
                    Consumption();
                }
                Thread.Sleep(100);
                //容器中没有商品通知消费者消费
                //_wh.WaitOne();   //等待信号
            }
        }

        public void ThreadSet()
        {
            lock (this)
            {
                bLock = false;
            }
            //_wh.Set();  // 给工作线程发信号
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
