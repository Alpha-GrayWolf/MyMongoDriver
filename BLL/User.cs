using Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandParser;
using Newtonsoft.Json;
using StackExchange.Redis;
using MongoDB.Driver;
using Utility;

namespace BLL
{
    public class User
    {
        public static void Create(string pData, out CmdResponsePacket pResponse)
        {
            pResponse = new CmdResponsePacket();
            pResponse.code = (int)CmdResponseMessage.Failed;
            pResponse.success = false;

            UserEntity ue = JsonConvert.DeserializeObject<UserEntity>(pData);
            if (null == ue)
                return;

            object result;
            if (CmdResponseMessage.Success != Create(ue, out result))
                return;

            pResponse.data = result;
            pResponse.success = true;
            pResponse.code = (int)CmdResponseMessage.Success;
            pResponse.message = "添加成功";
        }
        public static void Retrieve(string pData, out CmdResponsePacket pResponse)
        {
            pResponse = new CmdResponsePacket();

            UserSEntity use = JsonConvert.DeserializeObject<UserSEntity>(pData);

            object result;
            Retrieve(use, out result);

            pResponse.data = result;
            pResponse.success = true;
            pResponse.code = 200;
            pResponse.message = "查询成功";
        }
        public static void Update(string pData, out CmdResponsePacket pResponse)
        {
            pResponse = new CmdResponsePacket();
            pResponse.code = (int)CmdResponseMessage.Failed;
            pResponse.success = false;

            UserEntity ue = JsonConvert.DeserializeObject<UserEntity>(pData);
            if (null == ue)
                return;

            object result;
            if (CmdResponseMessage.Success != Update(ue, out result))
                return;

            pResponse.data = result;
            pResponse.success = true;
            pResponse.code = (int)CmdResponseMessage.Success;
            pResponse.message = "修改成功";
        }
        public static void Delete(string pData, out CmdResponsePacket pResponse)
        {
            pResponse = new CmdResponsePacket();
            pResponse.success = false;
            pResponse.code = (int)CmdResponseMessage.Failed;
            pResponse.message = "删除失败";

            if (string.IsNullOrEmpty(pData))
                return;

            if (CmdResponseMessage.Success != Delete(pData))
                return;

            pResponse.success = true;
            pResponse.code = 200;
            pResponse.message = "删除成功";
        }

        private static CmdResponseMessage Create(UserEntity pEntity, out object pResult)
        {
            pResult = null;
            pEntity.Id = UserD.GetAutoId();

            if ("-1" == pEntity.Id)
                return CmdResponseMessage.Failed;

            pEntity.CreateTime = DateTime.Now.ToString();


            CmdResponseMessage msg;
            if (CmdResponseMessage.Success != (msg = UserD.Create(pEntity)))
                return msg;

            pResult = pEntity;
            return CmdResponseMessage.Success;
        }
        private static CmdResponseMessage Retrieve(UserSEntity pSEntity, out object pResult)
        {
            MongoFilterCondition filter = new MongoFilterCondition();

            if (!string.IsNullOrEmpty(pSEntity.Name))
                filter.AddFilterCondition("Name", pSEntity.Name, MongoRelationalOperate.Like);

            if (0 < pSEntity.Sex)
                filter.AddFilterCondition("Sex", pSEntity.Sex.ToString(), MongoRelationalOperate.Equal);

            if (!string.IsNullOrEmpty(pSEntity.NativePlace))
                filter.AddFilterCondition("NativePlace", pSEntity.NativePlace, MongoRelationalOperate.Like);

            if (0 < pSEntity.Occupation)
                filter.AddFilterCondition("Occupation", pSEntity.Occupation.ToString(), MongoRelationalOperate.Equal);

            if (!string.IsNullOrEmpty(pSEntity.Hobby))
                filter.AddFilterCondition("Hobby", pSEntity.Hobby, MongoRelationalOperate.Like);

            UserD.RetrievePage(pSEntity.PageIndex, pSEntity.PageSize, filter, out pResult);
            return CmdResponseMessage.Success;
        }
        private static CmdResponseMessage Update(UserEntity pEntity, out object pResult)
        {
            pResult = null;

            CmdResponseMessage msg;
            if (CmdResponseMessage.Success != (msg = UserD.Update(pEntity)))
                return msg;

            pResult = pEntity;
            return CmdResponseMessage.Success;
        }
        private static CmdResponseMessage Delete(string pUid)
        {
            CmdResponseMessage msg;
            if (CmdResponseMessage.Success != (msg = UserD.Delete(pUid)))
                return msg;

            return CmdResponseMessage.Success;
        }
    }

