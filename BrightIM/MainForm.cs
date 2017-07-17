using CefSharp;
using System; 
using System.Drawing; 
using System.Runtime.InteropServices; 
using System.Windows.Forms;
using System.Diagnostics;

namespace BrightIM
{
    public partial class MainForm : Form
    {
        Form MyFrmlogin;
        public MainForm(string userName, string password, string language, Form frmlogin)
        {
            InitializeComponent();
            SetForm();
            IsNewMessage = true;
            UserName = userName;
            Password = password;

            #region chrome参数设置  
            chromiumWebBrowser.Dock = DockStyle.Fill;
            chromiumWebBrowser.BackgroundImage = global::BrightIM.Properties.Resources.bg;
            chromiumWebBrowser.BackgroundImageLayout = ImageLayout.Tile;
            chromiumWebBrowser.MenuHandler = new MenuHandler();
            //下载事件
            DownloadHandler downloadHandler = new DownloadHandler();
            downloadHandler.OnBeforeDownloadFired += OnBeforeDownloadFired;
            downloadHandler.OnDownloadUpdatedFired += OnDownloadUpdatedFired;
            chromiumWebBrowser.DownloadHandler = downloadHandler;

            chromiumWebBrowser.RequestHandler = new RequestHandler();

            //注册脚本事件，用于前端调用后台方法
            chromiumWebBrowser.RegisterJsObject("cefsharpCallback", this);
            #endregion

            MyFrmlogin = frmlogin;

            this.Icon = Properties.Resources.icon;
            notifyIcon.Icon = Properties.Resources.icon;
        }

        #region 属性
        CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser = new CefSharp.WinForms.ChromiumWebBrowser("");
        private bool IsNewMessage = false;
        private Icon icon = Properties.Resources.icon;
        private Icon noico = Properties.Resources.noico; //两个图标 切换显示 以达到消息闪动的效果
        private string UserName = string.Empty;                         //登录用户
        private string Password = string.Empty;                         //
        private string DefaultLanguage = string.Empty;                  //当前语言
        #endregion

        #region 事件

        private void MainForm_Load(object sender, EventArgs e)
        {
            //string webChatUrl = "http://mtest.bdo.com.cn/eoop-webchat/auth?username=zhu.junfang&password=123456Aa&logintype=winform";//  EoopService.GetEoopWebChatLoginURL(UserName, Password, "winform");
            //"http://172.18.50.120/eoop/a.html";

            string webChatUrl =  EoopService.GetEoopWebChatLoginURL(UserName, Password, "winform");
            chromiumWebBrowser.Load(webChatUrl);
            this.panelMain.Controls.Add(chromiumWebBrowser);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                notifyMenu.Show();
            }

            if (e.Button == MouseButtons.Left)
            {
            }
        }

        private void notifyMenu_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(ResourceCulture.GetString("GeneralMsg_ExitApp"), ResourceCulture.GetString("GeneralTitle_ExitApp"), MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)//如果点击“确定”按钮
                {
                    //CloseOpenBrowseForms();
                    //this.notifyIcon.Visible = false;
                    //ApplicationExit();
                    //释放图标资源
                    this.notifyIcon.Dispose();
                    KillProcess("CefSharp.BrowserSubprocess");
                    KillProcess("BrightIM");
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }

         
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            //this.Show();
            //this.Activate();

            //this.WindowState = FormWindowState.Normal;
            //this.backgroundWorker1.CancelAsync();  

