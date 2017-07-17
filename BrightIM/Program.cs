using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace BrightIM
{
    static class Program
    {
        public static System.Threading.Mutex Run;

        /// <summary>
        /// 为指定用户组，授权目录指定完全访问权限
        /// </summary>
        /// <param name="user">用户组，如Users</param>
        /// <param name="folder">实际的目录</param>
        /// <returns></returns>
        //private static bool SetAccess(string user, string folder)
        //{
        //    //定义为完全控制的权限
        //    const FileSystemRights Rights = FileSystemRights.FullControl;

        //    //添加访问规则到实际目录
        //    var AccessRule = new FileSystemAccessRule(user, Rights,
        //        InheritanceFlags.None,
        //        PropagationFlags.NoPropagateInherit,
        //        AccessControlType.Allow);

        //    var Info = new DirectoryInfo(folder);
        //    var Security = Info.GetAccessControl(AccessControlSections.Access);

        //    bool Result;
        //    Security.ModifyAccessRule(AccessControlModification.Set, AccessRule, out Result);
        //    if (!Result) return false;

        //    //总是允许再目录上进行对象继承
        //    const InheritanceFlags iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

        //    //为继承关系添加访问规则
        //    AccessRule = new FileSystemAccessRule(user, Rights,
        //        iFlags,
        //        PropagationFlags.InheritOnly,
        //        AccessControlType.Allow);

        //    Security.ModifyAccessRule(AccessControlModification.Add, AccessRule, out Result);
        //    if (!Result) return false;

        //    Info.SetAccessControl(Security);

        //    return true;
        //}


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        { 
            //为用户组指定对应目录的完全访问权限
            //SetAccess("Users", Application.StartupPath); 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region 保证同时只有一个客户端在运行  
            bool noRun = false;
            Run = new System.Threading.Mutex(true, "BrightIM", out noRun);
            if (!noRun)
            {
                //互斥锁
                Process instance = RunningInstance();
                string productName = ResourceCulture.GetString("AppName");  //Application.ProductName
                string exMsg = string.Format(ResourceCulture.GetString("GeneralMsg_AppAlreadyRun"), productName);
                MessageBox.Show(exMsg, productName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                HandleRunningInstance(instance);
                return;
            }

            #endregion

            #region 检测网络连接是否正常
            if (!CommonService.IsConnectInternet())
            {
                string productName = ResourceCulture.GetString("AppName");  //Application.ProductName
                string exMsg = string.Format(ResourceCulture.GetString("GeneralMsg_NetworkOffline"), Application.ProductName);
                MessageBox.Show(exMsg, productName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
            #endregion

            #region 程序升级 
            string localVersion = string.Empty, serverVersion = string.Empty;
            var shouldUpdateApp = ShouldUpdateApp(out localVersion, out serverVersion);
            if (shouldUpdateApp)
            {
                Process.Start(Application.StartupPath + @"\AutoUpdate.exe");
                //Application.Exit();
                Environment.Exit(0);
                return;
            }
            #endregion

            string strIsFirstRun = "false";
            bool isFirstRun = false;
            strIsFirstRun = ConfigurationManager.AppSettings["IsFirstRun"] ; 
            if (string.IsNullOrEmpty(strIsFirstRun) || strIsFirstRun.ToLower() != "true")
            {
                isFirstRun = false;
            }
            else
            {
                isFirstRun = true;
            } 
            if(isFirstRun)
            {
                //重置存储的用户和密码
                Properties.Settings.Default.Reset();
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["IsFirstRun"].Value = "false";
                cfa.Save();
            } 

            Application.Run(new Login());
            // 自动生成的代码是这样的   
            // 表示 实例化一个新的 Form1 并显示之 此时程序进入消息循环  
            // 一旦 Form1 被关闭则程序也随之关闭了  
            // 为了让程序在 Form1 关闭后可以继续运行 需要修改下  
            //new Login().Show();
            //Application.Run();

            //Application.Run (new MainForm("linling", "123456", "zh-CN"));
        }

        private static bool ShouldUpdateApp(out string localVersion, out string serverVersion)
        {
            bool shouldUpdate = false;
            try
            {
                Run.ReleaseMutex();
                //AutoUpdate.XmlFiles localXmlFiles = new AutoUpdate.XmlFiles(Path.GetDirectoryName(Application.ExecutablePath) + @"\UpdateList.xml");
                XmlFiles localXmlFiles = new XmlFiles(Application.StartupPath + @"\UpdateList.xml");
                localVersion = localXmlFiles.GetNodeValue("//Version");
                XmlFiles serverXmlFiles = new XmlFiles(localXmlFiles.GetNodeValue("//Url") + @"UpdateList.xml");
                serverVersion = serverXmlFiles.GetNodeValue("//Version");
                shouldUpdate = Convert.ToInt32(serverVersion.Replace(".", "")) > Convert.ToInt32(localVersion.Replace(".", ""));
            }
            catch
            {
                localVersion = string.Empty;
                serverVersion = string.Empty;
            }
            return shouldUpdate;
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        private static void HandleRunningInstance(Process instance)
        {
            // 确保窗口没有被最小化或最大化   
            ShowWindowAsync(instance.MainWindowHandle, 4);
            // 设置真实例程为foreground  window    
            SetForegroundWindow(instance.MainWindowHandle);// 放到最前端   
        }
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    // 确保例程从EXE文件运行   
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
    }
}
