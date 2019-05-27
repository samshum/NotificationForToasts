using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using NotificationForToasts.ShellHelpers;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotificationForToasts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //"58300WindowsNotifications.NotificationsVisualizer_8rkfj2ay7vd1w!App"
        //"Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge"
        private CreateShortcut shelp = new CreateShortcut("Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge");
        private System.Timers.Timer _time = new System.Timers.Timer();
        private ExceptionProcess _ep = new ExceptionProcess();
        private GetData getData = new GetData();
        public MainWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Minimized;

            showNotificationIcon();
            shelp.Do("NotificationForToasts");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
            e.Cancel = true;
        }

        #region 任务栏相关操作
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 更新订阅ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示最新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 缓存目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 跟随系统启动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看日志ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private void showNotificationIcon()
        {
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.显示最新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "显示最新",
                Font = new Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                Image = global::NotificationForToasts.Properties.Resources.p_shownew
            };
            this.更新订阅ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "更新订阅",
                Image = global::NotificationForToasts.Properties.Resources.p_updaterss
            };
            this.缓存目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "缓存目录"//,Image = global::NotificationForToasts.Properties.Resources.p_cachecat
            };
            this.跟随系统启动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem()
            {
                Text = "跟随系统启动"
            };
            this.查看日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "查看日志"
            };
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "关于我们"//,Image = global::NotificationForToasts.Properties.Resources.p_about
            };
            
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem() {
                Text = "退出应用",
                Image = global::NotificationForToasts.Properties.Resources.p_exit
            };

            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = global::NotificationForToasts.Properties.Resources.chart2;
            this.notifyIcon1.Text = "财经新闻订阅";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示最新ToolStripMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            this.更新订阅ToolStripMenuItem,
            this.缓存目录ToolStripMenuItem,
            this.查看日志ToolStripMenuItem,
            this.跟随系统启动ToolStripMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            this.关于ToolStripMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 114);

            this.更新订阅ToolStripMenuItem.MouseUp += 更新订阅ToolStripMenuItem_MouseUp;
            this.显示最新ToolStripMenuItem.MouseUp += 显示最新ToolStripMenuItem_MouseUp;
            this.缓存目录ToolStripMenuItem.MouseUp += 缓存目录ToolStripMenuItem_MouseUp;
            this.跟随系统启动ToolStripMenuItem.MouseUp += 跟随系统启动ToolStripMenuItem_MouseUp;
            this.查看日志ToolStripMenuItem.MouseUp += 查看日志ToolStripMenuItem_MouseUp;
            this.关于ToolStripMenuItem.MouseUp += 关于ToolStripMenuItem_MouseUp;
            this.退出ToolStripMenuItem.MouseUp += 退出ToolStripMenuItem_MouseUp;
        }


        private void NotifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ShowNotification(getData.GetTop1News(), shelp.AppID);
        }

        private void 退出ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void 关于ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Process.Start("https://github.com/samshum/NotificationForToasts");
        }
        private void 查看日志ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (File.Exists(_ep.LogPath))
            {
                Process.Start(_ep.LogPath);
            }
        }

        private void 跟随系统启动ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(getStartBySystem()))
            {
                setStartBySystem(true);
            }
        }

        private void 缓存目录ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = WindowState.Normal;
            this.listView1.ItemsSource = replynews;
        }

        private void 显示最新ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ShowNotification(getData.GetTop1News(), shelp.AppID);
        }

        private void 更新订阅ToolStripMenuItem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            getData.UpdateNews();
        }
        #endregion

        #region 注册表操作相关
        /// <summary>
        /// 注册表路径
        /// </summary>
        private string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// 注册表键名
        /// </summary>
        private string keyName = "NotificationForToasts";

        /// <summary>
        /// 设置是否随系统启动，true:跟随系统启动，false：反之
        /// </summary>
        /// <param name="isStart">设置是否随系统启动</param>
        private bool setStartBySystem(bool isStart)
        {
            bool result = false;
            try
            {
                string path = System.Windows.Forms.Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rkmain = rk.CreateSubKey(keyPath);

                if (isStart)
                {
                    rkmain.SetValue(keyName, path);
                    result = true;
                }
                else
                {
                    rkmain.DeleteValue(keyName, false);
                    result = false;
                }
                rkmain.Close();
                rk.Close();
            }
            catch (UnauthorizedAccessException aes)
            {
                _ep.ExceptionReceive(304, "系统权限不足", aes, keyPath);
            }
            return result;
        }

        /// <summary>
        /// 获取当前注册表写入状态的值
        /// </summary>
        /// <returns></returns>
        private string getStartBySystem()
        {
            string path = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rkopen = rk.OpenSubKey(keyPath);
            string getResult = rkopen.GetValue(keyName) as String;
            rk.Close();
            rkopen.Close();
            return getResult;
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _time.Interval = 1000;
            _time.Elapsed += T_Elapsed;
            _time.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            sendMsg();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                this.listView1.Items.Refresh();
                Console.WriteLine("listView1.Items.Refresh();");
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification(getData.GetTop1News(), shelp.AppID);
        }

        /// <summary>
        /// 在工作日的9-15时为正常时段，返回Ture, 则返回 False
        /// </summary>
        /// <returns></returns>
        private bool isMatchTime()
        {
            bool isExTime = DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 15;
            int dw = (int)(DateTime.Now.DayOfWeek);
            bool isWorkday = dw>=1 && dw<=5;
            return isExTime && isWorkday;
        }

        private List<NewsModel> replynews = new List<NewsModel>();
        private string objPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "newslist.json");
        private void sendMsg()
        {
            // 9到15点刷新时间为10秒，其它刷新时间为1分钟
            if (isMatchTime())
            {
                _time.Interval = 1000 * 10;
            }
            else
            {
                _time.Interval = 1000 * 60;
            }

            getData.UpdateNews();
            NewsModel newItem = getData.GetTop1News();

            // 1. 初始化时载入原始数据
            if (replynews.Count.Equals(0))
            {
                string objStrings = string.Empty;
                if (File.Exists(objPath))
                {
                    using (StreamReader sr = new StreamReader(objPath))
                    {
                        objStrings = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                    }
                    replynews = JsonConvert.DeserializeObject<List<NewsModel>>(objStrings);
                }
            }

            // 2. 获取最新信息，如果不存在缓存中就显示
            //    如果缓存数据大于等于10时个，就将最早的信息删除
            if (newItem != null && replynews.Count(n => n.id == newItem.id)==0)
            {
                replynews.Add(newItem);
                if (replynews.Count >= 10)
                {
                    replynews.RemoveAt(0);
                }

                ShowNotification(newItem, shelp.AppID);
                string getObjJson = JsonConvert.SerializeObject(newItem);
                Console.WriteLine(getObjJson);
                // 数据发生变化时，更新缓存
                using (StreamWriter sw = new StreamWriter(new FileStream(objPath, FileMode.OpenOrCreate), Encoding.UTF8))
                {
                    sw.Write(JsonConvert.SerializeObject(replynews));
                    sw.Close();
                    sw.Dispose();
                }
                _ep.WriteLog("获取最新新闻", getObjJson);
            }
            else
            {
                string relog = "Some as NewsModel" + DateTime.Now.ToString("HH:dd:ss");
                Console.WriteLine(relog);
                _ep.WriteLog("该消息已提醒过", relog);
            }
        }

        private void ShowNotification(NewsModel item, string appid)
        {
            if (item == null) return;

            var toastContent1 = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = item.indexTitle
                            },
                            new AdaptiveText()
                            {
                                Text = item.newcontent
                            },
                            new AdaptiveImage()
                            {
                                //NASDAQ: http://d1.biz.itc.cn/usimg/mz/IXIC/usindex_6.png?timeStamp=
                                //中证: http://d1.biz.itc.cn/q/zs/001/000001/index_6.png?timeStamp=
                                Source = "http://d1.biz.itc.cn/q/zs/001/000001/index_6.png?timeStamp=" + new Random().Next(111111111, 999999999).ToString()
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "https://picsum.photos/id/"+ new Random().Next(1, 1084).ToString() + "/64/64",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Launch = string.Format("https://www.yicai.com/brief/{0}.html", item.id) //JsonConvert.SerializeObject(item)
            };
            var toastContent2 = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = item.indexTitle
                            },
                            new AdaptiveText()
                            {
                                Text = item.newcontent
                            }
                            //,
                            //new AdaptiveImage()
                            //{
                            //    //NASDAQ: http://d1.biz.itc.cn/usimg/mz/IXIC/usindex_6.png?timeStamp=
                            //    //中证: http://d1.biz.itc.cn/q/zs/001/000001/index_6.png?timeStamp=
                            //    Source = "http://d1.biz.itc.cn/q/zs/001/000001/index_6.png?timeStamp=" + new Random().Next(111111111, 999999999).ToString()
                            //}
                        },
                        //Attribution = new ToastGenericAttributionText()
                        //{
                        //    Text = "@Sam Shum"
                        //},
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = "https://picsum.photos/360/180?random=2&" + new Random().Next(111111111, 999999999).ToString()
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "https://findicons.com/files/icons/2833/flatastic_part_1/128/chart.png", // "https://picsum.photos/id/" + new Random().Next(1, 1084).ToString() + "/64/64",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Launch = string.Format("https://www.yicai.com/brief/{0}.html", item.id) //JsonConvert.SerializeObject(item)
            };
            var toastContent3 = new ToastContent();

            if (isMatchTime())
            {
                toastContent3 = toastContent1;
            }
            else
            {
                toastContent3 = toastContent2;
            }

            // Get a toast XML template
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(toastContent3.GetContent());

            var tn = new ToastNotification(doc)
            {
                Tag = "tag",
                Group = "group"
            };
            tn.Activated += ToastActivated;

            ToastNotificationManager.CreateToastNotifier(appid).Show(tn);
        }
        private void ToastActivated(ToastNotification sender, object e)
        {
            string getResult = (e as ToastActivatedEventArgs).Arguments;
            if (!string.IsNullOrWhiteSpace(getResult))
            {
                //直接在ToastContent中定义Launch属性为网址（X64），即可完成指定页面的打开，这里的Process.Start暂时取消。
                //NewsModel item = JsonConvert.DeserializeObject<NewsModel>(getResult);
                //Process.Start(string.Format("https://www.yicai.com/brief/{0}.html", getResult));
            }
            Console.Write(getResult);
            Dispatcher.Invoke(() =>
            {
                //Activate();
                Console.WriteLine(" & The user activated the toast.");
            });
        }

        private void ListView1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView1.SelectedIndex >= 0)
            {
                string getUrl = string.Format("https://www.yicai.com/brief/{0}.html", (listView1.ItemsSource as List<NewsModel>)[listView1.SelectedIndex].id);
                Process.Start(getUrl); 
            }
        }
    }
}
