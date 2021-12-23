using DataSync;
using log4net;
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
       private static readonly string tiggerName = "TestJobTrigger";
       private static readonly string gropName = "TestJobTriggerGrop";
       private static readonly string jobName = "TestJob";

        private static readonly string tiggerName1 = "MicrovuJobTrigger";
        private static readonly string gropName1 = "MicrovuJobTriggerGrop";
        private static readonly string jobName1 = "MicrovuJob";

        private static readonly string  QuestionCron = "0,10,21,32,43,54 * 10 * * ?";
        //从工厂中获取一个调度器实例化
        private static IScheduler scheduler = null;

        //从工厂中获取一个调度器实例化
        private static IScheduler scheduler1 = null;
       public  void Start()
       {
           MicrovuJobStart();
         
       }
       public void Start1()
       {
           MicrovuJobStart1();

       }
       private static  void MicrovuJobStart()
       {
           //从工厂中获取一个调度器实例化
           scheduler =  StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();


           //创建一个作业
           IJobDetail job1 = JobBuilder.Create<CreateCheckTaskJob>()
            .WithIdentity(jobName1, gropName1)
            .UsingJobData("key", "MicrovuJob")// 传递参数 在Execute方法中获取（以什么类型值传入，取值就用相应的类型方法取值）
            .UsingJobData("key1", "MicrovuJob1")
            .Build();

           // 创建触发器
           ITrigger trigger1 = TriggerBuilder.Create()
                                       .WithIdentity(tiggerName1, gropName1)
                                       .StartNow()                        //现在开始
                                       .WithCronSchedule(StaticClass.Cron          //触发时间，10秒一次。
                                      )              //不间断重复执行
                                       .Build();
            scheduler.ScheduleJob(job1, trigger1);      //把作业，触发器加入调度器。
           // 清除任务和触发器
           //ClearJobTrigger();
       }
       private static void MicrovuJobStart1()
       {
           //从工厂中获取一个调度器实例化
           scheduler1 = StdSchedulerFactory.GetDefaultScheduler();
           scheduler1.Start();


           //创建一个作业
           IJobDetail job1 = JobBuilder.Create<CreateCheckTaskJob1>()
            .WithIdentity(jobName, gropName)
            .UsingJobData("key", "TestJob")// 传递参数 在Execute方法中获取（以什么类型值传入，取值就用相应的类型方法取值）
            .UsingJobData("key1", "TestJob1")
            .Build();

           // 创建触发器
           ITrigger trigger1 = TriggerBuilder.Create()
                                       .WithIdentity(tiggerName, gropName)
                                       .StartNow()                        //现在开始
                                       .WithCronSchedule(StaticClass.Cron1          //触发时间，10秒一次。
                                      )              //不间断重复执行
                                       .Build();
           scheduler.ScheduleJob(job1, trigger1);      //把作业，触发器加入调度器。
           // 清除任务和触发器
           //ClearJobTrigger();
       }
       /// <summary>
       /// 清除任务和触发器
       /// </summary>
       public  void ClearJobTrigger()
       {
           TriggerKey triggerKey = new TriggerKey(tiggerName1, gropName1);
           JobKey jobKey = new JobKey(jobName1, gropName1);
           if (scheduler != null)
           {
               scheduler.PauseTrigger(triggerKey);
               scheduler.UnscheduleJob(triggerKey);
               scheduler.DeleteJob(jobKey);
               scheduler.Shutdown();// 关闭
           }
       }
       public void ClearJobTrigger1()
       {
           TriggerKey triggerKey = new TriggerKey(tiggerName, gropName);
           JobKey jobKey = new JobKey(jobName, gropName);
           if (scheduler != null)
           {
               scheduler1.PauseTrigger(triggerKey);
               scheduler1.UnscheduleJob(triggerKey);
               scheduler1.DeleteJob(jobKey);
               scheduler1.Shutdown();// 关闭
           }
       }
    }
}
