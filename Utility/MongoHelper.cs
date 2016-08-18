using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class MongoHelper
    {
        public static IMongoCollection<T> GetMongoCollection<T>(string db_name, string collection_name)
        {
            IMongoClient client = new MongoClient("mongodb://192.168.1.111:27017"); 
            IMongoDatabase db = client.GetDatabase(db_name);
            return db.GetCollection<T>(collection_name);
        }
        public static void Create<T>(IMongoCollection<T> collection, T entity)
        {
            collection.InsertOne(entity);
        }
        public static List<T> Retrieve<T>(IMongoCollection<T> collection, MongoFilterCondition fcp = null)
        {
            string json = null == fcp ? "{}" : "{" + fcp.condition + "}";
            BsonDocument filter = BsonDocument.Parse(json);

            return collection.Find(filter, null).Sort("{CreateTime: -1}").ToList<T>();
        }
        public static DataPacket RetrievePage<T>(IMongoCollection<T> collection, int pageindex, int pagesize, MongoFilterCondition fcp)
        {
            string json = null == fcp ? "{}" : "{" + fcp.condition + "}";
            BsonDocument filter = BsonDocument.Parse(json);

            int datatotal = (int)collection.Find(filter, null).Count();
            int pagetotal = datatotal / pagesize + (0 == datatotal % pagesize ? 0 : 1);

            DataPacket dp = new DataPacket();
            dp.Data = collection.Find(filter, null).Sort("{CreateTime: -1}").Skip((pageindex - 1) * pagesize).Limit(pagesize).ToList<T>();
            dp.PagePacket = new PagePacket(pageindex, pagesize, pagetotal, datatotal);

            return dp;
        }
        public static void Update<T>(IMongoCollection<T> collection, string id, T entity, string tag)
        {
            MongoFilterCondition fc = new MongoFilterCondition();
            fc.AddFilterCondition("_id", "'" + id + "'", MongoRelationalOperate.Equal);

            string filter = null == fc ? "{}" : "{" + fc.condition + "}";

            System.Reflection.PropertyInfo[] pi = typeof(T).GetProperties();
            string update = "{$set:{";
            foreach (var p in pi)
            {
                var classAttribute = (DataField)Attribute.GetCustomAttribute(p, typeof(DataField));
                if (!classAttribute.Tag.Contains(tag))
                    continue;

                if ("String" == p.PropertyType.Name)
                    update += p.Name + ":'" + p.GetValue(entity, null) + "',";
                else
                    update += p.Name + ":" + p.GetValue(entity, null) + ",";
            }

            if (',' == update[update.Length - 1])
            {
                update = update.Remove(update.Length - 1);
                update += "}";
            }

            update += "}";

            Update(collection, filter, update);
        }
        public static void Update<T>(IMongoCollection<T> collection, string filter, string update)
        {
            collection.UpdateOne(filter, update);
        }

        public static void Delete<T>(IMongoCollection<T> collection, string id)
        {
            MongoFilterCondition fc = new MongoFilterCondition();

            fc.AddFilterCondition("_id", "'" + id + "'", MongoRelationalOperate.Equal);

            string json = null == fc ? "{}" : "{" + fc.condition + "}";

                
        }

    }

    public class MongoFilterCondition
    {
        public string condition { get; set; }

        public MongoFilterCondition()
        {
            this.condition = "";
        }
        public MongoFilterCondition(string key, string value, MongoRelationalOperate rop)
        {
            this.condition = "";
            string[] values = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            MongoFilterCondition mfc = new MongoFilterCondition(key, values, rop);
            this.condition = mfc.condition;
        }
        public MongoFilterCondition(string key, string[] value, MongoRelationalOperate rop)
        {
            this.condition = "";
            do
            {
                if (null == value || 0 == value.Length)
                    break;

                if (MongoRelationalOperate.Between == rop && 2 > value.Length)
                    break;

                switch ((int)rop)
                {
                    case 0: this.condition = key + ":" + value[0]; break;
                    case 1: this.condition = key + ":" + "{$gt:'" + value[0] + "'}"; break;
                    case 2: this.condition = key + ":" + "{$lt:'" + value[0] + "'}"; break;
                    case 3: this.condition = key + ":" + "{$gte:'" + value[0] + "'}"; break;
                    case 4: this.condition = key + ":" + "{$lte:'" + value[0] + "'}"; break;
                    case 5: this.condition = key + ":" + "{$ne:'" + value[0] + "'}"; break;
                    case 6: this.condition = key + ":" + "{$gt:'" + value[0] + ",$lt:" + value[1] + "'}"; break;
                    case 7: this.condition = key + ":" + "{$in:" + value + "}"; break;
                    case 8: this.condition = key + ":" + "{$nin:" + value + "}"; break;
                    case 9: this.condition = key + ":/" + value[0] + "/"; break;
                    default: this.condition = ""; break;
                }
            } while (false);
        }
        public bool AddFilterCondition(string key, string value, MongoRelationalOperate rop, MongoLogicalOperate lop = MongoLogicalOperate.And)
        {
            MongoFilterCondition fc = new MongoFilterCondition(key, value, rop);

            return AddFilterCondition(fc, lop);
        }
        public bool AddFilterCondition(string key, string[] value, MongoRelationalOperate rop, MongoLogicalOperate lop = MongoLogicalOperate.And)
        {
            MongoFilterCondition fc = new MongoFilterCondition(key, value, rop);

            return AddFilterCondition(fc, lop);
        }
        public bool AddFilterCondition(MongoFilterCondition fc, MongoLogicalOperate lop = MongoLogicalOperate.And)
        {
            switch ((int)lop)
            {
                case 0: this.condition = "" == this.condition ? fc.condition : this.condition + "," + fc.condition; break;
                case 1: this.condition = "" == this.condition ? fc.condition : "$or: [{" + this.condition + "},{" + fc.condition + "}]"; break;
                default: this.condition = "" == this.condition ? fc.condition : this.condition + "," + fc.condition; break;
            }

            return true;
        }
        public bool SetFilterCondition(string key, string value, MongoRelationalOperate rop)
        {
            string[] values = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return SetFilterCondition(key, values, rop);
        }
        public bool SetFilterCondition(string key, string[] value, MongoRelationalOperate rop)
        {
            if (null == value || 0 == value.Length)
                return false;

            if (MongoRelationalOperate.Between == rop && 2 > value.Length)
                return false;

            switch ((int)rop)
            {
                case 0: this.condition = key + ":" + value[0]; break;
                case 1: this.condition = key + ":" + "{$gt:'" + value[0] + "'}"; break;
                case 2: this.condition = key + ":" + "{$lt:'" + value[0] + "'}"; break;
                case 3: this.condition = key + ":" + "{$gte:'" + value[0] + "'}"; break;
                case 4: this.condition = key + ":" + "{$lte:'" + value[0] + "'}"; break;
                case 5: this.condition = key + ":" + "{$ne:'" + value[0] + "'}"; break;
                case 6: this.condition = key + ":" + "{$gt:'" + value[0] + ",$lt:" + value[1] + "'}"; break;
                case 7: this.condition = key + ":" + "{$in:" + value + "}"; break;
                case 8: this.condition = key + ":" + "{$nin:" + value + "}"; break;
                default: this.condition = ""; break;
            }

            return true;
        }
        public static MongoFilterCondition operator &(MongoFilterCondition lp, MongoFilterCondition rp)
        {
            MongoFilterCondition fc = new MongoFilterCondition();
            fc.condition = lp.condition + "," + rp.condition;
            return fc;
        }
        public static MongoFilterCondition operator |(MongoFilterCondition lp, MongoFilterCondition rp)
        {
            MongoFilterCondition fc = new MongoFilterCondition();
            fc.condition = "$or: [{" + lp.condition + "},{" + rp.condition + "}]";
            return fc;
        }
    }

    public enum MongoRelationalOperate
    {
        Equal = 0,
        Greater = 1,
        Less = 2,
        GreaterOrEqual = 3,
        LessOrEqual = 4,
        NotEqual = 5,
        Between = 6,
        In = 7,
        NotIn = 8,
        Like = 9
    }

    public enum MongoLogicalOperate
    {
        And = 0,
        Or = 1
    }


}
