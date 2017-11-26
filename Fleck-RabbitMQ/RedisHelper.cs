using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
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
            lock (redisCli)
            {
                bool re = redisCli.SetEntryIfNotExists(key, value);
                return re;        
            } 
        }
        /// <summary>
        /// 获取key,返回string格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getValueString(string key)
        {
            lock (redisCli)
            {
                string value = redisCli.GetValue(key);
                return value;
            } 
        }
        /// <summary>
        /// 获取key,返回byte[]格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] getValueByte(string key)
        {
            lock (redisCli)
            {
                byte[] value = redisCli.Get(key);
                return value;
            } 
        }
        /// <summary>
        /// 获得某个hash型key下的所有字段
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public List<string> getHashFields(string hashId)
        {
            lock (redisCli)
            {
                List<string> hashFields = redisCli.GetHashKeys(hashId);
                return hashFields;
            } 
        }
        /// <summary>
        /// 获得某个hash型key下的所有值
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public List<string> getHashValues(string hashId)
        {
            lock (redisCli)
            {
                List<string> hashValues = redisCli.GetHashKeys(hashId);
                return hashValues;
            } 
        }
        /// <summary>
        /// 获得hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public string getHashField(string key, string field)
        {
            lock (redisCli)
            {
                string value = redisCli.GetValueFromHash(key, field);
                return value;
            } 
        }
        /// <summary>
        /// 设置hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void setHashField(string key, string field, string value)
        {
            lock (redisCli)
            {
                redisCli.SetEntryInHash(key, field, value);
            }             
        }
        /// <summary>
        ///使某个字段增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public void setHashIncr(string key, string field, long incre)
        {
            lock (redisCli)
            {
                redisCli.IncrementValueInHash(key, field, incre);
            }  

        }
        /// <summary>
        /// 向list类型数据添加成员，向列表底部(右侧)添加
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="list"></param>
        public void addItemToListRight(string list, string item)
        {
            lock (redisCli)
            {
                redisCli.AddItemToList(list, item);
            }           
        }
        /// <summary>
        /// 向list类型数据添加成员，向列表顶部(左侧)添加
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public void addItemToListLeft(string list, string item)
        {
            lock (redisCli)
            {
                redisCli.LPush(list, Encoding.Default.GetBytes(item));               
            }   
        }
        /// <summary>
        /// 从list类型数据读取所有成员
        /// </summary>
        public List<string> getAllItems(string list)
        {
            lock (redisCli)
            {
                List<string> listMembers = redisCli.GetAllItemsFromList(list);
                return listMembers;
            }   
        }
        /// <summary>
        /// 从list类型数据指定索引处获取数据，支持正索引和负索引
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string getItemFromList(string list, int index)
        {
            lock (redisCli)
            {
                string item = redisCli.GetItemFromList(list, index);
                return item;
            }   
        }
        /// <summary>
        /// 向列表底部（右侧）批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public void getRangeToList(string list, List<string> values)
        {
            lock (redisCli)
            {
                redisCli.AddRangeToList(list, values);
            }   
        }
        /// <summary>
        /// 向集合中添加数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="set"></param>
        public void getItemToSet(string item, string set)
        {
            lock (redisCli)
            {
                redisCli.AddItemToSet(item, set);
            }   
        }
        /// <summary>
        /// 获得集合中所有数据
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getAllItemsFromSet(string set)
        {
            lock (redisCli)
            {
                HashSet<string> items = redisCli.GetAllItemsFromSet(set);
                return items;
            }   
        }
        /// <summary>
        /// 获取fromSet集合和其他集合不同的数据
        /// </summary>
        /// <param name="fromSet"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public HashSet<string> getSetDiff(string fromSet, params string[] toSet)
        {
            lock (redisCli)
            {
                HashSet<string> diff = redisCli.GetDifferencesFromSet(fromSet, toSet);
                return diff;
            } 
        }
        /// <summary>
        /// 获得所有集合的并集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getSetUnion(params string[] set)
        {
            lock (redisCli)
            {
                HashSet<string> union = redisCli.GetUnionFromSets(set);
                return union;
            }   
        }
        /// <summary>
        /// 获得所有集合的交集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<string> getSetInter(params string[] set)
        {
            lock (redisCli)
            {
                HashSet<string> inter = redisCli.GetIntersectFromSets(set);
                return inter;
            }              
        }
        /// <summary>
        /// 向有序集合中添加元素
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public void addItemToSortedSet(string set,string value,long score)
        {
            lock (redisCli)
            {
                redisCli.AddItemToSortedSet(set, value, score);
            }   
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的降序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long getItemIndexInSortedSetDesc(string set, string value)
        {
            lock (redisCli)
            {
                long index = redisCli.GetItemIndexInSortedSetDesc(set, value);
                return index;
            }               
        }
        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的升序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long getItemIndexInSortedSet(string set, string value)
        {
            lock (redisCli)
            {
                long index = redisCli.GetItemIndexInSortedSet(set, value);
                return index;
            }               
        }
        /// <summary>
        /// 获得有序集合中某个值得分数
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double getItemScoreInSortedSet(string set, string value)
        {
            lock (redisCli)
            {
                double score = redisCli.GetItemScoreInSortedSet(set, value);
                return score;
            }              
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
            lock (redisCli)
            {
                List<string> valueList = redisCli.GetRangeFromSortedSet(set, beginRank, endRank);
                return valueList;
            }             
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
            lock (redisCli)
            {
                List<string> valueList = redisCli.GetRangeFromSortedSetByHighestScore(set, beginScore, endScore);
                return valueList;
            }               
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
            lock (redisCli)
            {
                List<string> vlaueList = redisCli.GetRangeFromSortedSetByLowestScore(set, beginScore, endScore);
                return vlaueList;
            }              
        }
        public void dispose()
        {
            lock (redisCli)
            {
                redisCli.Dispose();
            }   
        }
        
        internal void setItemToList(string listId, int listIndex, string value)
        {
            lock (redisCli)
            {
                redisCli.SetItemInList(listId, listIndex, value);
            }   
        }

        internal bool ContainsKey(string p)
        {
            lock (redisCli)
            {
                return redisCli.ContainsKey(p);
            }   
        }
    }
}
