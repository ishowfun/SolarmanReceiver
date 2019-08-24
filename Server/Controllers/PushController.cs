using System;
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
using Microsoft.Extensions.Logging;

namespace Server.Controllers
{
    [Route("[controller]")]
    public class PushController : Controller
    {
        private DBHelper _db;
        private ILogger _logger;
        public PushController(DBHelper dBHelper, ILogger<PushController> logger)
        {
            _db = dBHelper;
            _logger = logger;
        }
        [HttpPost]
        public ActionResult<Result> Post([FromBody]dynamic[] messageModels)
        {
            try
            {
                foreach (var messageModel in messageModels)
                {
                    string ts = messageModel.zd;
                    DateTime time = ParseTime(double.Parse(ts));
                    string recData = JsonConvert.SerializeObject(messageModel);
                    recData = recData.Replace('\'', '_');
                    recData = recData.Replace("\",\"", "|");
                    recData = recData.Replace("\"", "").Trim(new char[] { '{','}'});
                    string idstr = messageModel.za;
                    int senssorId = int.Parse(idstr);
                    var sql = $"IF NOT EXISTS (SELECT DeviceId from dbo.GuangFuRec where GuangFuRecId = '{senssorId}') INSERT INTO dbo.GuangFuRec (GuangFuRecId,DeviceId,Time,RecData) VALUES({senssorId},'{messageModel.g}','{time.ToString()}','{recData}') else update dbo.GuangFuRec set Time = '{time.ToString()}',RecData='{recData}' where GuangFuRecId = '{senssorId}'";

                    if (_db.ExecuteNonQuery(CommandType.Text, sql) == 1)
                    {
                        
                    }
                    else
                    {
                        return new ActionResult<Result>(new Result() { Status = "NODATA" });
                    }
                }
                return new ActionResult<Result>(new Result() { Status = "ACCEPTED" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");                
                return new StatusCodeResult(503);
            }
        }
        private DateTime ParseTime(double utc)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            startTime = startTime.AddSeconds(utc);

            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            

            return startTime;
        }
    }


}