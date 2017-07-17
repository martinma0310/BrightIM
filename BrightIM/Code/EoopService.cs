using System;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;

namespace BrightIM
{
    public static class EoopService
    {
        /// <summary>
        /// 获取WebChat聊天系统URL
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetEoopWebChatLoginURL(string userName, string password,string logintype)
        {
            string returnUrl = string.Format("{0}?username={1}&password={2}&logintype={3}", GetEoopAppSettings("EOOP_WEB_CHAT_AUTH"), userName, password, logintype);
            returnUrl = HttpUtility.UrlDecode(returnUrl);
            return returnUrl;
        }

        

        /// <summary>
        /// 子系统统一跳转登录URL
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public static string GetSubsystemLoginURL(string userName, string password, string systemKey, string systemName, string systemUrl)
        //{
        //    string returnUrl = string.Empty;
        //    EncryptBiz encryptBiz = new EncryptBiz();
        //    string enctPwd = encryptBiz.EncryptDES(password, null, true);
        //    string md5Pwd = Md5Hash(password + systemKey);
        //    string token = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(userName + DateTime.Now.ToString("yyyyMMdd") + systemKey, "MD5").ToLower();
        //    //string token = Md5Hash(userName + DateTime.Now.ToString("yyyyMMdd") + systemKey); 
        //    string eoopServiceUrl = GetEoopAppSettings("BDO_USER_SUBSYSTEM_LOGIN_VERIFY");
        //    returnUrl = string.Format("{0}?username={1}&password={2}&systemid={3}&name={4}&url={5}&mid={6}&token={7}", eoopServiceUrl, userName, enctPwd, systemKey, systemName, systemUrl, md5Pwd, token);
        //    return returnUrl;
        //}

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool LoginVerify(string userName, string password, out string userId, out string message)
        {
            try
            {
                //string eoopLoginVerify = GetEoopAppSettings("EOOP_LOGIN_VERIFY");
                ////eoopLoginVerify = string.Format("{0}?username={1}&passwd={2}&device=&deviceType=3", eoopLoginVerify, userName, password);
                //string result = GETEoopHttpRequest(eoopLoginVerify);
                //LoginResult loginResult = (LoginResult)JsonConvert.DeserializeObject(result, typeof(LoginResult));
                //userId = loginResult.ok ? loginResult.objValue : null;
                //message = loginResult.ok ? loginResult.value : loginResult.objValue;
                //return loginResult.ok;  
                string eoopLoginVerify = GetEoopAppSettings("EOOP_LOGIN_VERIFY");
                StringBuilder sb = new StringBuilder();
                sb.Append("{"); 
                string body = string.Format("\"username\":\"{0}\",\"password\":\"{1}\",\"device\":null,\"deviceType\":\"3\"", userName, password);
                sb.Append(body);
                sb.Append("}"); 
                //string requesstBody = JsonConvert.SerializeObject(sb.ToString());
                string result = POSTEoopHttpRequest(eoopLoginVerify, sb.ToString());
                //BaseEoopResponse resultResponse = (BaseEoopResponse)JsonConvert.DeserializeObject(result, typeof(BaseEoopResponse)); 
                BaseEoopResponse resultResponse = (BaseEoopResponse)JsonConvert.DeserializeObject(result, typeof(BaseEoopResponse)); 
                userId = resultResponse.ok ? resultResponse.objValue.id : "";
                message = resultResponse.value;
                return resultResponse.ok; 
            }
            catch //(Exception ex)
            {
                //Exception apiEx = new Exception("用户登录失败：" + ex.Message, ex);
                //throw apiEx;
                userId = null;
                message = "用户登录失败：请检查服务器配置！";//+ ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取用户相关信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        //public static UserProfile GetUserProfile(string userId)
        //{
        //    try
        //    {
        //        string eoopUserProfileUrl = string.Format("{0}?userId={1}", GetEoopAppSettings("EOOP_USER_PROFILE"), userId);
        //        string result = GETEoopHttpRequest(eoopUserProfileUrl);
        //        UserProfileResult userProfileResult = (UserProfileResult)JsonConvert.DeserializeObject(result, typeof(UserProfileResult));

        //        return userProfileResult.ok ? userProfileResult.objValue : new UserProfile();
        //    }
        //    catch (Exception ex)
        //    {
        //        Exception apiEx = new Exception("获取用户详情信息失败：" + ex.Message, ex);
        //        throw apiEx;
        //    }
        //}

        //public static bool ResetPassword(string userName, string oldPassword, string newPassword, out string message)
        //{
        //    try
        //    {
        //        string eoopResetPwdUrl = GetEoopAppSettings("EOOP_RESET_PASSWORD");
        //        //string postData = "{\"username\":\"" + userName + "\",\"oldPassword\":\"" + oldPassword + "\",\"newPassword\":\"" + newPassword + "\"}";
        //        eoopResetPwdUrl = string.Format("{0}?loginId={1}&oldPassword={2}&newPassword={3}", eoopResetPwdUrl, userName, oldPassword, newPassword);
        //        string result = GETEoopHttpRequest(eoopResetPwdUrl);
        //        LoginResult resetPwdResult = (LoginResult)JsonConvert.DeserializeObject(result, typeof(LoginResult));
        //        message = resetPwdResult.objValue;
        //        return resetPwdResult.ok;
        //    }
        //    catch (Exception ex)
        //    {
        //        Exception apiEx = new Exception("用户密码重置失败：" + ex.Message, ex);
        //        throw apiEx;
        //    }
        //}
         
 
      
        ///// <summary>
        ///// 添加 BDO 客户端日志
        ///// </summary>
        ///// <param name="logs"></param>
        ///// <returns></returns>
        //public static bool AddBDOClientLogs(HrmResource loginUser, string userName, string slKeyCode, string itMark, string logType, string logData)
        //{
        //    try
        //    {
        //        ClientLogsV2 clientLogsV2 = new ClientLogsV2();
        //        clientLogsV2.company = !string.IsNullOrEmpty(loginUser.subcompanyname) ? loginUser.subcompanyname : "";
        //        clientLogsV2.department = !string.IsNullOrEmpty(loginUser.departmentname) ? loginUser.departmentname : "";
        //        //clientLogsV2.userName = !string.IsNullOrEmpty(userName) ? userName : "";
        //        //clientLogsV2.loginId = !string.IsNullOrEmpty(loginUser.userId) ? loginUser.userId : "";
        //        clientLogsV2.userName = !string.IsNullOrEmpty(loginUser.name) ? loginUser.name : "";
        //        clientLogsV2.loginId = !string.IsNullOrEmpty(loginUser.loginid) ? loginUser.loginid : "";
        //        clientLogsV2.userSlCode = !string.IsNullOrEmpty(slKeyCode) ? slKeyCode : "";
        //        clientLogsV2.itMark = !string.IsNullOrEmpty(itMark) ? itMark : "";

        //        HardwareHelper hardwareInfo = HardwareHelper.Instance();
        //        clientLogsV2.pcName = hardwareInfo.ComputerName;
        //        clientLogsV2.cpuId = hardwareInfo.CpuID;
        //        clientLogsV2.cpuName = hardwareInfo.CpuName;
        //        clientLogsV2.diskName = hardwareInfo.DiskID;
        //        clientLogsV2.mcAddress = hardwareInfo.MacAddress;
        //        clientLogsV2.ip = hardwareInfo.IpAddress;
        //        clientLogsV2.ram = Math.Ceiling(Convert.ToDecimal(hardwareInfo.TotalPhysicalMemory) / 1024 / 1024 / 1024).ToString() + "G";
        //        clientLogsV2.logDate = DateTime.Now.ToString("yyyy-MM-dd");
        //        clientLogsV2.logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        clientLogsV2.logType = logType;                 //TODO: ....
        //        clientLogsV2.logTypeValue = "1";                //TODO: ....
        //        clientLogsV2.logData = logData;                 //

        //        string bdoClientLogURL = GetEoopAppSettings("EOOP_BDO_CLIENT_LOG");
        //        string requesstBody = JsonConvert.SerializeObject(clientLogsV2);
        //        string result = POSTEoopHttpRequest(bdoClientLogURL, requesstBody);
        //        BaseEoopResponse resultResponse = (BaseEoopResponse)JsonConvert.DeserializeObject(result, typeof(BaseEoopResponse));
        //        return resultResponse.ok;

        //    }
        //    catch //(Exception ex)
        //    {
        //        //Exception clientEx = new Exception("添加客户端日志失败：" + ex.Message, ex);
        //        //throw clientEx;
        //        return false;
        //    }
        //}
         
        /// <summary>
        /// 获取 Eoop URL
        /// </summary>
        /// <param name="appSettingName"></param>
        /// <returns></returns>
        public static string GetEoopAppSettings(string appSettingName)
        {
            return GetAppSettings("EOOP_SERVER_PATH") + GetAppSettings(appSettingName);
        }

        /// <summary>
        /// 获取 AppSetting 配置信息
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static string GetAppSettings(string appSettingName)
        {
            appSettingName = appSettingName.ToUpper();
            return ConfigurationManager.AppSettings[appSettingName.ToUpper()].ToString();
        }

        /// <summary>
        /// Send GET HttpRequest
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        private static string GETEoopHttpRequest(string requestUrl)
        {
            requestUrl = HttpUtility.UrlDecode(requestUrl, Encoding.UTF8);
            System.Net.HttpWebRequest myRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUrl);
            myRequest.Headers.Set(System.Net.HttpRequestHeader.AcceptLanguage, Thread.CurrentThread.CurrentCulture.Name);
            myRequest.Method = "POST";
            myRequest.Timeout = 4000;
            myRequest.ContentType = "application/json;charset=UTF-8";
            myRequest.Accept = "application/json;charset=UTF-8";
              
            System.Net.HttpWebResponse myResponse = (System.Net.HttpWebResponse)myRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            int status = (int)myResponse.StatusCode;
            reader.Close();
            return result;
        }

        /// <summary>
        /// Send POST HttpRequest
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        private static string POSTEoopHttpRequest(string requestUrl, string requestBody)
        {
            requestUrl = HttpUtility.UrlDecode(requestUrl, Encoding.UTF8);

            byte[] data = Encoding.UTF8.GetBytes(requestBody);
            System.Net.HttpWebRequest myRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUrl);
            myRequest.Method = "POST";
            string language = Thread.CurrentThread.CurrentCulture.Name;
            myRequest.Headers.Set(System.Net.HttpRequestHeader.AcceptLanguage, Thread.CurrentThread.CurrentCulture.Name);
            myRequest.Timeout = 8000;
            myRequest.ContentType = "application/json;charset=UTF-8";
            myRequest.Accept = "application/json;charset=UTF-8";
            myRequest.ContentLength = data.Length;
            System.IO.Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            System.Net.HttpWebResponse myResponse = (System.Net.HttpWebResponse)myRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            int status = (int)myResponse.StatusCode;
            reader.Close();
            return result;
        }


        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="inputSTR"></param>
        /// <returns></returns>
        private static string Md5Hash(string inputSTR)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(inputSTR));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }

    }
}
