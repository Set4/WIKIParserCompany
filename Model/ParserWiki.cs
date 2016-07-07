using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Model
{
    enum ParseWikiStatus
    {
        CardCompanuSearch,
        MoreSearchResult,
        Error
    }

    class Parser
    {
        const double timewait = 1;


        /// <summary>
        /// Убирает из названия компании сокращения(ооо, фирма и т.д.)
        /// </summary>
        /// <param name="namecompani">название компании</param>
        /// <returns></returns>
        public string ReplaceNameCompany(string namecompani)
        {
            Regex reg;
            string result = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(namecompani.Trim()))
                    throw new NullReferenceException();

            

                reg = new Regex(@"(ООО)|("")|(')|(ЗАО)|(ОАО)|(\\(АО\\))|(ФГУП)|(КОНЦЕРН)|(ТС)|(ФИРМА)|(КОМПАНИЯ)",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

            


             
        result = reg.Replace(namecompani, "").Trim();
     
                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;


            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ReplaceNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }
        }


        /// <summary>
        /// определяет найдена ли страница компании, или поиск выдал мно-во результатов
        /// </summary>
        /// <param name="response"></param>
        /// <returns>ParseWikiStatus-результат поиска, string-html блок или empty</returns>
        public Tuple<ParseWikiStatus, string> CompaniIsSearth(string response)
        {
           
            string block = ParseWikiBlockCardCompany(response);

            if (!String.IsNullOrWhiteSpace(block))
                return new Tuple<ParseWikiStatus, string>(ParseWikiStatus.CardCompanuSearch, block);
            else if(MoreSearchResult(response))
                return new Tuple<ParseWikiStatus, string>(ParseWikiStatus.MoreSearchResult, String.Empty);
            else
                return new Tuple<ParseWikiStatus, string>(ParseWikiStatus.Error, String.Empty);
        }



        /// <summary>
        /// определяет страницу как страницу поиска
        /// </summary>
        /// <param name="response">html блок</param>
        /// <returns></returns>
        private bool MoreSearchResult(string response)
        {
            Regex reg;
            string result;
            try
            {
                if (String.IsNullOrWhiteSpace(response.Trim()))
                    throw new NullReferenceException();


                reg = new Regex("<h1.*?id=[\"]*firstHeading[\"].*?class=[\"] * firstHeading[\"]*.*?>+Результаты поиска+<\\/h1>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.CultureInvariant,
                    TimeSpan.FromSeconds(timewait));

                result = reg.Match(response).Value;

                if (!String.IsNullOrWhiteSpace(result))
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(MoreSearchResult-метод) :  {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Выделяет из html блока, карточку компании
        /// </summary>
        /// <param name="response">html блок</param>
        /// <returns>карточку компании</returns>
        private string ParseWikiBlockCardCompany(string response)
        {
            Regex reg;
            string result;
            try
            {
                if (String.IsNullOrWhiteSpace(response.Trim()))
                    throw new NullReferenceException();


                reg = new Regex("<table.*?class=[\"]*infobox vcard[\"]*.*?>+(.*?|\\s)+<\\/table>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.CultureInvariant,
                    TimeSpan.FromSeconds(timewait));

                result = reg.Match(response).Value;

                if (!String.IsNullOrWhiteSpace(result))
                    return result;
                else
                    return String.Empty;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseBlockCardCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

        }


        /*
        /// <summary>
        /// выполнение последовательности Regex операции
        /// </summary>
        /// <param name="response">html блок</param>
        /// <param name="regex">список Regex запросов</param>
        /// <returns></returns>
        private string ParseWikiInformationCompany(string response, List<string> regex)
        {
            Regex reg;
            string result= response;

            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                for (int i = 0; i < regex.Count; i++)
                {
                    reg = new Regex(regex[i],
                   RegexOptions.IgnoreCase
                   | RegexOptions.Multiline
                   | RegexOptions.CultureInvariant,
                   TimeSpan.FromSeconds(timewait));

                    result = reg.Match(result).Value;

                    if (String.IsNullOrWhiteSpace(result))
                        return String.Empty;
                }


                return result;


            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

        }
        */



        /// <summary>
        /// название компании
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiNameCompany(string response)
        {
            Regex reg;
            string result = response;
            StringBuilder namecomp = new StringBuilder();

            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                    reg = new Regex(@"<td.*?class=[""]*fn org[""]*.*?>+(.*?|\s)+<\/td>",
                   RegexOptions.IgnoreCase
                   | RegexOptions.Multiline
                   | RegexOptions.CultureInvariant,
                   TimeSpan.FromSeconds(timewait));

                    result = reg.Match(result).Value;

                    if (String.IsNullOrWhiteSpace(result))
                        return String.Empty;



                reg = new Regex(@">.*?<",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                foreach (Match m in reg.Matches(result))
                {

                    reg = new Regex(@"(>)|(<)|([+[0-9]+])|(\/td)",
                   RegexOptions.IgnoreCase
                   | RegexOptions.Multiline
                   | RegexOptions.CultureInvariant,
                   TimeSpan.FromSeconds(timewait));

                    if (m.Value != null)
                    {

                        namecomp = namecomp.Append(reg.Replace(m.Value, "").Trim());
                    }
                }


            

                if (String.IsNullOrWhiteSpace(namecomp.ToString()))
                    return String.Empty;
                else
                    return namecomp.ToString();



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }


         
        }

        /// <summary>
        /// ссылка на логотип компании
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiLogoCompany(string response)
        {

            Regex reg;
            string result = response;
        

            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"<td.*?class=[""]*logo[""]*.*?>+(.*?|\s)+<\/td>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@"src="".*?""",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;

                reg = new Regex(@"\""([^\""]+)\""",
             RegexOptions.IgnoreCase
             | RegexOptions.Multiline
             | RegexOptions.CultureInvariant,
             TimeSpan.FromSeconds(timewait));

               
                if (reg.Match(result).Groups.Count<2)
                    return String.Empty;

                result = reg.Match(result).Groups[1].Value;

            

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }


        }

        /// <summary>
        /// год создание компании
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiYearFoundatoinCompany(string response)
        {

            Regex reg;
            string result = response;


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();

              

                reg = new Regex(@"(Основание+(.*?|\s)+<\/p>)|(Год основания+(.*?|\s)+<\/p>)",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@"[0-9]{4}",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

             

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

        }

        /// <summary>
        /// страна
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiLocationCountryCompany(string response)
        {

            Regex reg;
            string result = response;


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Расположение+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@">.*?<",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));


                MatchCollection mColl = reg.Matches(result);
                    
                if (mColl.Count < 2)
                    return String.Empty;

                result = mColl[1].Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@"[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)*",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }


        }

        /// <summary>
        /// город
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiLocationCityCompany(string response)
        {
            Regex reg;
            string result = response;


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Расположение+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@">.*?<",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));


                MatchCollection mColl = reg.Matches(result);

                if (mColl.Count < 11)
                    return String.Empty;

                result = mColl[10].Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@"[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)*",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

        }

        /// <summary>
        /// значение уставного капитала
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiCharterCapitalCompany(string response)
        {
            Regex reg;
            string result = response;


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Уставный капитал+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;


              
                reg = new Regex(@"<p>*.*?<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;


                reg = new Regex(@">.*?<",
              RegexOptions.IgnoreCase
              | RegexOptions.Multiline
              | RegexOptions.CultureInvariant,
              TimeSpan.FromSeconds(timewait));

                MatchCollection mColl = reg.Matches(result);

                if (mColl.Count < 3)
                    return String.Empty;

                result = mColl[2].Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;


              
                 reg = new Regex(@">.*?\(",
               RegexOptions.IgnoreCase
                | RegexOptions.Multiline
                | RegexOptions.CultureInvariant,
                TimeSpan.FromSeconds(timewait));

                 result = reg.Match(result).Value;


                reg = new Regex(@"(>)|(&#160)|(;)|(\()",
                 RegexOptions.IgnoreCase
                 | RegexOptions.Multiline
                 | RegexOptions.CultureInvariant,
                 TimeSpan.FromSeconds(timewait));

     

                result = reg.Replace(result, "").Trim();
           


                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

      }

        /// <summary>
        /// дата значения уставного капитала
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiCharterCapitalDateCompany(string response)
        {

            Regex reg;
            string result = response;


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Уставный капитал+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;



                reg = new Regex(@"<p>*.*?<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;


                reg = new Regex(@">.*?<",
              RegexOptions.IgnoreCase
              | RegexOptions.Multiline
              | RegexOptions.CultureInvariant,
              TimeSpan.FromSeconds(timewait));

                MatchCollection mColl = reg.Matches(result);

                if (mColl.Count < 3)
                    return String.Empty;

                result = mColl[2].Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;




                reg = new Regex(@"[0-9]{4}",
               RegexOptions.IgnoreCase
              | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

           

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;
                else
                    return result;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }


        }


        /*

        //Ключевые фигуры+(.*?|\s)+<\/p> -Ключевые фигуры
        //>.*?< g,m 
        //replace (><)|(>[+[0-9]+]<)
        //replace (>)|(<)|(\()|(\))

        /// <summary>
        /// значения ключевых фигур капитала
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<People> ParseWikiKeyPeopleNameCompany(string response)
        {

            Regex reg;
            string result = response;
            List<People> peoples = new List<People>();


            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Ключевые фигуры+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return peoples;


                reg = new Regex(@">+\s",
            RegexOptions.IgnoreCase
            | RegexOptions.Multiline
            | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(timewait));
                result = reg.Replace(result, ">").Trim();

                if (String.IsNullOrWhiteSpace(result))
                    return peoples;

                reg = new Regex(@">.*?<",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));
               



                List<string> items = new List<string>();

                foreach (Match m in reg.Matches(result))
                {

                    reg = new Regex(@"(><)|(>[+[0-9]+]<)|(>)|(<)|(\()|(\))",
                     RegexOptions.IgnoreCase
                     | RegexOptions.Multiline
                     | RegexOptions.CultureInvariant,
                     TimeSpan.FromSeconds(timewait));



                    string s = reg.Replace(m.Value, "").Trim();
                    if (!String.IsNullOrWhiteSpace(s))
                        items.Add(s);
                }




                for (int i = 0; i < items.Count;)
                {
                    People people = new People();

                    string []s = items[0].Split(' ');
                    if (s.Length > 1)
                    {
                        people.FirstName = s[0];
                        people.LastName = s[1];
                    }
                    else
                    {
                        people.FirstName = items[0];
                        people.LastName = String.Empty;
                    }
                   

                    people.Position = items[1];

                    peoples.Add(people);

                    i += 2;
                }

                    return peoples;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return peoples;
            }


        }


    */

        /// <summary>
        /// значения ключевых фигур капитала
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string ParseWikiKeyPeopleNameCompany(string response)
        {

            Regex reg;
            string result = response;



            try
            {
                if (String.IsNullOrWhiteSpace(result.Trim()))
                    throw new NullReferenceException();



                reg = new Regex(@"Ключевые фигуры+(.*?|\s)+<\/p>",
               RegexOptions.IgnoreCase
               | RegexOptions.Multiline
               | RegexOptions.CultureInvariant,
               TimeSpan.FromSeconds(timewait));

                result = reg.Match(result).Value;

                if (String.IsNullOrWhiteSpace(result))
                    return String.Empty;


                reg = new Regex(@"(<a.*?href=*.*?>)|(Ключевые фигуры<\/th>)|(<td.*?class=*.*?>)|(<span .*?style=*.*?>)|(<\/span>)|(<\/a>)|(<p>)|(<\/p>)|(&#160;)|(<br \/>)|([+[0-9]+])|(<sup.*?class=*.*?>)|(<\/sup>)|(<li>)|(<\/li>)|(<ul>)|(<\/ul>)|(<\/td>)|(<\/tr>)",
                         RegexOptions.IgnoreCase
                         | RegexOptions.Multiline
                         | RegexOptions.CultureInvariant,
                         TimeSpan.FromSeconds(timewait));



                result = reg.Replace(result, "").Trim();
                if (!String.IsNullOrWhiteSpace(result))
                    return result;
                else
                    return String.Empty;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }


        }

    }



}
