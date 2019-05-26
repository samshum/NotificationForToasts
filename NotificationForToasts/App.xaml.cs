using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace NotificationForToasts
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            //启动时检测是否已有程序启动，如果存在当前不启动
            string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcesses();
            Process currentProcess = Process.GetCurrentProcess();
            Process searchProcess = null;
            bool appisExist = false;
            for (int i = 0; i < processes.Length; i++)
            {
                if (processes == null || currentProcess.Id == processes[i].Id) continue;
                if (processes[i].ProcessName == strProcessName)
                {
                    searchProcess = processes[i];
                    appisExist = true;
                    continue;
                }
            }
            if (appisExist)
            {
                HandleRunningInstance(searchProcess);
                currentProcess.Kill();
            }
            else
            {
                new MainWindow().Show();
            }
        }

        #region 异常处理
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ProcessException(100, "任务调度异常", e.Exception, sender);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProcessException(404, "未捕捉到异常", new Exception(e.ExceptionObject.ToString()), sender);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ProcessException(500, "未捕捉到线程异常", e.Exception, sender);
            //e.Handled = true;
        }

        /// <summary>
        /// 异常处理方法
        /// </summary>
        /// <param name="exCode">异常代码</param>
        /// <param name="exTitle">异常标题</param>
        /// <param name="ers">异常类</param>
        /// <param name="obj">异常对象</param>
        private void ProcessException(int exCode, string exTitle, Exception ers, object obj)
        {
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/log/" + DateTime.Now.ToString("yyyyMM"));
            string logPath = Path.Combine(logFolder, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            using (StreamWriter sr = new StreamWriter(new FileStream(logPath, FileMode.OpenOrCreate|FileMode.Append)))
            {
                sr.WriteLine("【"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"/"+exTitle+"】");
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
         
        // 已经有了就把它激活，并将其窗口放置最前端
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, 1); //调用api函数，正常显示窗口
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);
    }
}
