using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace WIKIParserCompany.Core
{
    class HTTPProvider
    {

        public HTTPProvider(Uri uri)
        {
            this.uri = uri;
            Cookiecontainer = new CookieContainer();

        }

        /// <summary>
        /// адрес запроса new Uri("");
        /// </summary>
     public Uri uri { get; private set; }

        CookieContainer Cookiecontainer { get; set; }

        HttpClient httpclient = null;
        HttpClient HttpClient
        {
            get
            {
                HttpClientHandler handler = new HttpClientHandler(); 

                handler.CookieContainer = Cookiecontainer;
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                handler.AllowAutoRedirect = false;

                if (httpclient == null)
                {
                    httpclient = new HttpClient(handler);
                    httpclient.BaseAddress = uri;

                    #region Устанавливаем заголовки запроса    httpclient.DefaultRequestHeaders.Add() 

                    httpclient.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                    httpclient.DefaultRequestHeaders.Add("Connection", @"keep-alive");

                    httpclient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

                    httpclient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.109 Safari/537.36");



                    httpclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

                    httpclient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");



                    httpclient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                    #endregion
                }
                return httpclient;
            }
        }


    



    }
}
