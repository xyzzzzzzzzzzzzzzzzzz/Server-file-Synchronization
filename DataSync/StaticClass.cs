using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataSync
{
   public static class StaticClass
    {

       /// <summary>
       /// 本地存储文件夹
       /// </summary>
       public static string localpath = "";
       /// <summary>
       /// 本地存储文件夹
       /// </summary>
       public static string cloudpath = "/";

       public static string ip;

       public static string user;
       public static string password;
       public static string prot ;

       public static string Cron;
       public static string Cron1;
       public static string ip1;

       public static string user1;
       public static string password1;
       public static string prot1;

       public static string autodelete;

       public static string startingup;


       /// <summary>
       /// 本地备份文件夹
       /// </summary>
       public static string localbkpath = "";

       /// <summary>
       /// 云端备份存储文件夹
       /// </summary>
       public static string cloudbkpath = "/";

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="lpAppName"></param>
        /// <param name="lpKeyName"></param>
        /// <param name="lpString"></param>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpString, string lpFileName);
        /// <summary>
        /// 读取ini配置文件
        /// </summary>
        /// <param name="Section">段</param>
        /// <param name="Key">键名</param>
        /// <param name="def">默认值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>读出内容</returns>
        public static string ReadValueFromIniFile(string Section, string Key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, def, temp, 255, filePath);
            return temp.ToString();
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        /// <summary>
        /// 更新combox事件
        /// </summary>
        public delegate void ShowLog(string value);
        //定义事件
        public static event ShowLog ShowLogClick;
        public static void ShowLog_Method(string value)
        {
            if (ShowLogClick != null)
            {
                ShowLogClick(value);
            }
        }
   }
}
