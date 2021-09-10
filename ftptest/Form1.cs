using FluentFTP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftptest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
              
                var client = Connect();

                try
                {
                    //using (FileStream fs = new FileStream(localPath, FileMode.Open))
                    //{
                    //    string path = localPath.Substring(localPath.LastIndexOf('\\') + 1);       //取文件名
                    //    bool flag = client.Upload(fs, path);
                    //}
                    MessageBox.Show("手动备份成功！");
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                }
                finally
                {
                 
                }
            }
            catch (Exception ee)
            {
              
                throw;
            }
        }
        public FtpClient Connect()
        {

            FtpClient client = new FtpClient("47.105.53.70");
            client.DataConnectionType = FtpDataConnectionType.PORT;
            client.Encoding = System.Text.Encoding.Default;
            client.DownloadDataType = FtpDataType.Binary;//二进制文件下载才能获取文件大小
            client.UploadRateLimit = 500;//上传速度 kb
            client.DownloadRateLimit = 0;//下载速度 kb
            // begin connecting to the server
            client.RetryAttempts = 3;//重试次数
            // specify the login credentials, unless you want to use the "anonymous" user account   ftpuser1"xyz1229-"
            client.Credentials = new NetworkCredential("ftpuser1","xyz1229-");

            // begin connecting to the server
            client.Connect();
            //StaticClass.cloudpath
            // get a list of files and directories in the "/htdocs" folder
            client.SetWorkingDirectory("/hpfile");
            return client;
        }
       

    }

}
