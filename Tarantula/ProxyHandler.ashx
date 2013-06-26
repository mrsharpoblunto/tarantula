<%@ WebHandler Language="C#" Class="ProxyService.ProxyHandler" %>
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net;
using System.Collections.Generic;
 
namespace ProxyService
{
 
    public class ProxyHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Headers["X-Proxy-Url"])) return;
            
            string proxyUrl = context.Request.Headers["X-Proxy-Url"].ToString();

            if (!proxyUrl.StartsWith("http://ecs.amazonaws.com/onca/xml?")) return;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(proxyUrl);

            req.ContentType = context.Request.ContentType;
            req.Method = context.Request.HttpMethod;
 
            if (req.Method == "POST")
            {
                string input = new StreamReader(context.Request.InputStream).ReadToEnd();
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] bytesToSend = encoding.GetBytes(input);
                req.ContentLength = bytesToSend.Length;
                Stream newStream = req.GetRequestStream();
                newStream.Write(bytesToSend, 0, bytesToSend.Length);
                newStream.Close();
            }
 
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                ServeResponse(context, resp);
            }
            catch (WebException ex)
            {
                HttpWebResponse resp = (HttpWebResponse)ex.Response;
                ServeResponse(context, resp);
            }
        }

        private void ServeResponse(HttpContext context,HttpWebResponse resp)
        {
            context.Response.StatusCode = (int)resp.StatusCode;
            context.Response.ContentType = resp.ContentType;
            Stream respStream = resp.GetResponseStream();
            StreamReader r = new StreamReader(respStream);
            string output = r.ReadToEnd();
            context.Response.Write(output);
        }
 
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}