using MVC_Test.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MVC_Test.Common
{
    public class KeepAwake
    {
        private static readonly string url = "http://localhost:8080/Home/Index";
        //public async static Task KeepAwakeSchedule()
        //{
        //    IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        //    if (!scheduler.IsStarted) await scheduler.Start();

        //    IJobDetail job = JobBuilder.Create<KeepAwakeJob>()
        //        .WithIdentity("Job_KeepAwake", "WriteLogJobGroup")
        //        .Build();

        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity("Trigger__KeepAwake", "WriteLogJobGroup")
        //        .StartNow()
        //        //.WithCronSchedule(cron)
        //        .WithSimpleSchedule(x => x
        //            .WithIntervalInMinutes(5)
        //            .RepeatForever())
        //        .Build();

        //    await scheduler.ScheduleJob(job, trigger);
        //}

        public static void PingServer()
        {
            try
            {
                WebClient http = new WebClient();
                string result = http.DownloadString(url);
                Log.WriteTextAsync("PingServer done. " + result.Substring(0, 15)).Wait();
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
            }
        }
    }
}