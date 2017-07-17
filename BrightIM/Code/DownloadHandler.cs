using CefSharp;
using System;
using System.Windows.Forms;

namespace BrightIM
{ 
    public class DownloadHandler : IDownloadHandler
    {
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        { 

            var handler = OnBeforeDownloadFired;
            if (handler != null)
            {
                //handler(this, downloadItem);
                handler(browser, downloadItem);
            }

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
                }
            }

            //callback.Continue(string.Empty, true);
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            var handler = OnDownloadUpdatedFired;
            if (handler != null)
            {
                //handler(this, downloadItem);
                handler(browser, downloadItem);
            }

            //if (downloadItem.IsComplete)
            //{ 
            //    if (browser.IsPopup && !browser.HasDocument)
            //    {
            //        browser.GetHost().CloseBrowser(true);
            //    }
            //}
        }


        //public  void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        //{
        //    callback.Continue(string.Empty, true);
        //}
        //public void OnDownloadUpdated(IBrowser browser, CefSharp.DownloadItem downloadItem,IDownloadItemCallback callback)
        //{
        //    if (downloadItem.IsComplete)
        //    {
        //        MessageBox.Show("下载成功");
        //        if (browser.IsPopup && !browser.HasDocument)
        //        { 
        //            browser.GetHost().CloseBrowser(true);
        //        }
        //    }
        //}

    }
}
