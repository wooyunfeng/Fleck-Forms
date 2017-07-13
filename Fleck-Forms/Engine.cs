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
using Fleck_Forms;

namespace Fleck.aiplay
{
    class Engine : Comm
    {
        private static StreamWriter PipeWriter { get; set; }
        private static bool bLock { get; set; }
        Process pProcess;
        Boolean bEngineRun;
        DateTime EngineRunTime;
        public Queue<NewMsg> InputEngineQueue;
        public NewMsg currentMsg;
        public Queue OutputEngineQueue;
        // 为保证线程安全，使用一个锁来保护_task的访问
        readonly static object _locker = new object();
        // 通过 _wh 给工作线程发信号
        static EventWaitHandle _wh = new AutoResetEvent(false);

        public bool bJson { get; set; }
        public int getMsgQueueCount()
        {
            int count = 0;
            if (InputEngineQueue == null)
            {
                return 0;
            }
            lock (_locker)
            {
                count = InputEngineQueue.Count;
            }
            return count;
        }

        public void OutputEngineQueueEnqueue(string line, bool save = false)
        {
            if (OutputEngineQueue == null || line == null || line.Length == 0)
            {
                return;
            }
            if (save)
            {
                WriteInfo(line);
            }
            lock (OutputEngineQueue)
            {
                OutputEngineQueue.Enqueue(line);
            }
        }

        public string OutputEngineQueueDequeue()
        {
            if (OutputEngineQueue == null)
            {
                return null;
            }
            lock (OutputEngineQueue)
            {
                return (string)OutputEngineQueue.Dequeue();
            }
        }

        public void Start()
        {
            //启动引擎线程
            Init();    
            //启动管道线程
            StartPipeThread();
            //启动消费者线程
            StartCustomerThread();
            InputEngineQueue = new Queue<NewMsg>();
            OutputEngineQueue = new Queue();
            bLock = false;
            currentMsg = null;
        }

        public void StartPipeThread()
        {
            Thread pipeThread = new Thread(new ThreadStart(PipeThread));
            pipeThread.IsBackground = true;
            pipeThread.Start();
        }

        private void PipeInit(string strFile, string arg)
        {
            pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo = new System.Diagnostics.ProcessStartInfo();
            pProcess.StartInfo.FileName = strFile;
            pProcess.StartInfo.Arguments = arg;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
        }

        private void KillPipeThread()
        {
            bEngineRun = false;
            try
            {
                if (pProcess != null)
                {
                    pProcess.Kill();
                    pProcess.Close();
                }                
                PipeWriter = null;
            }
            catch (System.Exception ex)
            {                
                OutputEngineQueueEnqueue("[error] KillPipeThread " + ex.Message, true);
            }
            Thread.Sleep(100);
        }

        public void PipeThread()
        {            
            EngineRunTime = System.DateTime.Now;
            int intDepth = 0;
            string line = "";
            try
            {
                //管道参数初始化
                PipeInit(Setting.engine, "");
                //截取输出流
                StreamReader reader = pProcess.StandardOutput;
                //截取输入流
                PipeWriter = pProcess.StandardInput;
                //每次读取一行
                line = reader.ReadLine();
                OutputEngineQueueEnqueue(line); 
                line = reader.ReadLine();
                OutputEngineQueueEnqueue(line);
                bEngineRun = true;

                while (bEngineRun)
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        string[] sArray = line.Split(' ');
                        /* 消息过滤
                         * info depth 14 seldepth 35 multipv 1 score 19 nodes 243960507 nps 6738309 hashfull 974 tbhits 0 time 36205 
                         * pv h2e2 h9g7 h0g2 i9h9 i0h0 b9c7 h0h4 h7i7 h4h9 g7h9 c3c4 b7a7 b2c2 c9e7 c2c6 a9b9 b0c2 g6g5 a0a1 h9g7 
                         */
                        if (sArray.Length > 3 && sArray[1] == "depth" && sArray[3] == "seldepth")
                        {
                            intDepth = Int32.Parse(sArray[2]);
                            currentMsg.Send(line);
                        //    redisPushItemToList(currentRole.GetCurrentMsg().message, line);
                        }

                        if (line.IndexOf("bestmove") != -1)
                        {
                            currentMsg.Send(line);
                            OutputEngineQueueEnqueue("depth " + intDepth.ToString() + " " + line);
                            SQLite_UpdateCommand(1, line, currentMsg.GetAddr(), currentMsg.GetMessage());
                            _wh.Set();  // 给工作线程发信号
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _wh.Set();  // 给工作线程发信号
                OutputEngineQueueEnqueue("[error] PipeThread " + ex.Message, true);
                resetEngine();                
            }
        }
        
        public void resetEngine()
        {
            bLock = false;
            KillPipeThread();
            //启动管道线程
            StartPipeThread();
        }

        public void StartCustomerThread()
        {
            Thread customerThread = new Thread(new ThreadStart(CustomerThread));
            customerThread.IsBackground = true;
            customerThread.Start();
        }

        public Role GetRoleAt(IWebSocketConnection socket)
        {
            return user.GetAt(socket);
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
                string str = DealQueryallMessage(message);
                OutputEngineQueueEnqueue(str);
                socket.Send(str);
            }
            else if (message.IndexOf("position") != -1)
            {
                DealPositionMessage(socket, message);
                WritePosition(socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort.ToString() + " " + message);
            }
        }

        private void DealPositionMessage(IWebSocketConnection socket, string message)
        {
            //记录每个用户的消息队列
            NewMsg msg = new NewMsg(socket,message);
            lock (_locker)
            {
                InputEngineQueue.Enqueue(msg);
            }      
        }

        public void CustomerThread()
        {            
            while (true)
            {
                try
                {
                    currentMsg = null;
                    lock (_locker)
                    {
                         if (InputEngineQueue.Count > 0)
                        {
                            currentMsg = InputEngineQueue.Dequeue();
                        }
                    }
                   
                    if (currentMsg != null)
                    {
                        EngineDeal(currentMsg);  // 任务不为null时，处理并保存数据     
                        _wh.WaitOne();   //等待信号
                    }

                    Thread.Sleep(100);
                }
                catch (System.Exception ex)
                {                    
                    OutputEngineQueueEnqueue("[error] GetFromEngine " + ex.Message, true);
                    resetEngine();
                }
            }
        }

        public void EngineDeal(NewMsg msg)
        {
            if (redisContainsKey(msg.GetCommand()))
            {
                OutputEngineQueueEnqueue("getFromList");
                getFromList(msg);
                _wh.Set();  // 给工作线程发信号
            }
            else
            {
                OutputEngineQueueEnqueue("getFromEngine");
                PipeWriter.Write(msg.GetCommand() + "\r\n");
                PipeWriter.Write("go depth " + Setting.level + "\r\n");
                Thread.Sleep(50);
            }
        }

        public void stopEngine()
        {
            bLock = false;
            KillPipeThread();
            OutputEngineQueueEnqueue("引擎停止！");
        }

        internal object getUserCount()
        {
            return user.allRoles.Count;
        }
    }
}
