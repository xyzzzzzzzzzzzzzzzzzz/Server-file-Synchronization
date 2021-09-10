using FluentFTP;
using MES.ComClass;
using Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void UIenble(bool value)
        {
            textBox1.Enabled = value;
            textBox2.Enabled = value;
            textBox3.Enabled = value;
            textBox4.Enabled = value;
            textBox5.Enabled = value;
            textBox6.Enabled = value;
            textBox7.Enabled = value;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ShowState("开始安装服务");
            try
            {
                string CurrentDirectory = System.Environment.CurrentDirectory;
                System.Environment.CurrentDirectory = CurrentDirectory;
                ShowState("查找服务目录");
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Install.bat";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                System.Environment.CurrentDirectory = CurrentDirectory;
                ShowState("服务安装完成。");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                ShowState("安装服务出现错误：" + msg);
            }
        }
        private void ShowState(string message)
        {
            textBox2.Text = message;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowState("开始卸载服务");
            try
            {
                string CurrentDirectory = System.Environment.CurrentDirectory;
                System.Environment.CurrentDirectory = CurrentDirectory;
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Uninstall.bat";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                System.Environment.CurrentDirectory = CurrentDirectory;
                ShowState("服务卸载完成。");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                ShowState("卸载服务出现错误：" + msg);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                ShowState("启动服务开始");
                ServiceController serviceController = new ServiceController("hpCloudDataSync");
                serviceController.Start();
                ShowState("启动服务完成。");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                ShowState("启动服务出现错误：" + msg);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {

                ShowState("停止服务开始");
                ServiceController serviceController = new ServiceController("hpCloudDataSync");
                if (serviceController.CanStop)
                    serviceController.Stop();

                ShowState("停止服务完成。");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += ex.InnerException.Message;
                ShowState("停止服务出现错误：" + msg);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StaticClass.ShowLogClick += ShowInfo;
            StaticClass.localpath = StaticClass.ReadValueFromIniFile("localpath", "value", "null", Application.StartupPath + @"\config.ini");
            textBox1.Text =StaticClass.localpath;
           

            StaticClass.cloudpath = StaticClass.ReadValueFromIniFile("cloudpath", "value", "null", Application.StartupPath + @"\config.ini");
            textBox3.Text = StaticClass.cloudpath;
       

            StaticClass.Cron = StaticClass.ReadValueFromIniFile("Cron", "value", "null", Application.StartupPath + @"\config.ini");
            textBox4.Text = StaticClass.Cron;


            StaticClass.ip = StaticClass.ReadValueFromIniFile("ip", "value", "null", Application.StartupPath + @"\config.ini");
            textBox2.Text = md5.MD5Decrypt(StaticClass.ip);
            StaticClass.ip = md5.MD5Decrypt(StaticClass.ip);

            StaticClass.user = StaticClass.ReadValueFromIniFile("user", "value", "null", Application.StartupPath + @"\config.ini");
            textBox5.Text = md5.MD5Decrypt(StaticClass.user);
            StaticClass.user = md5.MD5Decrypt(StaticClass.user);

            StaticClass.password = StaticClass.ReadValueFromIniFile("password", "value", "null", Application.StartupPath + @"\config.ini");
            textBox6.Text = md5.MD5Decrypt(StaticClass.password);
            StaticClass.password = md5.MD5Decrypt(StaticClass.password);

            StaticClass.prot = StaticClass.ReadValueFromIniFile("prot", "value", "null", Application.StartupPath + @"\config.ini");
            textBox7.Text = StaticClass.prot;
   

        }
        private void ShowInfo(string msg)
        {
            this.BeginInvoke((Action)(() =>
            {
               
                if (listView1.Items.Count > 10)
                {
                    listView1.Items.Clear();
                }
                listView1.Items.Add(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
                listView1.EnsureVisible(listView1.Items.Count-1);

                


                //textBox2.AppendText(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
                //textBox2.AppendText(Environment.NewLine);
                //textBox2.ScrollToCaret();
            }));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public FtpClient Connect()
        {
           
            FtpClient client = new FtpClient(StaticClass.ip);
            client.DataConnectionType = FtpDataConnectionType.PORT;
            client.Encoding = System.Text.Encoding.Default;
            client.DownloadDataType = FtpDataType.Binary;//二进制文件下载才能获取文件大小
            client.UploadRateLimit = 500;//上传速度 kb
            client.DownloadRateLimit = 0;//下载速度 kb
            // begin connecting to the server
            client.RetryAttempts = 3;//重试次数
            // specify the login credentials, unless you want to use the "anonymous" user account   ftpuser1"xyz1229-"
            client.Credentials = new NetworkCredential(StaticClass.user, StaticClass.password);

            // begin connecting to the server
            client.Connect();
            //StaticClass.cloudpath
            // get a list of files and directories in the "/htdocs" folder
            client.SetWorkingDirectory(StaticClass.cloudpath);
            return client;
        }
        static string lsPath = "";
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                UIenble(false);
                var client = Connect();

                //string remoteDic = "/hpfile";
                //// get a list of files and directories in the "/htdocs" folder
                //client.SetWorkingDirectory(remoteDic);


                //获取当前程序所在的文件路径
                String rootPath = Directory.GetCurrentDirectory();
                string colodPath = StaticClass.cloudpath;
                String localPath = textBox1.Text;
                lsPath = localPath;
                //string parentPath = Directory.GetParent(rootPath).FullName;//上级目录
                //string topPath = Directory.GetParent(parentPath).FullName;//上上级目录
                StreamWriter sw = null;
                try
                {
                    string log_file = rootPath + "\\logs\\" + string.Format("[sync]{0:yyyyMMdd}", DateTime.Now) + ".txt";
                    if (!File.Exists(log_file))
                    {
                        FileStream fs = new FileStream(log_file, FileMode.CreateNew);
                        sw = new StreamWriter(fs);
                    }
                    else
                    {
                        FileStream fs = new FileStream(log_file, FileMode.Append);
                        //存在
                        sw = new StreamWriter(fs);
                    }
                    //创建输出流，将得到文件名子目录名保存到txt中
                    sw.WriteLine("--------------------------------手动备份成功--------------------------------------------");
                    sw.WriteLine("根目录：" + localPath + "   时间：" + DateTime.Now);
                    getDirectory(sw, client, localPath, colodPath, 2);
                    LogHelper.WriteInfoLog("手动备份成功！");
                    ShowInfo("手动备份成功!");
                    MessageBox.Show("手动备份成功！");
                    UIenble(true);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                        Console.WriteLine("完成");
                    }
                }
            }
            catch (Exception ee)
            {
                LogHelper.WriteErrorLog(typeof(Form1), ee.Message);
                throw;
            }
            
        }

        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="sw">文件写入流</param>
        /// <param name="path">文件写入流</param>
        /// <param name="indent">输出时的缩进量</param>
        public static FtpListItem[] getFileName(StreamWriter sw, FtpClient client, string path, string remoteDic, int indent)
        {
            var list = client.GetListing(remoteDic);
            //FtpListItem[] listitem = new FtpListItem[list.Count()];
            foreach (FtpListItem item in list)
            {
                // if this is a file
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    for (int i = 0; i < indent; i++)
                    {
                        sw.Write("  ");
                    }
                    string localpath = path +"\\"+ item.Name;
                    lsPath = localpath;
                    if (!File.Exists(lsPath))
                    {
                        //不存在
                        DownloadFile(client, lsPath, item.FullName);
                        sw.WriteLine("[Created]" + item.Name + "Size:" + item.Size + "      RawModified:" + item.RawModified);
                    }
                    else
                    {
                        //存在
                        sw.WriteLine("[Exist]" + item.Name + "Size:" + item.Size + "      RawModified:" + item.RawModified);
                    }
                }
            }
            return list;

            //DirectoryInfo root = new DirectoryInfo(path);
            //foreach (FileInfo f in root.GetFiles())
            //{
            //    for (int i = 0; i < indent; i++)
            //    {
            //        sw.Write("  ");
            //    }
            //    sw.WriteLine(f.Name);
            //}
        }

        /// <summary>
        /// 获得指定路径下所有子目录名
        /// </summary>
        /// <param name="sw">文件写入流</param>
        /// <param name="path">文件夹路径</param>
        /// <param name="indent">输出时的缩进量</param>
        public static void getDirectory(StreamWriter sw, FtpClient client, string path,string remoteDic, int indent)
        {
            
           var list= getFileName(sw, client, path, remoteDic, indent);

           foreach (FtpListItem d in list)
            {
               
                if (d.Type == FtpFileSystemObjectType.Directory)
                {
                    for (int i = 0; i < indent; i++)
                    {
                        sw.Write("  ");
                    }
                    string Dirlpath = path +"\\"+ d.Name;
                    lsPath = Dirlpath;
                    if (Directory.Exists(Dirlpath))
                    {
                        //存在
                        sw.WriteLine("[Exist]" + d.Name + "Size:" + d.Size + "      RawModified:" + d.RawModified);
                    }
                    else
                    {
                        sw.WriteLine("[Created]" + d.Name + "Size:" + d.Size + "      RawModified:" + d.RawModified);
                        DirectoryInfo directoryInfo = new DirectoryInfo(Dirlpath);
                        directoryInfo.Create();
                    }

                    sw.WriteLine("文件夹：" + d.Name);
                    getDirectory(sw, client, lsPath, d.FullName, indent + 2);
                    sw.WriteLine();
                }
            }
        }

        /// <summary>
        /// FTP服务器文件下载到本地
        /// </summary>
        /// <param name="ftphost">ftp地址：
        /// <param name="user">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        /// <param name="saveLocalPath">下载到本地的地址：d:\\doctument\\0F5GAHRT4A484TRA5D15FEA.pdf</param>
        /// <param name="downPath">将要下载的文件在FTP上的路径：/DownFile/0F5GAHRT4A484TRA5D15FEA</param>
        static void DownloadFile(FtpClient client, string saveLocalPath, string downPath)
        {
            byte[] outBuffs;
            bool flag = client.Download(out outBuffs, downPath);

            string s = saveLocalPath.Substring(0, saveLocalPath.LastIndexOf('\\'));
            Directory.CreateDirectory(s);//如果文件夹不存在就创建它

            FileStream fs = new FileStream(saveLocalPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(outBuffs, 0, outBuffs.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strPath = System.IO.Directory.GetCurrentDirectory();

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            dialog.SelectedPath = strPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                textBox1.Text = dialog.SelectedPath;
                System.IO.Directory.SetCurrentDirectory(dialog.SelectedPath);
               
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请先设置一个目录！"); return;
            }
            System.Diagnostics.Process.Start(textBox1.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
          
        }

        private void button12_Click(object sender, EventArgs e)
        {
           
        }

        private void button7_Click(object sender, EventArgs e)
        { 

            PlanRegistry ss = new PlanRegistry();
            if (button7.Text == "启动定时备份")
            {
                UIenble(false);
                ss.Start();
                button7.Text = "停止定时备份";
                LogHelper.WriteInfoLog("启动定时备份");
            }
            else
            {
                UIenble(true);
                ss.ClearJobTrigger();
                button7.Text = "启动定时备份";
                LogHelper.WriteInfoLog("停止定时备份");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {

                Regex regex = new Regex(@"^([a-zA-Z]:\\)?[^\/\:\*\?\""\<\>\|\,]*$");
                Match m = regex.Match(textBox1.Text);
                if (!m.Success)
                {
                    MessageBox.Show("非法的文件保存路径，请重新选择或输入！");
                    return;
                }
                if (textBox3.Text == "")
                {
                    MessageBox.Show("ftp目录不能为空！");
                    return;
                }




                StaticClass.localpath = textBox1.Text;
                StaticClass.WritePrivateProfileString("localpath", "value",StaticClass.localpath, Application.StartupPath + @"\config.ini");

                StaticClass.cloudpath = textBox3.Text;
                StaticClass.WritePrivateProfileString("cloudpath", "value", StaticClass.cloudpath, Application.StartupPath + @"\config.ini");


                StaticClass.ip = textBox2.Text;
                StaticClass.WritePrivateProfileString("ip", "value", md5.MD5Encrypt(StaticClass.ip), Application.StartupPath + @"\config.ini");
                StaticClass.user = textBox5.Text;
                StaticClass.WritePrivateProfileString("user", "value", md5.MD5Encrypt(StaticClass.user), Application.StartupPath + @"\config.ini");
                StaticClass.password = textBox6.Text;
                StaticClass.WritePrivateProfileString("password", "value", md5.MD5Encrypt(StaticClass.password), Application.StartupPath + @"\config.ini");
                StaticClass.prot = textBox7.Text;
                StaticClass.WritePrivateProfileString("prot", "value",StaticClass.prot, Application.StartupPath + @"\config.ini");

                StaticClass.Cron = textBox4.Text;
                StaticClass.WritePrivateProfileString("Cron", "value", StaticClass.Cron, Application.StartupPath + @"\config.ini");


                LogHelper.WriteInfoLog("保存配置参数！");
                MessageBox.Show("保存成功！");
            }
            catch (Exception ee)
            {
                LogHelper.WriteErrorLog(typeof(Form1), ee.Message);
                throw;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        } 
    }
}
