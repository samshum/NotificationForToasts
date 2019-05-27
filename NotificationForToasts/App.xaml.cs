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
        private ExceptionProcess ep = new ExceptionProcess();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //异常处理
            AppDomain.CurrentDomain.UnhandledException += ep.CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += ep.TaskScheduler_UnobservedTaskException;
            this.DispatcherUnhandledException += ep.App_DispatcherUnhandledException;



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
