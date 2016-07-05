using System;
using System.Collections.Generic;
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
    public enum Result
    {
        CompanySaved,
        SearthMoreCompanis,
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
        void Pririvanie(CancellationTokenSource cancelTokenSource);

      
    }




    public class Model:IModel
    {
        string pathloadtxt;
        string pathsavexml;
        string pathsavetxt;

        const string uripath1 = @"https://ru.wikipedia.org/wiki/Special:Search?search=";
        const string uripath2 = @"&go=Go";

        public event EventHandler<EventMessage>  Message=delegate{};



        static object locker = new object();
        static object locker1 = new object();

      

        public Model(string pathloadtxt, string pathsavexml, string pathsavetxt)
        {
            this.pathloadtxt = pathloadtxt;
            this.pathsavexml = pathsavexml;
            this.pathsavetxt = pathsavetxt;

            
        }


        public List<string> GetListCompanyToTxT()
        {
            if (String.IsNullOrWhiteSpace(pathloadtxt.Trim()) && !System.IO.File.Exists(pathloadtxt.Trim()))
                return null;
            else
               return TXTProvider.ReadTextFile(pathloadtxt);
        }





        public ParallelLoopResult GetParallelCompanis(List<string> items, CancellationToken token, CancellationTokenSource cancelTokenSource)
        {
            ParallelLoopResult result = new ParallelLoopResult();
            try
            {

                result = Parallel.ForEach<string>(items, new ParallelOptions { CancellationToken = token }, Get);
                return result;

            }
            finally
            {
                cancelTokenSource.Dispose();
            }

        }




        public void Pririvanie(CancellationTokenSource cancelTokenSource)
        {
            cancelTokenSource.Cancel();
        }




        public Tuple<Result, string> GetCompany(string namecompany)
        {
            HTTPProvider http = new HTTPProvider();
            string response = http.HttpGetRequest(uripath1 + namecompany + uripath2);

            if (response == null)
            {
                lock (locker1)
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

                case ParseWikiStatus.MoreSearchResult:
                    lock (locker1)
                    {
                        if (SaveErrorCompanyToTxT(namecompany))
                            return new Tuple<Result, string>(Result.SearthMoreCompanis, "Warning. Поиск Компании " + namecompany + " выдол несколко вариантов. Компания Сохранена в фаил.");
                        else
                            return new Tuple<Result, string>(Result.Error, "ERROR. txt фаил 'не найденных компаний' вызвал ошибку");
                    }

                case ParseWikiStatus.Error:
                    return new Tuple<Result, string>(Result.Error, "ERROR. Поиск карточки компании " + namecompany + "неудачен");
            }

        


            Company company = new Company();

            company.NameCompany =
                 parse.ParseWikiInformationCompany(wikicompanyblock,
                 new List<string>() { @"<td.*?class=[""]*fn org[""]*.*?>+(.*?|\s)+<\/td>", @">.*?<", @"[а-я А-Я 0-9]" });



            //zagryzka foto
            string sourcefoto = parse.ParseWikiInformationCompany(wikicompanyblock,
                new List<string>() { @"<td.*?class=[""]*logo[""]*.*?>+(.*?|\s)+<\/td>", @"src="".*?""", @"\""([^\""]+)\""" });
            if (!String.IsNullOrWhiteSpace(sourcefoto))
            {
                byte[] logoresponse = http.HttpLoadImage(sourcefoto);
                if (logoresponse == null)
                    return new Tuple<Result, string>(Result.Error, "ERROR. Logotip компаний не найден");
                else
                    company.LogoCompany = logoresponse;

            }


            //Год основания
            company.YearOfFoundation = parse.ParseWikiInformationCompany(wikicompanyblock,
                new List<string>() { @"Год основания+(.*?|\s)+<\/p>", @"[0-9]{4}" });

            //Расположениe            
            company.Location.Country = parse.ParseWikiInformationCompany(wikicompanyblock,
               new List<string>() { @"Расположение+(.*?|\s)+<\/p>", @">.*?<", @"[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)*" });

            company.Location.City = parse.ParseWikiInformationCompany(wikicompanyblock,
               new List<string>() { @"Расположение+(.*?|\s)+<\/p>", @">.*?<", @"[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)*" });


            //Уставный капитал       
            company.CharterCapital.CharterCapital = parse.ParseWikiInformationCompany(wikicompanyblock,
               new List<string>() { @"Уставный капитал+(.*?|\s)+<\/p>", @"<p>*.*?<\/p>" });

            company.CharterCapital.Date = parse.ParseWikiInformationCompany(wikicompanyblock,
               new List<string>() { @"Уставный капитал+(.*?|\s)+<\/p>", @"(0[1-9]|[12][0-9]|3[01])[- \/.](0[1-9]|1[012])[-\/.](19|20)\d\" });

            string result;

            lock (locker)
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
