﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using AspNetRunPeriodicTask.PeriodicTask;

namespace AspNetRunPeriodicTask
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var cancellationToken = new CancellationToken();
            var scheduledTaskDeamon = new Task(token =>
            {
                var taskList = new IPeriodicTask[]
                {
                    new SendEmailPeriodicTask(),
                    new CheckDatabasePeriodicTask()
                };
                foreach (var scheduledTask in taskList)
                {
                    var timer = new System.Timers.Timer(scheduledTask.RunInterval);
                    timer.Elapsed += (o, args) =>
                    {
                        scheduledTask.TaskStart();
                        timer.Start();
                    };
                    timer.AutoReset = false;
                    timer.Start();
                }
            }, cancellationToken);
            scheduledTaskDeamon.Start();
        }
    }
}