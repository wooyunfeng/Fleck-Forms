﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using Fleck;

namespace Fleck_Forms
{
    class Role
    {
        public IWebSocketConnection connection { get; set; }
        public Queue<Msg> MsgQueue { get; set; }
        public Queue QueryallQueue { get; set; }
        public Queue<Msg> FinishQueue { get; set; }
        public Msg currentMsg { get; set; }
        public DateTime createTime { get; set; }
        public DateTime lastdealTime { get; set; }
        public int MsgCount = 0;
        public Role()
        {
            connection = null;
            MsgQueue = new Queue<Msg>();
            QueryallQueue = new Queue();
            FinishQueue = new Queue<Msg>();
            createTime = System.DateTime.Now;
            lastdealTime = System.DateTime.Now;
        }
        public Role(IWebSocketConnection connection)
        {
            this.connection = connection;
            MsgQueue = new Queue<Msg>();
            FinishQueue = new Queue<Msg>();
            QueryallQueue = new Queue();
            createTime = System.DateTime.Now;
            lastdealTime = System.DateTime.Now;
        }
        public void EnqueuePositionMessage(Msg msg)
        {
            lock (MsgQueue)
            {
                MsgQueue.Enqueue(msg);
            }
            MsgCount++;
        }

        public void EnqueueQueryMessage(string msg)
        {
//             lock (QueryallQueue)
//             {
//                 QueryallQueue.Enqueue(msg);
//             }
            MsgCount++;
        }

        public override string ToString()
        {
            return connection.ConnectionInfo.ClientIpAddress + ":" + connection.ConnectionInfo.ClientPort.ToString() + " createTime:" + createTime.ToString() + " lastdealTime:" + lastdealTime.ToString(); ;
        }
        public void Done(string line)
        {
            Send(line);
            currentMsg.dealTime = System.DateTime.Now;
            currentMsg.retval = line;
            currentMsg.isreturn = true;
            lastdealTime = currentMsg.dealTime;
            lock (MsgQueue)
            {
                MsgQueue.Dequeue();
            }
          //  FinishQueue.Enqueue(currentMsg);
        }

        public string GetAddr()
        {
            return connection.ConnectionInfo.ClientIpAddress + ":" + connection.ConnectionInfo.ClientPort.ToString();
        }

        public Msg GetCurrentMsg()
        {
            currentMsg = MsgQueue.Peek();
            return currentMsg;
        }

        public int GetMsgCount()
        {
            return MsgCount;
        }

        public void Send(string line)
        {
            connection.Send(line);
        }
        //检查用户活跃度，10分钟不操作认为离线
        public bool isActive()
        {
            DateTime currentTime = System.DateTime.Now;
            TimeSpan span = currentTime.Subtract(lastdealTime);
            if (span.Minutes > 0)
            {
                return false;
            }
            return true;
        }
        //检查消息处理，20秒不操作认为离线
        public bool Check()
        {
            DateTime currentTime = System.DateTime.Now;
            Msg firstMsg = GetCurrentMsg();
            TimeSpan span = currentTime.Subtract(firstMsg.createTime);
            if (span.Seconds > 20)
            {
                return false;
            }
            return true;
        }
    }
    class resultMsg
    {
        public string index { get; set; }
        public string commandtype { get; set; }
        public string result { get; set; }
        public resultMsg()
        {
            index = "";
            commandtype = "";
            result = "";
        }
        // 对象--->JSON  
        public string GetJson()
        {
            return JavaScriptConvert.SerializeObject(this);
        }  
    }
    //info depth 14 seldepth 35 multipv 1 score 19 nodes 243960507 nps 6738309 hashfull 974 tbhits 0 time 36205
    //pv h2e2 h9g7 h0g2 i9h9 i0h0 b9c7 h0h4 h7i7 h4h9 g7h9 c3c4 b7a7 b2c2 c9e7 c2c6 a9b9 b0c2 g6g5 a0a1 h9g7                    
    class depthInfo
    {
        public int depth { get; set; }
        public int seldepth { get; set; }
        public int multipv { get; set; }
        public int score { get; set; }
        public int nodes { get; set; }
        public int nps { get; set; }
        public int hashfull { get; set; }
        public int tbhits { get; set; }
        public int time { get; set; }
        public string pv { get; set; }
        public depthInfo(string message)
        {
            depth = -1;
            seldepth = -1;
            multipv = -1;
            score = -1;
            nodes = -1;
            nps = -1;
            hashfull = -1;
            tbhits = -1;
            time = -1;
            pv = "";
            Parser(message);
        }