            //notifyIcon.Visible = false; 
            WindowState = FormWindowState.Normal;
            this.Show();
            this.Activate();//激活窗体
            //this.Focus();
        }


        //private static int inTimer = 0; //防止线程重入
        int i = 0;//先设置一个全局变量 i ,用来控制图片索引,然后创建定时事件,双击定时控件就可以编辑 
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (Interlocked.Exchange(ref inTimer, 1) == 0)
            //{

            //如果最小化显示在任务栏，则右下角不闪烁
            //if (this.WindowState != FormWindowState.Normal && 
            //    this.WindowState != FormWindowState.Minimized && 
            //    IsNewMessage)
             
            //右下角闪烁
            if(!this.Visible&& IsNewMessage)
            { 
                //如果i=0则让任务栏图标变为透明的图标并且退出    
                if (i < 1)
                {
                    this.notifyIcon.Icon = icon;
                    i++;
                    return;
                }
                //如果i!=0,就让任务栏图标变为ico1,并将i置为0;    
                else
                    this.notifyIcon.Icon = noico;
                i = 0;
            }

            //if(IsNewMessage)
            //{
            //    //任务栏闪烁
            //    flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMERNOFG);
            //}
              
            //    Interlocked.Exchange(ref inTimer, 0);
            //} 

            #region 注销
            if (IsLogout)
            {
                //IsLogout = false;
                #region 删除本地记录的用户信息  
                //Properties.Settings.Default.UserName = "";
                Properties.Settings.Default.Password = "";//注销清除密码
                Properties.Settings.Default.Save();
                #endregion
                this.Close();
                //关闭CEF窗体
                //int browseFormCount = 1;
                //while (browseFormCount > 0)
                //{
                //    foreach (Form openForm in Application.OpenForms)
                //    {
                //        if (openForm.Name == "MainForm")
                //        {
                //            openForm.Close();
                //            browseFormCount -= 1;
                //            break;
                //        }
                //    }
                //}

                //int browseFormCount2 = 1;
                //while (browseFormCount2 > 0)
                //{
                //    foreach (Form openForm in Application.OpenForms)
                //    {
                //        if (openForm.Name == "Login")
                //        {
                //            openForm.Show();
                //            //Application.Run(openForm);
                //            browseFormCount2 -= 1;
                //            break;
                //        }
                //    }
                //}
            }
            #endregion
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsLogout)
            {
                //不是注销，处理右键关闭，进程仍然存在问题，注销的情况下，需要切换到登录页面，不退出，
                //this.FormClosing -= new FormClosingEventHandler(this.MainForm_FormClosing);
                //为保证Application.Exit();时不再弹出提示，所以将FormClosing事件取消
                //ApplicationExit();
                 
                //释放图标资源
                this.notifyIcon.Dispose(); 
                KillProcess("CefSharp.BrowserSubprocess");
                KillProcess("BrightIM");
            }
            else
            {
                MyFrmlogin.Activate();
                MyFrmlogin.Show();
            } 
        }


        #region 窗体拖动事件  
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //private Point mPoint = new Point(); 
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            //mPoint.X = e.X;
            //mPoint.Y = e.Y; 
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        //private void Form_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        Point myPosittion = MousePosition;
        //        myPosittion.Offset(-mPoint.X, -mPoint.Y);
        //        Location = myPosittion;
        //    }
        //} 

        #endregion


        private void btnLogout_MouseEnter(object sender, EventArgs e)
        {
            this.btnLogout.BackColor = ColorTranslator.FromHtml("#FF2525");
        }

        private void btnLogout_MouseLeave(object sender, EventArgs e)
        {
            this.btnLogout.BackColor = ColorTranslator.FromHtml("#E0353B");
        }


        #region 缩小按钮相关事件 
        //bool IsShowInTaskbar = true;
        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            this.btnMinimize.BackColor = ColorTranslator.FromHtml("#FF2525");
        }
        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            this.btnMinimize.BackColor = ColorTranslator.FromHtml("#E0353B");
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            //this.ShowInTaskbar = true; 
            this.WindowState = FormWindowState.Minimized;
            //IsShowInTaskbar = true;
        }
        #endregion


        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            //刷新功能
            chromiumWebBrowser.GetBrowser().Reload();
        }
        private void btnRefresh_MouseEnter(object sender, EventArgs e)
        {
            this.btnRefresh.BackColor = ColorTranslator.FromHtml("#FF2525");
        }
        private void btnRefresh_MouseLeave(object sender, EventArgs e)
        {
            this.btnRefresh.BackColor = ColorTranslator.FromHtml("#E0353B");
        }

        #region 关闭按钮相关事件    
        private void btnCloseApp_MouseEnter(object sender, EventArgs e)
        {
            this.btnCloseApp.BackColor = ColorTranslator.FromHtml("#FF2525");
        }
        private void btnCloseApp_MouseLeave(object sender, EventArgs e)
        {
            this.btnCloseApp.BackColor = ColorTranslator.FromHtml("#E0353B");
        }
        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            //try
            //{
            //     DialogResult result = MessageBox.Show(ResourceCulture.GetString("GeneralMsg_ExitApp"), ResourceCulture.GetString("GeneralTitle_ExitApp"), MessageBoxButtons.OKCancel);
            //    if (result == DialogResult.OK)//如果点击“确定”按钮
            //    {
            //        CloseOpenBrowseForms();
            //        ApplicationExit();
            //    }
            //    else//如果点击“取消”按钮
            //    {
            //        //DoNothing
            //    }
            //}
            //catch //(Exception ex)
            //{
            //    MessageBox.Show(ResourceCulture.GetString("GeneralMsg_AppException"), ResourceCulture.GetString("LoginMsg_Title"));
            //    ApplicationExit();
            //}

            //切换到右下角

            //this.notifyIcon.Icon = this.Icon;
            //this.ShowInTaskbar = false;
           
            //IsShowInTaskbar = false;
             
            //this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
        #endregion


        /// <summary>
        /// 是否是注销
        /// </summary>
        bool IsLogout = false;

        /// <summary>
        /// 是否是Web注销
        /// </summary>
        //bool IsWebLogout = false;

        /// <summary>
        /// 注销用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                //DialogResult result = MessageBox.Show("请问您确定要注销用户吗？", "退出系统", MessageBoxButtons.OKCancel);
                DialogResult result = MessageBox.Show(string.Format(ResourceCulture.GetString("Msg_Logout"), UserName), ResourceCulture.GetString("MsgTitle_Logout"), MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)//如果点击“确定”按钮
                {
                    IsLogout = true;

                    #region 删除本地记录的用户信息  
                    //Properties.Settings.Default.UserName = "";
                    Properties.Settings.Default.Password = "";//注销清除密码
                    Properties.Settings.Default.Save();
                    #endregion

                    //关闭CEF窗体
                    int browseFormCount = 1;
                    while (browseFormCount > 0)
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm.Name == "MainForm")
                            {
                                openForm.Close();
                                browseFormCount -= 1;
                                break;
                            }
                        }
                    }
                    //注销程序 
                    Cef.Shutdown();
                    Program.Run.Close(); 
                    Application.Restart();

                    //MyFrmlogin.Activate();
                    //MyFrmlogin.Show();
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                //this.notifyIcon.Visible = true;    //显示托盘图标
                //if (!IsShowInTaskbar)
                //    this.Hide();    //隐藏窗口
                //if (IsNewMessage)
                //    flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMERNOFG);
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                //IsShowInTaskbar = true;
                IsNewMessage = false;//正常状态下不闪烁
                this.notifyIcon.Icon = icon;//图标还原
                //取消任务栏闪动
                //flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_STOP);
            }

            //switch (this.WindowState)
            //{
            //    case FormWindowState.Normal:
            //        IsNewMessage = false;
            //        this.notifyIcon.Icon = icon;
            //        break;
            //    case FormWindowState.Minimized:
            //        if (IsNewMessage)
            //            flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMERNOFG);
            //        break;
            //}
        }


        #endregion

        #region 方法

        /// <summary>
        /// 网页调用注销
        /// </summary>
        public void webLogout()
        {
            //这一句必不可少
            CheckForIllegalCrossThreadCalls = false;
            //btnLogout_Click(null, null);
            //IsWebLogout = true;
            IsLogout = true;
        }

        /// <summary>
        /// winform中设置FormBorderStyle为None后点击任务栏自动最小化实现
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        private void SetForm()
        {
            int WS_SYSMENU = 0x00080000; // 系统菜单
            int WS_MINIMIZEBOX = 0x20000; // 最大最小化按钮
            int windowLong = (GetWindowLong(new HandleRef(this, this.Handle), -16));
            SetWindowLong(new HandleRef(this, this.Handle), -16, windowLong | WS_SYSMENU | WS_MINIMIZEBOX);

        }

        #region 实现 任务栏 程序闪烁 
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        //public const UInt32 FLASHW_STOP = 0;
        //public const UInt32 FLASHW_CAPTION = 1;
        //public const UInt32 FLASHW_TRAY = 2;
        //public const UInt32 FLASHW_ALL = 3;
        //public const UInt32 FLASHW_TIMER = 4;
        //public const UInt32 FLASHW_TIMERNOFG = 12;

        //闪动并停留需要使用这个函数：
        [DllImport("user32.dll")]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [DllImport("user32.dll")]
        static extern bool FlashWindow(IntPtr handle, bool invert);

        public enum FLASH_TYPE : uint
        {
            FLASHW_STOP = 0,    //停止闪烁
            FALSHW_CAPTION = 1,  //只闪烁标题
            FLASHW_TRAY = 2,   //只闪烁任务栏
            FLASHW_ALL = 3,     //标题和任务栏同时闪烁
            FLASHW_PARAM1 = 4,
            FLASHW_PARAM2 = 12,
            FLASHW_TIMER = FLASHW_TRAY | FLASHW_PARAM1,   //无条件闪烁任务栏直到发送停止标志，停止后高亮
            FLASHW_TIMERNOFG = FLASHW_TRAY | FLASHW_PARAM2  //未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮
        }

        public static bool flashTaskBar(IntPtr hWnd, FLASH_TYPE type)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            fInfo.dwFlags = (uint)type;//闪烁的类型
            //fInfo.uCount = UInt32.MaxValue;//闪烁窗口的次数
            fInfo.uCount = 5;//UInt32.MaxValue;//闪烁窗口的次数
            fInfo.dwTimeout = 0; //窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            return FlashWindowEx(ref fInfo);
        }

        //未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮
        //flashTaskBar(this.Handle, falshType.FLASHW_TIMERNOFG);
        //下面的调用：停止闪烁，停止后如果未激活窗口，窗口高亮
        //flashTaskBar(this.Handle, falshType.FLASHW_STOP);

        /// <summary>
        /// 任务栏闪动
        /// </summary>
        /// <param name="msg"></param>
        public void newMessageNotify()
        {
            //获取未读信息，实现窗体闪动 ,这一句必不可少
            CheckForIllegalCrossThreadCalls = false;
            IsNewMessage = true;
            //this.backgroundWorker1.RunWorkerAsync();  
            //flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMER);
            //FlashIcon(); 
            flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMERNOFG);
        }

        /// <summary>
        /// 取消任务栏闪动
        /// </summary>
        /// <param name="msg"></param>
        public void stopNotify()
        {
            //获取未读信息，实现窗体闪动 ,这一句必不可少
            CheckForIllegalCrossThreadCalls = false;
            IsNewMessage = false;
            flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_STOP);
            //this.backgroundWorker1.RunWorkerAsync();  
            //flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_TIMER);
            //FlashIcon(); 
        }

        #endregion

        private void KillProcess(string processName)
        {
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            //得到所有打开的进程   
            try
            {
                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName(processName))
                {
                    thisproc.Kill();
                } 
                //隐藏右下角icon
                //this.notifyIcon.Visible = false;
            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.Message);
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private void OnBeforeDownloadFired(object sender, DownloadItem e)
        {
            //DownloadHandler downloadHandler = (DownloadHandler)sender;
            IBrowser browser = (IBrowser)sender;
            if (browser.IsPopup)
            {
                //解决下载弹出框问题 neo 2017年1月24日15:51:39
                IntPtr handle = browser.GetHost().GetWindowHandle();//得到窗口的句柄
                ShowWindow(handle, 0);//不显示下载对话框页面
            }
            //this.UpdateDownloadAction("OnBeforeDownload", e);
        }

        private void OnDownloadUpdatedFired(object sender, DownloadItem e)
        {
            //DownloadHandler downloadHandler = (DownloadHandler)sender;
            IBrowser browser = (IBrowser)sender;
            //this.UpdateDownloadAction("OnDownloadUpdated", e);

            //
            //TODO: 显示下载进度...
            //

            if (e.IsCancelled || e.IsComplete)
            {
                if (e.IsComplete)
                {
                    string[] fileNames = e.FullPath.Split('\\');
                    string fileName = fileNames.Length > 0 ? fileNames[fileNames.Length - 1] : "";
                    string message = string.Format(ResourceCulture.GetString("Cefsharp_DownloadComplete"), fileName);
                    MessageBox.Show(message, ResourceCulture.GetString("GeneralTitle_Prompt"));
                }
                if (browser.IsPopup)
                {
                    browser.CloseBrowser(true);
                }
            }
        }


        #endregion

        private void MainForm_Activated(object sender, EventArgs e)
        {
            //激活的时候，需要把新消息设置为false
            IsNewMessage = false;//正常状态下不闪烁
            this.notifyIcon.Icon = icon;//图标还原

            //取消任务栏闪动
            //flashTaskBar(this.Handle, FLASH_TYPE.FLASHW_STOP);
        }

        public void openVideo(string filePath)
        {
            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "wmplayer.exe";
                myProcess.StartInfo.Arguments = filePath;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择合适的播放器！");
            }
        }
    }
}