    internal class UserD
    {
        public static CmdResponseMessage Create(UserEntity pEntity)
        {
            try
            {
                IMongoCollection<UserEntity> collection = MongoHelper.GetMongoCollection<UserEntity>("db_base", "user");

                MongoHelper.Create<UserEntity>(collection, pEntity);

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage Retrieve(MongoFilterCondition pFilter, out object pResult)
        {
            pResult = null;

            try
            {
                IMongoCollection<UserEntity> collection = MongoHelper.GetMongoCollection<UserEntity>("db_base", "user");

                pResult = MongoHelper.Retrieve<UserEntity>(collection, pFilter);

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage Update(UserEntity pEntity)
        {
            try
            {
                IMongoCollection<UserEntity> collection = MongoHelper.GetMongoCollection<UserEntity>("db_base", "user");

                MongoHelper.Update<UserEntity>(collection, pEntity.Id, pEntity, "update");

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage Delete(string pUid)
        {
            try
            {
                if (string.IsNullOrEmpty(pUid))
                    return CmdResponseMessage.Failed;

                IMongoCollection<UserEntity> collection = MongoHelper.GetMongoCollection<UserEntity>("db_base", "user");

                MongoHelper.Delete<UserEntity>(collection, pUid);

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage RetrievePage(int pPageIndex, int pPageSize, MongoFilterCondition pFilter, out object pResult)
        {
            pResult = null;

            try
            {
                IMongoCollection<UserEntity> collection = MongoHelper.GetMongoCollection<UserEntity>("db_base", "user");

                pResult = MongoHelper.RetrievePage<UserEntity>(collection, pPageIndex, pPageSize, pFilter);

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }

        public static CmdResponseMessage CreateToRedis(UserEntity pEntity)
        {
            try
            {
                using (ConnectionMultiplexer conn = RedisHelper.GetRedisConnection())
                {
                    IDatabase db = conn.GetDatabase(0);

                    if (!RedisHelper.SaveEntity<UserEntity>(db, "user_" + pEntity.Id, pEntity, "create"))
                        return CmdResponseMessage.Failed;
                }

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage UpdateToRedis(UserEntity pEntity)
        {
            try
            {
                using (ConnectionMultiplexer conn = RedisHelper.GetRedisConnection())
                {
                    IDatabase db = conn.GetDatabase(0);

                    if (!RedisHelper.SaveEntity<UserEntity>(db, "user_" + pEntity.Id, pEntity, "update"))
                        return CmdResponseMessage.Failed;
                }

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }
        public static CmdResponseMessage DeleteToRedis(string pUid)
        {
            try
            {
                if (string.IsNullOrEmpty(pUid))
                    return CmdResponseMessage.Failed;

                using (ConnectionMultiplexer conn = RedisHelper.GetRedisConnection())
                {
                    IDatabase db = conn.GetDatabase(0);
                    if (!db.KeyExists("user_" + pUid))
                        return CmdResponseMessage.Failed;

                    db.KeyDelete("user_" + pUid);
                }

                return CmdResponseMessage.Success;
            }
            catch (Exception ex)
            {
                string tagMessage = ex.Message;
                return CmdResponseMessage.Failed;
            }
        }

        public static string GetAutoId()
        {
            try
            {
                using (ConnectionMultiplexer conn = RedisHelper.GetRedisConnection())
                {
                    var db = conn.GetDatabase(0);

                    long id = db.HashIncrement("auto_id", "user", 1);

                    return id.ToString();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return "-1";
            }
        }
    }
}