        private void Parser(string message)
        {
            string[] arr = message.Split(' ');

            depth = getInt(arr, "depth");
            seldepth = getInt(arr, "seldepth");
            multipv = getInt(arr, "multipv");
            score = getInt(arr, "score");
            nodes = getInt(arr, "nodes");
            nps = getInt(arr, "nps");
            hashfull = getInt(arr, "hashfull");
            tbhits = getInt(arr, "tbhits");
            time = getInt(arr, "time");
            pv = getvalue(arr, "pv");
        }

        private string getvalue(string[] arr, string key)
        {
            for (int i = 0; i < arr.Length; i++ )
            {
                if (arr[i] == key)
                {
                    if (key == "pv")
                    {
                        string str = "";
                        for (int j = i+1; j < arr.Length; j++)
                        {
                            str += arr[j] + " ";
                        }
                        return str;
                    }
                    return arr[i+1];
                }
            }
            return "";
        }
        private int getInt(string[] arr, string key)
        {
            for (int i = 0; i < arr.Length; i++ )
            {
                if (arr[i] == key)
                {
                    return Int32.Parse(arr[i+1]);
               }
            }
            return -1;
        }
    }

    class NewMsg
    {
        private IWebSocketConnection connection { get; set; }
        public string uuid { get; set; }
        private string message { get; set; }
        public string index { get; set; }
        private string command { get; set; }
        public string commandtype { get; set; }
        private bool isJson { get; set; }
        public UInt64 boardID { get; set; }
        public NewMsg(IWebSocketConnection connection, string message)
        {
            if (JsonSplit.IsJson(message))//传入的json串
            {
                isJson = true;
                JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(message);
                uuid = jsonObj["id"].ToString();
                index = jsonObj["index"].ToString();
                command = jsonObj["command"].ToString();
                commandtype = command.Substring(0, 8);
                if (command.IndexOf('K') < command.IndexOf('k'))
                {
                    command = null;
                }
            }
            else
            {
                isJson = false;
            }

            this.connection = connection;
            this.message = message;
            Zobrist zobrist = new Zobrist();
            this.boardID = zobrist.getKey(GetBoard());
        }

        public NewMsg(string message)
        {
            if (JsonSplit.IsJson(message))//传入的json串
            {
                isJson = true;
                JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(message);
                uuid = jsonObj["id"].ToString();
                index = jsonObj["index"].ToString();
                command = jsonObj["command"].ToString();
                commandtype = command.Substring(0, 8);
                if (command.IndexOf('K') < command.IndexOf('k'))
                {
                    command = null;
                }
            }
            else
            {
                isJson = false;
            }
            this.message = message;
            Zobrist zobrist = new Zobrist();
            this.boardID = zobrist.getKey(GetBoard());
        }
        internal string Send(string strmsg)
        {
            if (JsonSplit.IsJson(strmsg))//传入的json串
            {
                connection.Send(strmsg);
                return strmsg;
            }
            string result = strmsg;
            if (isJson)
            {
                resultMsg resultmsg = new resultMsg();
                resultmsg.index = index;
                resultmsg.commandtype = commandtype;
                resultmsg.result = strmsg;
                result = resultmsg.GetJson();               
            }

            connection.Send(result);
            return result;
        }
        internal string SendOpenbook(string strmsg)
        {
            string result = strmsg;
            if (isJson)
            {
                resultMsg resultmsg = new resultMsg();
                resultmsg.index = index;
                resultmsg.commandtype = "openbook";
                resultmsg.result = strmsg;
                result = resultmsg.GetJson();
            }

            connection.Send(result);
            return result;
        }

        internal string GetIndex()
        {
            return index;
        }

