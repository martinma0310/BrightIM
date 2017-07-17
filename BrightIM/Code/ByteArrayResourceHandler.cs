﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BrightIM.Code
{
    public class ByteArrayResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Underlying byte array that represents the data
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Gets or sets the Mime Type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayResourceHandler"/> class.
        /// </summary>
        /// <param name="mimeType">mimeType</param>
        /// <param name="filePath">filePath</param>
        public ByteArrayResourceHandler(string mimeType, byte[] data)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentNullException("mimeType", "Please provide a valid mimeType");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data", "Please provide a valid array");
            }


            MimeType = mimeType;
            Data = data;
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        bool IResourceHandler.CanGetCookie(Cookie cookie)
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        bool IResourceHandler.CanSetCookie(Cookie cookie)
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        void IResourceHandler.Cancel()
        {
            //Should never be called
            throw new NotImplementedException("This method should never be called");
        }

        void IDisposable.Dispose()
        {
            //NOOP
        }

        public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
