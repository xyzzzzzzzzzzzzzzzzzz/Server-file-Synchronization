using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
   public  class PlanRegistry
    {
       public static void Start()
       {
           string QuestionCron = "0,10,21,32,43,54 * 10 * * ?";
           //string QuestionCron = ConfigurationManager.AppSettings["quarterCon"] == null ? "0 0/1 * * * ? *" : ConfigurationManager.AppSettings["quarterCon"];

           ////新建一个调度器工工厂
           ISchedulerFactory factory = new StdSchedulerFactory();
           ////使用工厂生成一个调度器
           IScheduler scheduler = factory.GetScheduler();
           //启动调度器
           scheduler.Start();
           IJobDetail job = JobBuilder.Create<CreateCheckTaskJob>().Build();
           // 新建一个触发器
           ITrigger trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(QuestionCron).Build();
           //将任务与触发器关联起来放到调度器中
           scheduler.ScheduleJob(job, trigger);
       }
    }
}
