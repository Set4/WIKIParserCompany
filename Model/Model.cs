using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// результат поиска и парсинга компании
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// компания сохранена
        /// </summary>
        CompanySaved,
        /// <summary>
        /// поиск компании выдал неопределенный результат
        /// </summary>
        SearthMoreCompanis,
        /// <summary>
        /// произошла ошибка
        /// </summary>
        Error
    }


    public class EventMessage
    {
        public string Message { get; private set; }
        public Result ResultOperation { get; private set; }

        public EventMessage(Result result, string message)
        {
            Message = message;
            ResultOperation = result;
        }
    }



    public interface IModel
    {
        List<string> GetListCompanyToTxT();
        Tuple<Result, string> GetCompany(string namecompany);

        event EventHandler<EventMessage> Message;

        ParallelLoopResult GetParallelCompanis(List<string> items, CancellationToken token, CancellationTokenSource cancelTokenSource);
       // void CancelParallelSaerchCompanies(CancellationTokenSource cancelTokenSource);

      
    }




    public class Model:IModel
    {

        string pathloadtxt;
        string pathsavexml;
        string pathsavetxt;

        const string uripath1 = @"https://ru.wikipedia.org/wiki/Special:Search?search=";
        const string uripath2 = @"&go=Go";


        public event EventHandler<EventMessage>  Message=delegate{};



        static object lockerXML = new object();
        static object lockerTXT = new object();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathloadtxt">путь к файлу .txt со списком компаний</param>
        /// <param name="pathsavexml">путь к файлу .xml для хранения компаний</param>
        /// <param name="pathsavetxt">путь к файлу .txt со списком не найденных компаний</param>
        public Model(string pathloadtxt, string pathsavexml, string pathsavetxt)
        {
            this.pathloadtxt = pathloadtxt;
            this.pathsavexml = pathsavexml;
            this.pathsavetxt = pathsavetxt;

            
        }

        /// <summary>
        /// загрузка списка названий компаний
        /// </summary>
        /// <returns></returns>
        public List<string> GetListCompanyToTxT()
        {
            if (String.IsNullOrWhiteSpace(pathloadtxt.Trim()) && !System.IO.File.Exists(pathloadtxt.Trim()))
                return null;
            else
               return TXTProvider.ReadTextFile(pathloadtxt);
        }




        /// <summary>
        /// Parallel поиска компаний по списку
        /// </summary>
        /// <param name="items">список названий компаний</param>
        /// <param name="token">токен отмены вычисления</param>
        /// <param name="cancelTokenSource"></param>
        /// <returns></returns>
        public ParallelLoopResult GetParallelCompanis(List<string> items, CancellationToken token, CancellationTokenSource cancelTokenSource)
        {
            Task.Delay(3000);


            return Parallel.ForEach<string>(items, new ParallelOptions { CancellationToken = token }, Get);
        }



        ///// <summary>
        ///// отмена Parallel поиска компаний по списку
        ///// </summary>
        ///// <param name="cancelTokenSource"></param>
        //public void CancelParallelSaerchCompanies(CancellationTokenSource cancelTokenSource)
        //{
        //    cancelTokenSource.Cancel();
        //}



        /// <summary>
        /// поиска и получение информации из WIKI о компаний по названию
        /// </summary>
        /// <param name="namecompany"></param>
        /// <returns></returns>
        public Tuple<Result, string> GetCompany(string namecompany)
        {
            HTTPProvider http = new HTTPProvider();
            string response = http.HttpGetRequest(uripath1 + namecompany + uripath2);

            if (response == null)
            {
                lock (lockerTXT)
                {
                    if (SaveErrorCompanyToTxT(namecompany))
                        return new Tuple<Result, string>(Result.Error, "ERROR. HTTP запрос" + namecompany + " вызвал ошибку. Компания  Сохранена в фаил.");
                    else
                        return new Tuple<Result, string>(Result.Error, "ERROR. txt фаил 'не найденных компаний' вызвал ошибку");
                }
            }


            Parser parse = new Parser();
            string wikicompanyblock=String.Empty;
            Tuple<ParseWikiStatus, string> parseresult = parse.CompaniIsSearth(response);
            switch (parseresult.Item1)
            {
                case ParseWikiStatus.CardCompanuSearch:
                    wikicompanyblock = parseresult.Item2; break;
                default:
                    lock (lockerTXT)
                    {
                        if (SaveErrorCompanyToTxT(namecompany + " (ошибка поиска)"))
                            return new Tuple<Result, string>(Result.SearthMoreCompanis, "Warning. Поиск Компании " + namecompany + " выдaл ошибку. Компания Сохранена в фаил 'не найденных компаний'.");
                        else
                            return new Tuple<Result, string>(Result.Error, "ERROR. txt фаил 'не найденных компаний' вызвал ошибку");
                    }
            }

        


            Company company = new Company();

            company.NameCompany = parse.ParseWikiNameCompany(wikicompanyblock);


            //zagryzka foto
            string sourcefoto = parse.ParseWikiLogoCompany(wikicompanyblock);
            if (!String.IsNullOrWhiteSpace(sourcefoto))
            {
                byte[] logoresponse = http.HttpLoadImage("https:"+sourcefoto);
                if (logoresponse != null)
                    company.LogoCompany = logoresponse;
                else
                    company.LogoCompany = new byte[] { };
            }
            else
                company.LogoCompany = new byte[] { };

            //Год основания
            company.YearOfFoundation = parse.ParseWikiYearFoundatoinCompany(wikicompanyblock);

            //Расположениe      
            company.Location = new LocationCompany();                  
            company.Location.Country = parse.ParseWikiLocationCountryCompany(wikicompanyblock);

            company.Location.City = parse.ParseWikiLocationCityCompany(wikicompanyblock);


            //Уставный капитал       
            company.CharterCapital = new Capital();
            company.CharterCapital.CharterCapital = parse.ParseWikiCharterCapitalCompany(wikicompanyblock);
            company.CharterCapital.Date = parse.ParseWikiCharterCapitalDateCompany(wikicompanyblock);

            //Ключевые фигуры
            company.KeyPeople = parse.ParseWikiKeyPeopleNameCompany(wikicompanyblock);

            string result;

            lock (lockerXML)
            {

                result = SaveCompanyToXML(company);
            }

            if (result != null)
                return new Tuple<Result, string>(Result.CompanySaved, "Successful. Компания " + result + " найдена и сохранена в XML фаил");
            else
                return new Tuple<Result, string>(Result.Error, "ERROR. Компании " + namecompany + " не сохранена в XML фаил");




        }




     





        private void Get(string namecompany)
        {
            Tuple<Result, string> result = GetCompany(namecompany);
            Message(this, new EventMessage(result.Item1, result.Item2));
        }


        /// <summary>
        /// запись названия не найденной компании в фаил
        /// </summary>
        /// <param name="namecompany"></param>
        /// <returns></returns>
        private bool SaveErrorCompanyToTxT(string namecompany)
        {

            if (String.IsNullOrWhiteSpace(pathsavetxt.Trim()))
                return false;
            else
            {
                TXTProvider.WriteTextFile(pathsavetxt, namecompany);
                return true;
            }
        }

        /// <summary>
        /// запись информации о компании в фаил
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        private string SaveCompanyToXML(Company company)
        {

            if (String.IsNullOrWhiteSpace(pathsavexml.Trim()) && company == null)
                return null;
            else
            {
                XMLProvider xml = new XMLProvider(pathsavexml);
                return xml.SaveCompany(company);
            }
        }

    }
}
