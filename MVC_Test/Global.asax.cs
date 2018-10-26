using MVC_Test.Common;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC_Test
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            //if (!scheduler.IsStarted) scheduler.Start().Wait();
            //KeepAwake.KeepAwakeSchedule().Wait();
            var jobStoreConfig = (NameValueCollection)ConfigurationManager.GetSection("quartz");
            ISchedulerFactory factory = new StdSchedulerFactory(jobStoreConfig);
            IScheduler scheduler = factory.GetScheduler().Result;
            if (!scheduler.IsStarted) scheduler.Start().Wait();
            Log.WriteTextAsync("App started").Wait();
        }

        void Application_End(object sender, EventArgs e)
        {
            // Force the App to be restarted immediately
            Log.WriteTextAsync("App stopped").Wait();
            KeepAwake.PingServer();
        }
    }
}
