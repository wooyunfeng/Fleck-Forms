using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using RedisStudy;
using System.Net;
using System.IO;

namespace Fleck_Forms
{
    class RedisHelper
    {
         private RedisClient redisCli = null;
         public RedisHelper(string host)
         {
             string[] str = host.Split(':');
             string ip = str[0];
             int port = Int32.Parse(str[1]);
             createClient(ip,port);
         }
         public RedisHelper(string host, string keyword)
         {
             string[] str = host.Split(':');
             string ip = str[0];
             int port = Int32.Parse(str[1]);
             createClient(ip, port, keyword);
         }
        /// <summary>
        /// 建立redis长连接
        /// </summary>
        /// 将此处的IP换为自己的redis实例IP，如果设有密码，第三个参数为密码，string 类型
        public void createClient(string hostIP,int port,string keyword)
        {
            if (redisCli == null)
            {
                redisCli = new RedisClient(hostIP, port, keyword);
            }
 
        }
        public void createClient(string hostIP, int port)
        {
            if (redisCli == null)
            {
                redisCli = new RedisClient(hostIP, port);
             }
 
        }
        //private  RedisClient redisCli = new RedisClient("192.168.101.165", 6379, "123456");
        /// <summary>
        /// 获取key,返回string格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool setValueString(string key,string value)
        {
            bool re = redisCli.SetEntryIfNotExists(key, value);
            return re;          
        }
        /// <summary>
        /// 获取key,返回string格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getValueString(string key)
        {
            string value = redisCli.GetValue(key);
            return value;
        }
        /// <summary>
        /// 获取key,返回byte[]格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] getValueByte(string key)
        {
            byte[] value = redisCli.Get(key);
            return value;
        }
        /// <summary>
        /// 获得某个hash型key下的所有字段
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public List<string> getHashFields(string hashId)
        {
            List<string> hashFields = redisCli.GetHashKeys(hashId);
            return hashFields;
        }
        /// <summary>
        /// 获得某个hash型key下的所有值
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public List<string> getHashValues(string hashId)
        {
            List<string> hashValues = redisCli.GetHashKeys(hashId);
            return hashValues;
        }
        /// <summary>
        /// 获得hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public string getHashField(string key, string field)
        {
            string value = redisCli.GetValueFromHash(key, field);
            return value;
        }
        /// <summary>
        /// 设置hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void setHashField(string key, string field, string value)
        {
            redisCli.SetEntryInHash(key, field, value);
        }
        /// <summary>
        ///使某个字段增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public void setHashIncr(string key, string field, long incre)
        {
            redisCli.IncrementValueInHash(key, field, incre);

        }
        /// <summary>
        /// 向list类型数据添加成员，向列表底部(右侧)添加
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="list"></param>
        public void addItemToListRight(string list, string item)
        {
            redisCli.AddItemToList(list, item);
        }
        /// <summary>
        /// 向list类型数据添加成员，向列表顶部(左侧)添加
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public void addItemToListLeft(string list, string item)
        {
            redisCli.LPush(list, Encoding.Default.GetBytes(item));
        }
        /// <summary>
        /// 从list类型数据读取所有成员
        /// </summary>
        public List<string> getAllItems(string list)
        {
            List<string> listMembers = redisCli.GetAllItemsFromList(list);
            return listMembers;
        }
        /// <summary>
        /// 从list类型数据指定索引处获取数据，支持正索引和负索引
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string getItemFromList(string list, int index)
        {
            string item = redisCli.GetItemFromList(list, index);
            return item;
        }
        /// <summary>
        /// 向列表底部（右侧）批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public void getRangeToList(string list, List<string> values)
        {
            redisCli.AddRangeToList(list, values);
        }
        /// <summary>
        /// 向集合中添加数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="set"></param>
        public void getItemToSet(string item, string set)
        {
            redisCli.AddItemToSet(item, set);
        }
        /// <summary>
        /// 获得集合中所有数据
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getAllItemsFromSet(string set)
        {
            HashSet<string> items = redisCli.GetAllItemsFromSet(set);
            return items;
        }
        /// <summary>
        /// 获取fromSet集合和其他集合不同的数据
        /// </summary>
        /// <param name="fromSet"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public HashSet<string> getSetDiff(string fromSet, params string[] toSet)
        {
            HashSet<string> diff = redisCli.GetDifferencesFromSet(fromSet, toSet);
            return diff;
        }
        /// <summary>
        /// 获得所有集合的并集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getSetUnion(params string[] set)
        {
            HashSet<string> union = redisCli.GetUnionFromSets(set);
            return union;
        }
        /// <summary>
        /// 获得所有集合的交集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getSetInter(params string[] set)
        {
            HashSet<string> inter = redisCli.GetIntersectFromSets(set);
            return inter;
        }
        /// <summary>
        /// 向有序集合中添加元素
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public void addItemToSortedSet(string set,string value,long score)
        {
            redisCli.AddItemToSortedSet(set,value,score);
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的降序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long getItemIndexInSortedSetDesc(string set, string value)
        {
            long index = redisCli.GetItemIndexInSortedSetDesc(set, value);
            return index;
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的升序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long getItemIndexInSortedSet(string set, string value)
        {
            long index = redisCli.GetItemIndexInSortedSet(set, value);
            return index;
        }
        /// <summary>
        /// 获得有序集合中某个值得分数
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double getItemScoreInSortedSet(string set, string value)
        {
            double score = redisCli.GetItemScoreInSortedSet(set, value);
            return score;
        }
        /// <summary>
        /// 获得有序集合中，某个排名范围的所有值
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginRank"></param>
        /// <param name="endRank"></param>
        /// <returns></returns>
        public List<string> getRangeFromSortedSet(string set,int beginRank, int endRank)
        {
            List<string> valueList=redisCli.GetRangeFromSortedSet(set,beginRank,endRank);
            return valueList;
        }
        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，升序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public List<string> getRangeFromSortedSet(string set, double beginScore, double endScore)
        {
            List<string> valueList = redisCli.GetRangeFromSortedSetByHighestScore(set, beginScore, endScore);
            return valueList;
        }
        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，降序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public List<string> getRangeFromSortedSetDesc(string set, double beginScore, double endScore)
        {
            List<string> vlaueList=redisCli.GetRangeFromSortedSetByLowestScore(set,beginScore,endScore);
            return vlaueList;
        }
        public void dispose()
        {
            redisCli.Dispose();
        }
 
// 
//         IRedisClient Redis;
//         HashOperator operators;
// 
//         public RedisHelper()
//         {
//             InitRedis();
//         }
// 
//         public void InitRedis()
//         {
//             //获取Redis操作接口
//             Redis = RedisManager.GetClient();
//             //Hash表操作
//             operators = new HashOperator();
// 
//             Redis.Password = "jiao19890228";
//         }
// 
//         public string getFromRedis(string key)
//         {
//             lock (Redis)
//             {
//                 return Redis.GetValue(key);
//             }
// 
//         }
// 
//         public bool ContainsKey(string key)
//         {
//             lock (Redis)
//             {
//                 if (GetListCount(key) >= Int32.Parse(Setting.level))
//                 {
//                     return true;
//                 }
//                 Redis.RemoveAllFromList(key);
//                 return false;
//             }
// 
//         }
// 
//         public void setToRedis(string key, string value)
//         {
//             lock (Redis)
//             {
//                 Redis.SetEntryIfNotExists(key, value);
//             }
//         }
// 
//         public long GetListCount(string listId)
//         {
//             lock (Redis)
//             {
//                 return Redis.GetListCount(listId);
//             }
//         }
// 
//         public List<string> GetAllItemsFromList(string listId)
//         {
//             lock (Redis)
//             {
//                 return Redis.GetAllItemsFromList(listId);
//             }
//         }
//         
//         public string GetItemFromList(string listId, int listIndex)
//         {
//             lock (Redis)
//             {
//                 return Redis.GetItemFromList(listId, listIndex);
//             }
//         }
//         
//         public void CheckItemInList(string listId, int count)
//         {
//             lock (Redis)
//             {
//                 for (long i = Redis.GetListCount(listId)-1; i < count; i++)
//                 {
//                     Redis.AddItemToList(listId, "");
//                 }
//             }
//         }
// 
//         public void PushItemToList(string listId, string value)
//         {
//             lock (Redis)
//             {
//                 Redis.PushItemToList(listId, value);
//             }
//         }
//         
//         public void SetItemInList(string listId, int listIndex, string value)
//         {
//             lock (Redis)
//             {
//                 if (listIndex > 0 && listIndex <= Int32.Parse(Setting.level))
//                 {
//                     Redis.SetItemInList(listId, listIndex, value);
//                 }                
//             }
//         }

        public string QuerybestFromCloud(string board)
        {
            if (!Setting.isSupportCloudApi)
            {
                return null;
            }
            string serverResult = "";
            try
            {
                serverResult = getValueString("Querybest:" + board);
                if (serverResult == null)
                {
                    string serverUrl = "http://api.chessdb.cn:81/chessdb.php?action=querybest&board=" + board;
                    string postData = "";
                    serverResult = HttpPostConnectToServer(serverUrl, postData);
                    if (serverResult != null)
                    {
                        setValueString("Querybest:" + board, serverResult);
                    }
                    else
                    {
                        serverResult = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[error] QuerybestFromCloud " + ex.Message);
            }
            return serverResult;
        }

        public string QueryallFromCloud(string message)
        {
            string board = message.Substring(9, message.Length - 9);
            string serverResult = "";
            try
            {
                serverResult = getValueString("Queryall:" + board);
                if (serverResult == null)
                {
                    string serverUrl = "http://api.chessdb.cn:81/chessdb.php?action=queryall&board=" + board;
                    string postData = "";
                    serverResult = HttpPostConnectToServer(serverUrl, postData);
                    if (serverResult != null)
                    {                        
                        serverResult = serverResult.Replace("move:", "");//替换为空
                        serverResult = serverResult.Replace("score:", "");//替换为空
                        serverResult = serverResult.Replace("rank:", "");//替换为空
                        serverResult = serverResult.Replace("note:", "");//替换为空
                        serverResult = "Queryall:" + serverResult;
                        setValueString("Queryall:" + board, serverResult);
                    }
                    else
                    {
                        serverResult = "Queryall:null";
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[error] QueryallFromCloud " + ex.Message);
            }
            return serverResult;
        }

        public string HttpPostConnectToServer(string serverUrl, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求  
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  
            request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认  
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间  
            request.Timeout = 10000;
            //创建输入流  
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return null;//连接服务器失败  
            }
            //发送请求  
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息  
            string res = "";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[error] HttpPostConnectToServer " + ex.Message);
            }
            return res;
        }

        internal void setItemToList(string listId, int listIndex, string value)
        {
            redisCli.SetItemInList(listId, listIndex,value);
        }

        internal bool ContainsKey(string p)
        {
            return redisCli.ContainsKey(p);
        }
    }
}
