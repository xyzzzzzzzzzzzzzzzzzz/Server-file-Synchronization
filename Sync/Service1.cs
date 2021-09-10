
using Common.Logging;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sync
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 服务启动的操作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                ILog log = LogManager.GetLogger(typeof(CreateCheckTaskJob));
                log.Info("------- Initializing -------------------");   
                //LogHelper
                //FilterConfig
                PlanRegistry.Start();
         
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
                throw ex;
            }
        }
       
        /// <summary>
        /// 服务停止的操作
        /// </summary>
        protected override void OnStop()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
            }
        }
    }
}
