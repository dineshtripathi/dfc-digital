﻿using System;
using System.Web;

namespace DFC.Digital.Web.Core.HttpModules
{
    public class CustomHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose()
        {
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current?.Response.Headers.Remove("Server");

            // Or you can set something misleading
            //HttpContext.Current.Response.Headers.Set("Server", "NCS Server");
        }
    }
}