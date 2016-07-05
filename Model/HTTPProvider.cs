using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Model
{
   /// <summary>
   /// Класс реализующий http get-запрос
   /// </summary>
    class HTTPProvider
    {

        public HTTPProvider()
        {
           
            Cookiecontainer = new CookieContainer();

        }

     

        CookieContainer Cookiecontainer { get; set; }

        HttpClient httpclient = null;
        HttpClient HttpClient
        {
            get
            {

                if (httpclient == null)
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        //Добавление заголовков
                        //Proxy = new WebProxy("http://127.0.0.1:8888"),
                        //UseProxy = true,
                    };



                    httpclient = new HttpClient(handler);

                    var byteArray = Encoding.ASCII.GetBytes("username:password");
                    httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                }
                return httpclient;
            }
        }




    /// <summary>
    /// GET-запрос к сайту
    /// </summary>
    /// <param name="uri">адресс запроса </param>
    /// <returns>string- ответ</returns>
    public string HttpGetRequest(string uri)
        {
            try
            {
                using (HttpResponseMessage response = HttpClient.GetAsync(uri).Result)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return response.Content.ReadAsStringAsync().Result;
                    else
                        return String.Empty;


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
        /// <returns>byte[] - ответ</returns>
        public byte[] HttpLoadImage(string uri)
        {
            try
            {
                using (HttpResponseMessage response = HttpClient.GetAsync(uri).Result)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return response.Content.ReadAsByteArrayAsync().Result;
                    else
                        return null;

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
