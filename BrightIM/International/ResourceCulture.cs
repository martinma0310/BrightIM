﻿using System.Reflection;
using System.Resources;
using System.Threading;
using System.Globalization;

namespace BrightIM
{
    public class ResourceCulture

    {
        /// <summary>
        /// Set current culture by name
        /// </summary>
        /// <param name="name">name</param>
        public static void SetCurrentCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "zh-CN";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
        }

        /// <summary>
        /// Get string by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>current language string</returns>
        public static string GetString(string id)
        {
            string strCurLanguage = "";
            try
            {
                ResourceManager rm = new ResourceManager("BrightIM.International.ResourceShow", Assembly.GetExecutingAssembly());
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;
                strCurLanguage = rm.GetString(id, ci);
            }
            catch
            {
                strCurLanguage = "No id:" + id + ", please add.";
            }
            return strCurLanguage;
        }
    }
}
