using MVC_Test.Common;
using MVC_Test.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC_Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<object> RunJob(string jobName, string cron = "30 0/1 * 1/1 * ? *")
        {
            // get a scheduler
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            if(!scheduler.IsStarted) await scheduler.Start();

            IJobDetail job = JobBuilder.Create<WriteLogJob>()
                .WithIdentity("MVCTestHomeIndexJob_" + jobName, "MVCTestHomeIndexJobGroup")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MVCTestHomeIndexTrigger_" + jobName, "MVCTestHomeIndexJobGroup")
                .StartNow()
                //.WithCronSchedule(cron)
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();

            try
            {
                await scheduler.ScheduleJob(job, trigger);
                string result = string.Format("Job name: {0}, State: {1}, Next Fire Time: {2}", job.Key.Name, scheduler.GetTriggerState(trigger.Key).Result.ToString(), trigger.GetNextFireTimeUtc().Value.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss"));
                //await Log.WriteTextAsync("Job added");
                //await Log.WriteJobInfo();
                return Json(new { code = 1, result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (SchedulerException ex)
            {
                return Json(new { code = 1, result = "Error: " + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<object> GetJob()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            var groupNames = await scheduler.GetJobGroupNames();
            //List<IReadOnlyCollection<JobKey>> ListJobKeys = new List<IReadOnlyCollection<JobKey>>();
            List<object> result = new List<object>();
            foreach (var group in groupNames)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = await scheduler.GetJobKeys(groupMatcher);
                //ListJobKeys.Add(jobKeys);
                foreach (var jobKey in jobKeys)
                {
                    var detail = await scheduler.GetJobDetail(jobKey);
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    foreach (ITrigger trigger in triggers)
                    {
                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();

                        DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                        result.Add(new { Group = group, JobKeyName = jobKey.Name, Description = detail.Description, TriggerKeyName = trigger.Key.Name, TriggerKeyGroup = trigger.Key.Group, TriggerState = scheduler.GetTriggerState(trigger.Key), NextFireTime = nextFireTime == null ? null : nextFireTime.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), PreviousFireTime = previousFireTime == null ? null : previousFireTime.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") });
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}