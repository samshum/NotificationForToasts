using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationForToasts
{
    public class ExceptionProcess
    {
        #region 异常处理
        public void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ExceptionReceive(100, "任务调度异常", e.Exception, sender);
        }

        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionReceive(404, "未捕捉到异常", new Exception(e.ExceptionObject.ToString()), sender);
        }

        public void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionReceive(500, "未捕捉到线程异常", e.Exception, sender);
            //e.Handled = true;
        }

        public string LogFolder {
            get {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/log/" + DateTime.Now.ToString("yyyyMM"));
            }
        }

        public string LogPath {
            get {
                return Path.Combine(LogFolder, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            }
        }

        public void CreateDir()
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        /// <summary>
        /// 异常处理方法
        /// </summary>
        /// <param name="exCode">异常代码</param>
        /// <param name="exTitle">异常标题</param>
        /// <param name="ers">异常类</param>
        /// <param name="obj">异常对象</param>
        public void ExceptionReceive(int exCode, string exTitle, Exception ers, object obj)
        {

            CreateDir();
            using (StreamWriter sr = new StreamWriter(new FileStream(LogPath, FileMode.OpenOrCreate | FileMode.Append), Encoding.UTF8))
            {
                sr.WriteLine("【" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "/" + exTitle + "】");
                sr.WriteLine("------" + exCode.ToString() + "----------------------------------------------------------");
                sr.WriteLine(JsonConvert.SerializeObject(obj));
                sr.WriteLine(ers.Message);
                sr.WriteLine(ers.Source);
                sr.WriteLine(ers.StackTrace);
                sr.WriteLine(ers.HResult);
                sr.WriteLine(ers.HelpLink);
                sr.Close();
                sr.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logtitle"></param>
        /// <param name="logcontent"></param>
        public void WriteLog(string logtitle, string logcontent)
        {
            CreateDir();
            using (StreamWriter sr = new StreamWriter(new FileStream(LogPath, FileMode.OpenOrCreate | FileMode.Append), Encoding.UTF8))
            {
                sr.WriteLine("【" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "/" + logtitle + "】");
                sr.WriteLine("------" + 200.ToString() + "----------------------------------------------------------");
                sr.WriteLine(logcontent);
                sr.Close();
                sr.Dispose();
            }
        }
    }
}
