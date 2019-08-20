using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server.Controllers
{
    [Route("[controller]")]
    public class PushController : Controller
    {
        [HttpPost]
        public ActionResult<Result> Post([FromBody]MessageModel messageModel)
        {

            return new Result();
        }
    }
}