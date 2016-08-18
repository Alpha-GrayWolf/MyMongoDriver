using System.Threading.Tasks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using CommandParser;

namespace MyWebApi.Controllers
{
    public class UserController : Controller
    {
        [HttpPost]
        public string Create(string param)
        {
            return CmdHandler.CmdExecute(param, BLL.User.Create);
        }

        [HttpPost]
        public string Retrieve(string param)
        {
            return CmdHandler.CmdExecute(param, BLL.User.Retrieve);
        }

        [HttpPost]
        public string Update(string param)
        {
            return CmdHandler.CmdExecute(param, BLL.User.Update);
        }

        [HttpPost]
        public string Delete(string param)
        {
            return CmdHandler.CmdExecute(param, BLL.User.Delete);
        }
    }
}