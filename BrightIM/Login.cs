using CefSharp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIM
{



    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            #region 读取配置文件: 绑定记住的用户名和密码 
            string username = Properties.Settings.Default.UserName;
            string password = Properties.Settings.Default.Password;
            this.txtUserName.Text = !string.IsNullOrWhiteSpace(username) ? username : string.Empty;
            this.txtPassword.Text = !string.IsNullOrEmpty(password) ? password : string.Empty;
            this.Icon = Properties.Resources.icon;
            #endregion 
        }

        #region 属性
        private string DefaultLanguage = string.Empty;
        #endregion

        #region 事件
        /// <summary>
        /// 登录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            #region 检测网络连接是否正常
            if (!CommonService.IsConnectInternet())
            {
                string productName = ResourceCulture.GetString("AppName");  //Application.ProductName
                string exMsg = string.Format(ResourceCulture.GetString("GeneralMsg_NetworkOffline"), Application.ProductName);
                MessageBox.Show(exMsg, productName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            #endregion

            //string userName = this.cbxUserName.Text.Trim();
            string userName = this.txtUserName.Text.Trim();
            string password = this.txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show(ResourceCulture.GetString("GeneralMsg_EmptyUser"), ResourceCulture.GetString("GeneralTitle_Prompt"));
                this.Show();
                return;
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(ResourceCulture.GetString("GeneralMsg_EmptyPwd"), ResourceCulture.GetString("GeneralTitle_Prompt"));
                this.Show();
                return;
            }

            bool loginResult = false;           //登录结果
            string loginUserId = string.Empty;  //登录用户对应 OA 系统用户 id
            string loginMessage = string.Empty; //登录结果提示信息

            try
            {
                #region 登录密码验证
                loginResult =  EoopService.LoginVerify(userName, password, out loginUserId, out loginMessage); 
                if (!loginResult)
                {
                    MessageBox.Show(loginMessage, ResourceCulture.GetString("GeneralTitle_Prompt"));
                    //this.txtPassword.Text = "";//清空密码，防止自动登录BUG 
                    this.Show();
                    return;
                }

                //if (loginResult && !string.IsNullOrEmpty(loginMessage) && loginMessage.Trim() == "1")//默认密码登录，强制更新密码
                //{
                //    this.Hide();
                //    ChangePassword changePwdForm = new ChangePassword(DefaultLanguage, userName, password, true);
                //    changePwdForm.ShowDialog();
                //    return;
                //} 
                #endregion

                #region 记住用户 & 记住密码  
                Properties.Settings.Default.UserName = userName;
                Properties.Settings.Default.Password = password; 
                Properties.Settings.Default.Save();
                #endregion

                #region 登录，进入程序主界面 MainForm 
                //if (!Cef.IsInitialized)
                //{
                //    Cef.RunMessageLoop();
                //    Cef.Shutdown();
                //}

                this.Hide();

                //if (Application.OpenForms.Count > 2)
                //{
                //    #region 如果是被踢出，则之前窗体仍然存在，需要先关闭 
                //    int browseFormCount = 1;
                //    while (browseFormCount > 0)
                //    {
                //        foreach (Form openForm in Application.OpenForms)
                //        {
                //            if (openForm.Name == "MainForm")
                //            {
                //                openForm.Close();
                //                browseFormCount -= 1;
                //                break;
                //            }
                //        }
                //    }
                //    #endregion
                //}
                 
                MainForm mainForm = new MainForm(userName, password, DefaultLanguage,this);
                mainForm.Show(); 

                //初始化 CefSharp 浏览器内核组件
                //InitializeCefSharp(DefaultLanguage);
                //txtPassword.Text = string.Empty;  
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region 窗体可拖动

        private Point mPoint = new Point();

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }

        private void Login_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
            }
        }
        //private void Login_Load(object sender, System.EventArgs e)
        //{
        //    string username = Properties.Settings.Default.UserName;
        //    string password = Properties.Settings.Default.Password;
        //    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        //    {
        //        //this.Hide();
        //        this.Visible = false;
        //        this.btnLogin_Click(sender, e);
        //    }
        //}
        #endregion

        private void Login_Shown(object sender, EventArgs e)
        {
            //this.Activate();
            //this.Focus(); 
            string username = Properties.Settings.Default.UserName;
            string password = Properties.Settings.Default.Password;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                this.Hide();
                //this.Visible = false;
                this.btnLogin_Click(sender, e);
            }
        } 

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            //在输入完成密码后按下enter键进入系统
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键
            {
                this.btnLogin_Click(sender, e);//触发button事件
            }
        }


        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗体，退出应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                //DialogResult result = MessageBox.Show("请问您确定退出“立信安全客户端”程序吗？", "退出系统", MessageBoxButtons.OKCancel);
                DialogResult result = MessageBox.Show(ResourceCulture.GetString("GeneralMsg_ExitApp"), ResourceCulture.GetString("GeneralTitle_ExitApp"), MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)//如果点击“确定”按钮
                {
                    KillProcess("CefSharp.BrowserSubprocess");
                    KillProcess("BrightIM");
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message );
            }
        }

        #endregion

        #region 方法


        /// <summary>
        /// 初始化 CefSharp
        /// </summary>
        private void InitializeCefSharp(string language)
        { 
            if (Cef.IsInitialized)
                return;
            var settings = new CefSettings();
            //
            //Load the pepper flash player that comes with Google Chrome - may be possible to load these values from the registry and query the dll for it's version info (Step 2 not strictly required it seems)
            //
            //Load a specific pepper flash version (Step 1 of 2)
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", Path.GetDirectoryName(Application.ExecutablePath) + @"\Plugins\PepperFlash\pepflashplayer.dll");
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", Path.GetDirectoryName(Application.ExecutablePath) + @"\pepflashplayer.dll");
            //Load a specific pepper flash version (Step 2 of 2)
            //settings.CefCommandLineArgs.Add("ppapi-flash-version", "20.0.0.228");

            //
            //Other Settings
            //
            //settings.LogSeverity = LogSeverity.Verbose;
            //settings.LogSeverity = LogSeverity.Error;
            //settings.PackLoadingDisabled = true;
            //settings.UserAgent = ""; 
            //settings.IgnoreCertificateErrors = true;
            //settings.CefCommandLineArgs.Add("debug-plugin-loading", "1");
            //settings.cefcommandlineargs.add("allow-outdated-plugins", "1");
            //settings.CefCommandLineArgs.Add("always-authorize-plugins", "1");
            //设置语言
            //settings.Locale = !string.IsNullOrWhiteSpace(language) ? language : "zh-CN";

            //开启chrome的媒体流
            //settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream"); 
            //settings.CefCommandLineArgs.Add("enable-exclusive-audio", "1");
            //settings.CefCommandLineArgs.Add("alsa-output-device", "1");
            Cef.Initialize(settings);
        }

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
            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.Message);
            }
        }
        #endregion

    }
}
