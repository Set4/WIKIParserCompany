using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace WIKIParserCompany.Core
{
    /// <summary>
    /// отражает статус http ответа
    /// </summary>
    enum HttpRequesStatus
    {
        ResourceFound,
        ResourceNotFound,
        Error
    }

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
              
                if (httpclient == null)
                {
                    HttpClientHandler handler = new HttpClientHandler();

                handler.CookieContainer = Cookiecontainer;
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                handler.AllowAutoRedirect = false;

                    httpclient = new HttpClient(handler);


                    #region Устанавливаем заголовки запроса    httpclient.DefaultRequestHeaders.Add() 

                    httpclient.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                    httpclient.DefaultRequestHeaders.Add("Connection", @"keep-alive");

                    httpclient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

                    httpclient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.109 Safari/537.36");

                    httpclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

                    httpclient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");

                    #endregion
                }
                return httpclient;
            }
        }


        /// <summary>
        /// GET-запрос к сайту
        /// </summary>
        /// <param name="uri">адресс запроса</param>
        /// <returns>HttpRequesStatus-Статус ответа,string- ответ</returns>
        public Tuple<HttpRequesStatus, string> HttpGetRequest(string uri)
        {
            try
            {
                using (HttpResponseMessage response = HttpClient.GetAsync(uri).Result)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new Tuple<HttpRequesStatus, string>(HttpRequesStatus.ResourceFound, response.Content.ReadAsStringAsync().Result);

                        case HttpStatusCode.Found:
                            return new Tuple<HttpRequesStatus, string>(HttpRequesStatus.ResourceNotFound, String.Empty);

                        default: return new Tuple<HttpRequesStatus, string>(HttpRequesStatus.Error, String.Empty);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(HttpRequest-метод) :  {0}", ex.Message);

                return null;
            }
        }

        /// <summary>
        /// GET-запрос для получения картинки
        /// </summary>
        /// <param name="uri">адресс запроса</param>
        /// <returns>HttpRequesStatus-Статус ответа,byte[] - ответ</returns>
        public Tuple<HttpRequesStatus, byte[]> HttpLoadImage(string uri)
        {
            try
            {
                using (HttpResponseMessage response = HttpClient.GetAsync(uri).Result)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return new Tuple<HttpRequesStatus, byte[]>(HttpRequesStatus.ResourceFound, response.Content.ReadAsByteArrayAsync().Result);

                        case HttpStatusCode.Found:
                            return new Tuple<HttpRequesStatus, byte[]>(HttpRequesStatus.ResourceNotFound, null);

                        default: return new Tuple<HttpRequesStatus, byte[]>(HttpRequesStatus.Error, null);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(HttpLoadImage-метод) :  {0}", ex.Message);

                return null;
            }
        }

        
    }
}
