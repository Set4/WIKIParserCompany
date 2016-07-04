using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Model
{
    public enum Result
    {
        CompanySaved,
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

        void GetCompanis(List<string> items);
        void Get(string namecompany);
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
            {
                return TXTProvider.ReadTextFile(pathloadtxt);
            }
        }


        public void GetCompanis(List<string> items)
        {

            ParallelLoopResult result = Parallel.ForEach<string>(items, Get);

           

            //parallel
           // Message
        }

        public void Get(string namecompany)
        {
            Tuple<Result, string> result = GetCompany(namecompany);
            Message(this, new EventMessage(result.Item1, result.Item2));
        }



        public Tuple<Result, string> GetCompany(string namecompany)
        {
            HTTPProvider http = new HTTPProvider();
            Tuple<HttpRequesStatus, string> response = http.HttpGetRequest(uripath1 + namecompany + uripath2);

            switch (response.Item1)
            {
                case HttpRequesStatus.Error:
                    return new Tuple<Result, string>(Result.Error, "ERROR. HTTP запрос вызвал ошибку");
                case HttpRequesStatus.ResourceNotFound:

                    lock (locker1)
                    {
                        if (SaveErrorCompanyToTxT(namecompany))
                            return new Tuple<Result, string>(Result.Error, "Warning. Компания " + namecompany + " не найдена. ");
                        else
                            return new Tuple<Result, string>(Result.Error, "ERROR. txt фаил 'не найденных компаний' вызвал ошибку");
                    }
                case HttpRequesStatus.ResourceFound: break;
            }



            ParserWiki parse = new ParserWiki();
            string wikicompanyblock = parse.ParseWikiBlockCardCompany(response.Item2);

            if (String.IsNullOrEmpty(wikicompanyblock))
                return new Tuple<Result, string>(Result.Error, "ERROR. Поиск карточки компании " + namecompany + " в HTTP ответе неудачен");




            Company company = new Company();

            company.NameCompany =
                 parse.ParseWikiInformationCompany(wikicompanyblock,
                 new List<string>() { @"<td.*?class=[""]*fn org[""]*.*?>+(.*?|\s)+<\/td>", @">.*?<", @"[а-я А-Я 0-9]" });



            //zagryzka foto
            string sourcefoto = parse.ParseWikiInformationCompany(wikicompanyblock,
                new List<string>() { @"<td.*?class=[""]*logo[""]*.*?>+(.*?|\s)+<\/td>", @"src="".*?""", @"\""([^\""]+)\""" });
            if (!String.IsNullOrWhiteSpace(sourcefoto))
            {
                Tuple<HttpRequesStatus, byte[]> logoresponse = http.HttpLoadImage(sourcefoto);
                switch (logoresponse.Item1)
                {
                    case HttpRequesStatus.Error:
                        return new Tuple<Result, string>(Result.Error, "ERROR. HTTP запрос вызвал ошибку");
                    case HttpRequesStatus.ResourceNotFound:
                        return new Tuple<Result, string>(Result.Error, "ERROR. Logotip компаний не найден");
                    case HttpRequesStatus.ResourceFound:
                        company.LogoCompany = logoresponse.Item2;
                        break;
                }
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


            lock (locker)
            {

                string result = SaveCompanyToXML(company);
            }

            if (result != null)
                return new Tuple<Result, string>(Result.CompanySaved, "Successful. Компания " + result + " найдена и сохранена в XML фаил");
            else
                return new Tuple<Result, string>(Result.Error, "ERROR. Компании " + namecompany + " не сохранена в XML фаил");




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
