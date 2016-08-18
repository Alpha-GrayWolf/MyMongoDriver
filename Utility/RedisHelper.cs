using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class RedisHelper
    {
        public static ConnectionMultiplexer GetRedisConnection()
        {
            return ConnectionMultiplexer.Connect("192.168.1.111,password=admin");
        }

        public static bool SaveEntity<T>(IDatabase db,  string key, T entity, string tag)
        {
            System.Reflection.PropertyInfo[] pi = typeof(T).GetProperties();

            foreach (var p in pi)
            {
                var classAttribute = (DataField)Attribute.GetCustomAttribute(p, typeof(DataField));
                if (classAttribute.Tag.Contains(tag))
                {
                    db.HashSet(key, p.Name, p.GetValue(entity, null).ToString());
                }
            }

            return true;
        }
    }
}
