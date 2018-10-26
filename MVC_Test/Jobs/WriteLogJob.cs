using MVC_Test.Common;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MVC_Test.Jobs
{
    public class WriteLogJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //Debug.WriteLine(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss] ") + "Job executing");
            //Debug.WriteLine(Environment.CurrentDirectory);
            //Debug.WriteLine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            //Debug.WriteLine(Thread.GetDomain().BaseDirectory);
            //Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());
            //await Log.WriteTextAsync("D:/Logs.txt", "Job executed");
            await Log.WriteTextAsync("Job executed");
        }
    }
}