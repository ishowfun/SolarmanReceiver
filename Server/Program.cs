using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)    
                                          .AddJsonFile("appsettings.json")                                         
                                          .Build();

            var url = configuration["host"];
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hc,logging)=>
                {
                    logging.AddFilter("System", LogLevel.Warning);
                    logging.AddFilter("MircroSoft", LogLevel.Warning);
                    logging.AddLog4Net();
                })
                .UseUrls(url)
                .UseStartup<Startup>();
        }
    }
}
