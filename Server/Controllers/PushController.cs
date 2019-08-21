﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Server.Models;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Text;

namespace Server.Controllers
{
    [Route("[controller]")]
    public class PushController : Controller
    {
        private DBHelper _db;
        public PushController(DBHelper dBHelper)
        {
            _db = dBHelper;
            
        }
        [HttpPost]
        public ActionResult<Result> Post([FromBody]MessageModel messageModel)
        {
            var memery = new MemoryStream();
            Request.EnableRewind();
            Request.Body.Position = 0;
            Request.Body.CopyTo(memery);
            memery.Position = 0;

            //MessageModel messageModel = JsonConvert.DeserializeObject<MessageModel>(message);
            string recData = new StreamReader(memery, UTF8Encoding.UTF8).ReadToEnd();
            DateTime time = ParseTime(double.Parse(messageModel.zd));
            string sql = $"IF NOT EXISTS (SELECT DeviceId from dbo.GuangFuRec where DeviceId = '{messageModel.g}') INSERT INTO dbo.GuangFuRec (GuangFuRecId,DeviceId,Time,RecData) VALUES({int.Parse(messageModel.za)},'{messageModel.g}','{time.ToString()}','{recData}') else update dbo.GuangFuRec set Time = '{time.ToString()}',RecData='{recData}' where DeviceId = '{messageModel.g}'";
            try
            {
                if (_db.ExecuteNonQuery(CommandType.Text, sql) == 1)
                {
                    return new ActionResult<Result>(new Result() { Status = "ACCEPTED" });
                }
                else
                {
                    return new ActionResult<Result>(new Result() { Status = "NODATA" });
                }
            }
            catch(Exception ex)
            {
                return new StatusCodeResult(503);
            }         
        }
        private DateTime ParseTime(double utc)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            startTime = startTime.AddSeconds(utc);

            startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            

            return startTime;
        }
    }

    
}