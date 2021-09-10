using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
   public class CreateCheckTaskJob:IJob
    {
       ILog log = LogManager.GetLogger(typeof(CreateCheckTaskJob));
       public void Execute(IJobExecutionContext context)
       {
           log.Info("任务运行");

       }
    }
}
