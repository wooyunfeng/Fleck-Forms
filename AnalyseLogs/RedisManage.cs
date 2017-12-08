using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Fleck_Forms
{
    class RedisManage
    {
        public RedisHelper redis;

        public RedisManage()
        {
            redis = new RedisHelper("127.0.0.1:6379", "jiao19890228");
        }

        internal bool getItemFromList(string list, int index)
        {
            if (redis.ContainsKey(list))
            {
                string value = redis.getItemFromList(list, index - 1);
                if (value != null && value.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal void setItemToList(string list, string message)
        {
            depthInfo depthinfo = new depthInfo(message);

            if (depthinfo.depth > 0 && depthinfo.pv != "")
            {
                int intDepth = depthinfo.depth;
                redis.addItemToListRight(list, message);
//                 for (int i = redis.getAllItems(list).Count; i < intDepth; i++)
//                 {
//                     redis.addItemToListRight(list, "");
//                 }
//                 redis.addItemToListRight(list, "");
//                 redis.setItemToList(list, intDepth - 1, message);
            }
        }

    }
}
