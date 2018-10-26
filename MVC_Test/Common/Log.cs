using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVC_Test.Common
{
    public class Log
    {
        static readonly object o = new object();
        static readonly string path = AppDomain.CurrentDomain.BaseDirectory + @"\AppLogs.txt";

        public static void WriteText(string content)
        {
            lock (o)
            {
                using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss] ") + content);
                }
            }
        }

        public static async Task WriteTextAsync(string content)
        {
            using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                await sw.WriteLineAsync(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss] ") + content);
            }
        }

        public static async Task WriteTextAsync(string filePath, string content)
        {
            //byte[] encodedText = Encoding.Unicode.GetBytes(content);

            //using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            //{
            //    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            //};

            using (StreamWriter sw = (File.Exists(filePath)) ? File.AppendText(filePath) : File.CreateText(filePath))
            {
                await sw.WriteLineAsync(content);
            }
        }

        public static async Task WriteJobInfo()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/AppLogs.txt");

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            var jobGroupNames = await scheduler.GetJobGroupNames();
            if (jobGroupNames.Count <= 0)
            {
                await Log.WriteTextAsync(filePath, "Job(s) scheduled count: " + jobGroupNames.Count.ToString());
            }
            else
            {

                foreach (var group in jobGroupNames)
                {
                    var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                    var jobKeys = await scheduler.GetJobKeys(groupMatcher);
                    await Log.WriteTextAsync(filePath, "Job(s) scheduled count: " + jobKeys.Count.ToString());
                    foreach (var jobKey in jobKeys)
                    {
                        var detail = scheduler.GetJobDetail(jobKey);
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        foreach (var trigger in triggers)
                        {
                            await Log.WriteTextAsync(filePath, string.Format("Group: {0}, Job name: {1}, State: {2}, Next Fire Time: {3}", group, jobKey.Name, scheduler.GetTriggerState(trigger.Key).Result.ToString(), trigger.GetNextFireTimeUtc().Value.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss")));
                        }
                    }
                }
            }
        }
    }
}