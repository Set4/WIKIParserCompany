﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Model
{
    class ParserWiki
    {
        const double timewait = 1;

        //g, m !!!!!!!!!!!!!

        //<table.*?class=["]*infobox vcard["]*.*?>+(.*?|\s)+<\/table> - block card



        //<td.*?class=["]*fn org["]*.*?>+(.*?|\s)+<\/td> -compania
        //>.*?< g,m 
        //[а-я А-Я 0-9] g


        //<td.*?class=["]*logo["]*.*?>+(.*?|\s)+<\/td> -logotip
        //src=".*?"  g,m 
        //\"([^\"]+)\"  g  - group 1 или удалить ""


        //Год основания+(.*?|\s)+<\/p> -Год основания
        //[0-9]{4} g


        //Расположение+(.*?|\s)+<\/p>- Расположение
        //>.*?< g,m 
        //[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)*


        //Ключевые фигуры+(.*?|\s)+<\/p> -Ключевые фигуры
        //>.*?< g,m 
        //\(.*?\) doljnosti izat i ydalit
        //[a-zA-Zа-яА-ЯёЁ]+(?:[ '-][a-zA-Zа-яА-ЯёЁ]+)* vse
        //\(.*?\) doljnosti


        //Уставный капитал+(.*?|\s)+<\/p>-Уставный капитал
        // <p>*.*?<\/p> - <p>24,532&#160;млрд руб. (10.10.2012 год)</p>

        //(0[1-9]|[12][0-9]|3[01])[- \/.](0[1-9]|1[012])[-\/.](19|20)\d\ data 
        //[0-9]{4} g или только год
       
            
             /*
              * 
              * 
   private string ParseNameCompany(string response)
        {
            Regex reg;
            string result;

            try
            {
                if (String.IsNullOrWhiteSpace(response.Trim()))
                    throw new NullReferenceException();


                reg = new Regex("<td.*?class=[\"]*fn org[\"]*.*?>+(.*?|\\s)+<\\/td>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.CultureInvariant,
                    TimeSpan.FromSeconds(timewait));

                result = reg.Match(response).Value;

                if (!String.IsNullOrWhiteSpace(result))
                {
                    reg = new Regex(">.*?<",
                 RegexOptions.IgnoreCase
                 | RegexOptions.Multiline
                 | RegexOptions.CultureInvariant,
                 TimeSpan.FromSeconds(timewait));

                    result = reg.Match(result).Value;

                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        reg = new Regex("[а-я А-Я 0-9]",
                 RegexOptions.IgnoreCase
                 | RegexOptions.Multiline
                 | RegexOptions.CultureInvariant,
                 TimeSpan.FromSeconds(timewait));


                        result = reg.Match(result).Value;

                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            return result;
                        }
                        else
                            return String.Empty;
                    }
                    else
                        return String.Empty;
                }
                else
                    return String.Empty;



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ParseNameCompany-метод) :  {0}", ex.Message);
                return String.Empty;
            }

        }
        */



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

     


        private string ParseWikiInformationCompany(string response, List<string> regex)
        {
            Regex reg;
            string result=String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(response.Trim()))
                    throw new NullReferenceException();



                for (int i = 0; i < regex.Count; i++)
                {
                    reg = new Regex(regex[i],
                   RegexOptions.IgnoreCase
                   | RegexOptions.Multiline
                   | RegexOptions.CultureInvariant,
                   TimeSpan.FromSeconds(timewait));

                    result = reg.Match(response).Value;

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

    }
}
