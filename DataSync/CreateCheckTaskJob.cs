
using DataSync;
using FluentFTP;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
   public class CreateCheckTaskJob:IJob
    {
       static string lsPath = "";
       public void Execute(IJobExecutionContext context)
       {
           LogHelper.WriteDebugLog(typeof(CreateCheckTaskJob), "自动备份");
           var client = Connect();

           //string remoteDic = "/hpfile";
           //// get a list of files and directories in the "/htdocs" folder
           //client.SetWorkingDirectory(remoteDic);


           //获取当前程序所在的文件路径
           String rootPath = Directory.GetCurrentDirectory();
           string colodPath = StaticClass.cloudpath;
           String localPath = StaticClass.localpath;
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
               sw.WriteLine("--------------------------------自动备份成功--------------------------------------------");
               sw.WriteLine("根目录：" + localPath + "   时间：" + DateTime.Now);
               getDirectory(sw, client, localPath, colodPath, 2);
               StaticClass.ShowLog_Method("自动备份成功！"+DateTime.Now);
               LogHelper.WriteInfoLog("自动备份成功！");
              
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
       public FtpClient Connect()
       {

           //var ftpClient = new FtpClient("47.105.53.70", 21, "ftpuser1", "xyz1229-")
           //{
           //    EncryptionMode = FtpEncryptionMode.None,
           //    DataConnectionType = FtpDataConnectionType.PORT,
           //    Encoding = System.Text.Encoding.Default
           //};
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

           string remoteDic = "/hpfile";
           // get a list of files and directories in the "/htdocs" folder
           client.SetWorkingDirectory(remoteDic);
           //var fff=client.GetListing(remoteDic).Count();
           //var fffff = client.GetListing(remoteDic);
           //FtpListItem[] ddd = new FtpListItem[fff];
           return client;
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
                   string localpath = path + "\\" + item.Name;
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
       public static void getDirectory(StreamWriter sw, FtpClient client, string path, string remoteDic, int indent)
       {

           var list = getFileName(sw, client, path, remoteDic, indent);

           foreach (FtpListItem d in list)
           {

               if (d.Type == FtpFileSystemObjectType.Directory)
               {
                   for (int i = 0; i < indent; i++)
                   {
                       sw.Write("  ");
                   }
                   string Dirlpath = path + "\\" + d.Name;
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
    }
}