        internal string GetCommandType()
        {
            return commandtype;
        }

        internal string GetCommand()
        {
            return command;
        }

        internal string GetBoard()
        {
            if (command.IndexOf("position") != -1)
            {
                return command.Substring(13,command.Length-13);
            }
            else if (command.IndexOf("queryall") != -1)
            {
                return command.Substring(9, command.Length - 9);
            }
            else if (command.IndexOf("openbook") != -1)
            {
                return command.Substring(9, command.Length - 9);
            }
            return "";
        }

        internal string GetFen()
        {
            if (command.IndexOf("position") != -1)
            {
                return command.Substring(13, command.Length - 13 - 8);
            }
            else if (command.IndexOf("queryall") != -1)
            {
                return command.Substring(9, command.Length - 9);
            }
            else if (command.IndexOf("openbook") != -1)
            {
                return command.Substring(9, command.Length - 9);
            }
            return "";
        }


        internal string GetDepth()
        {
            string strdepth = null;
            string strpower;
            string type;
            if (isJson)
            {
                try
                {
                    JavaScriptObject jsonObj = JavaScriptConvert.DeserializeObject<JavaScriptObject>(message);
                    type = jsonObj["type"].ToString();
                    if (type == "1") return Setting.level;

                    strpower = jsonObj["power"].ToString();
                    switch (strpower)
                    {
                        case "level-0":
                            strdepth = "3";
                            break;
                        case "level-1":
                            strdepth = "6";
                            break;
                        case "level-2":
                            strdepth = "9";
                            break;
                        case "level-3":
                            strdepth = "12";
                            break;
                        case "level-4":
                            strdepth = "15";
                            break;
                        case "level-5":
                            strdepth = "16";
                            break;
                        case "level-6":
                            strdepth = "17";
                            break;
                        default:
                            strdepth = Setting.level;
                            break;
                    }
                }
                catch 
                {
                    strdepth = Setting.level;
                }                
            }
            else
            {
                strdepth = Setting.level;
            }

            return strdepth;
        }


        internal string GetMessage()
        {
            return message;
        }

        internal string GetAddr()
        {
            return connection.ConnectionInfo.ClientIpAddress + ":" + connection.ConnectionInfo.ClientPort.ToString();
        }
    }

    internal class JsonSplit
    {
        private static bool IsJsonStart(ref string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim('\r', '\n', ' ');
                if (json.Length > 1)
                {
                    char s = json[0];
                    char e = json[json.Length - 1];
                    return (s == '{' && e == '}') || (s == '[' && e == ']');
                }
            }
            return false;
        }
        internal static bool IsJson(string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }
        internal static bool IsJson(string json, out int errIndex)
        {
            errIndex = 0;
            if (IsJsonStart(ref json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (SetCharState(c, ref cs) && cs.childrenStart)//设置关键符号状态。
                    {
                        string item = json.Substring(i);
                        int err;
                        int length = GetValueLength(item, true, out err);
                        cs.childrenStart = false;
                        if (err > 0)
                        {
                            errIndex = i + err;
                            return false;
                        }
                        i = i + length - 1;
                    }
                    if (cs.isError)
                    {
                        errIndex = i;
                        return false;
                    }
                }

                return !cs.arrayStart && !cs.jsonStart;
            }
            return false;
        }

