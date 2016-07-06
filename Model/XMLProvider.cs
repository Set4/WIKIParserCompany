using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Model
{
   class XMLProvider
    {
        string path;

        public XMLProvider(string path)
        {
            this.path = path;
        }

        public string SaveCompany(Company company)
        {

            try
            {
                XDocument doc;

                if (!System.IO.File.Exists(path.Trim()))
                {
                    doc = new XDocument(new XElement("companies"));
                }
                else
                {
                    doc = XDocument.Load(path);
                }

                XElement item = new XElement("company");
                //добавляем необходимые атрибуты
                item.Add(new XAttribute("name", company.NameCompany));
                item.Add(new XAttribute("year", company.YearOfFoundation));
                //кодирование logo в base64
                item.Add(new XAttribute("logo", Convert.ToBase64String(company.LogoCompany)));


                //создаем элемент "location"
                XElement location = new XElement("location");
                location.Add(new XAttribute("country", company.Location.Country));
                location.Add(new XAttribute("city", company.Location.City));
                item.Add(location);

                //создаем элемент "capital"
                XElement capital = new XElement("capital");
                capital.Add(new XAttribute("chartercapital", company.CharterCapital.CharterCapital));
                capital.Add(new XAttribute("date", company.CharterCapital.Date));
                item.Add(capital);

                //создаем элемент "keypeoples"
               
                 XElement keypeoples = new XElement("keypeoples", company.KeyPeople);

                //foreach (People p in company.KeyPeople)
                //{
                //    XElement people = new XElement("people");
                //    people.Add(new XAttribute("lastname", p.LastName));
                //    people.Add(new XAttribute("firstname", p.FirstName));
                //    people.Add(new XAttribute("position", p.Position));
                //    keypeoples.Add(people);
                //}
               item.Add(keypeoples);




                doc.Root.Add(item);
                doc.Save(path);
                return company.NameCompany;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(SaveCompany-метод) :  {0}", ex.Message);

                return null;
            }
        }
    }
}