        /// <summary>
        /// 获取值的长度（当Json值嵌套以"{"或"["开头时）
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (!SetCharState(c, ref cs))//设置关键符号状态。
                    {
                        if (!cs.jsonStart && !cs.arrayStart)//json结束，又不是数组，则退出。
                        {
                            break;
                        }
                    }
                    else if (cs.childrenStart)//正常字符，值状态下。
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//递归子值，返回一个长度。。。
                        cs.childrenStart = false;
                        cs.valueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.isError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.jsonStart && !cs.arrayStart)//记录当前结束位置。
                    {
                        len = i + 1;//长度比索引+1
                        break;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// 字符状态
        /// </summary>
        private class CharState
        {
            internal bool jsonStart = false;//以 "{"开始了...
            internal bool setDicValue = false;// 可以设置字典值了。
            internal bool escapeChar = false;//以"\"转义符号开始了
            /// <summary>
            /// 数组开始【仅第一开头才算】，值嵌套的以【childrenStart】来标识。
            /// </summary>
            internal bool arrayStart = false;//以"[" 符号开始了
            internal bool childrenStart = false;//子级嵌套开始了。
            /// <summary>
            /// 【0 初始状态，或 遇到“,”逗号】；【1 遇到“：”冒号】
            /// </summary>
            internal int state = 0;

            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int keyStart = 0;
            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int valueStart = 0;
            internal bool isError = false;//是否语法错误。

            internal void CheckIsError(char c)//只当成一级处理（因为GetLength会递归到每一个子项处理）
            {
                if (keyStart > 1 || valueStart > 1)
                {
                    return;
                }
                //示例 ["aa",{"bbbb":123,"fff","ddd"}] 
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        isError = jsonStart && state == 0;//重复开始错误 同时不是值处理。
                        break;
                    case '}':
                        isError = !jsonStart || (keyStart != 0 && state == 0);//重复结束错误 或者 提前结束{"aa"}。正常的有{}
                        break;
                    case '[':
                        isError = arrayStart && state == 0;//重复开始错误
                        break;
                    case ']':
                        isError = !arrayStart || jsonStart;//重复开始错误 或者 Json 未结束
                        break;
                    case '"':
                    case '\'':
                        isError = !(jsonStart || arrayStart); //json 或数组开始。
                        if (!isError)
                        {
                            //重复开始 [""",{"" "}]
                            isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
                        }
                        if (!isError && arrayStart && !jsonStart && c == '\'')//['aa',{}]
                        {
                            isError = true;
                        }
                        break;
                    case ':':
                        isError = !jsonStart || state == 1;//重复出现。
                        break;
                    case ',':
                        isError = !(jsonStart || arrayStart); //json 或数组开始。
                        if (!isError)
                        {
                            if (jsonStart)
                            {
                                isError = state == 0 || (state == 1 && valueStart > 1);//重复出现。
                            }
                            else if (arrayStart)//["aa,] [,]  [{},{}]
                            {
                                isError = keyStart == 0 && !setDicValue;
                            }
                        }
                        break;
                    case ' ':
                    case '\r':
                    case '\n'://[ "a",\r\n{} ]
                    case '\0':
                    case '\t':
                        break;
                    default: //值开头。。
                        isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);//
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }
        /// <summary>
        /// 设置字符状态(返回true则为关键词，返回false则当为普通字符处理）
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            cs.CheckIsError(c);
            switch (c)
            {
                case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                    #region 大括号
                    if (cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        if (cs.jsonStart && cs.state == 1)
                        {
                            cs.childrenStart = true;
                        }
                        else
                        {
                            cs.state = 0;
                        }
                        cs.jsonStart = true;//开始。
                        return true;
                    }
                    #endregion
                    break;
                case '}':
                    #region 大括号结束
                    if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
                    {
                        cs.jsonStart = false;//正常结束。
                        cs.state = 0;
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;
                    #endregion
                    break;
                case '[':
                    #region 中括号开始
                    if (!cs.jsonStart)
                    {
                        cs.arrayStart = true;
                        return true;
                    }
                    else if (cs.jsonStart && cs.state == 1)
                    {
                        cs.childrenStart = true;
                        return true;
                    }
                    #endregion
                    break;
                case ']':
                    #region 中括号结束
                    if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)//[{},333]//这样结束。
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.arrayStart = false;
                        return true;
                    }
                    #endregion
                    break;
                case '"':
                case '\'':
                    #region 引号
                    if (cs.jsonStart || cs.arrayStart)
                    {
                        if (cs.state == 0)//key阶段,有可能是数组["aa",{}]
                        {
                            if (cs.keyStart <= 0)
                            {
                                cs.keyStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.keyStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }
                        }
                        else if (cs.state == 1 && cs.jsonStart)//值阶段必须是Json开始了。
                        {
                            if (cs.valueStart <= 0)
                            {
                                cs.valueStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.valueStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }

                        }
                    }
                    #endregion
                    break;
                case ':':
                    #region 冒号
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                    {
                        if (cs.keyStart == 1)
                        {
                            cs.keyStart = -1;
                        }
                        cs.state = 1;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                    #endregion
                    break;
                case ',':
                    #region 逗号 //["aa",{aa:12,}]

                    if (cs.jsonStart)
                    {
                        if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                        {
                            cs.state = 0;
                            cs.keyStart = 0;
                            cs.valueStart = 0;
                            //if (cs.valueStart == 1)
                            //{
                            //    cs.valueStart = 0;
                            //}
                            cs.setDicValue = true;
                            return true;
                        }
                    }
                    else if (cs.arrayStart && cs.keyStart <= 2)
                    {
                        cs.keyStart = 0;
                        //if (cs.keyStart == 1)
                        //{
                        //    cs.keyStart = -1;
                        //}
                        return true;
                    }
                    #endregion
                    break;
                case ' ':
                case '\r':
                case '\n'://[ "a",\r\n{} ]
                case '\0':
                case '\t':
                    if (cs.keyStart <= 0 && cs.valueStart <= 0) //cs.jsonStart && 
                    {
                        return true;//跳过空格。
                    }
                    break;
                default: //值开头。。
                    if (c == '\\') //转义符号
                    {
                        if (cs.escapeChar)
                        {
                            cs.escapeChar = false;
                        }
                        else
                        {
                            cs.escapeChar = true;
                            return true;
                        }
                    }
                    else
                    {
                        cs.escapeChar = false;
                    }
                    if (cs.jsonStart || cs.arrayStart) // Json 或数组开始了。
                    {
                        if (cs.keyStart <= 0 && cs.state == 0)
                        {
                            cs.keyStart = 1;//无引号的
                        }
                        else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)//只有Json开始才有值。
                        {
                            cs.valueStart = 1;//无引号的
                        }
                    }
                    break;
            }
            return false;
        }
    }
    class Msg
    {
        public string id { get; set; }
        public string message { get; set; }
        public List<string> mList { get; set; }
        public bool isreturn { get; set; }
        public string retval { get; set; }
        public DateTime createTime { get; set; }
        public DateTime dealTime { get; set; }
        public int dealCount { get; set; }
        public Msg()
        {
            id = "";
            message = "";
            retval = "";
            mList = new List<string>();
            isreturn = false;
            createTime = System.DateTime.Now;
            dealTime = System.DateTime.Now;
            dealCount = 0;
        }
        public Msg(string message)
        {
            id = "";
            this.message = message;
            retval = "";
            mList = new List<string>();
            isreturn = false;
            createTime = System.DateTime.Now;
            dealTime = System.DateTime.Now;
            dealCount = 0;
        }
        public Msg(string id, string message)
        {
            this.id = id;
            this.message = message;
            retval = "";
            mList = new List<string>();
            isreturn = false;
            createTime = System.DateTime.Now;
            dealTime = System.DateTime.Now;
            dealCount = 0;
        }
        public bool CheckDate()
        {
            if (dealCount > 5)
            {
                return false;
            }
            return true;
        }
    }

    class User
    {
        public List<IWebSocketConnection> allSockets;
        public List<Role> allRoles;
        public Role currentRole { get; set; }
        public User()
        {
            allSockets = new List<IWebSocketConnection>();
            allRoles = new List<Role>();
        }

        public void Add(Role role)
        {
            allRoles.Add(role);
            allSockets.Add(role.connection);
        }

        public void Add(IWebSocketConnection socket)
        {
            var role = new Role(socket);
            allRoles.Add(role);
            allSockets.Add(socket);
        }

        public void Remove(IWebSocketConnection socket)
        {
            foreach (var r in allRoles.ToList())
            {
                if (r.connection == socket)
                {
                    allRoles.Remove(r);
                }
            }
            allSockets.Remove(socket);
        }

        public Role GetAt(IWebSocketConnection socket)
        {
            int index = allSockets.IndexOf(socket);
            if (index != -1)
            {
                return allRoles[index];
            }
            else
            {
                return null;
            }
        }


        public int getSize()
        {
            return allRoles.Count;
        }
    }
}
