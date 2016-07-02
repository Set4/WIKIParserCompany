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
            /*
            XmlDocument xDoc = new XmlDocument();
            
            xDoc.Load("users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            // создаем новый элемент user
            XmlElement userElem = xDoc.CreateElement("user");
            // создаем атрибут name
            XmlAttribute nameAttr = xDoc.CreateAttribute("name");
            // создаем элементы company и age
            XmlElement companyElem = xDoc.CreateElement("company");
            XmlElement ageElem = xDoc.CreateElement("age");
            // создаем текстовые значения для элементов и атрибута
            XmlText nameText = xDoc.CreateTextNode("Mark Zuckerberg");
            XmlText companyText = xDoc.CreateTextNode("Facebook");
            XmlText ageText = xDoc.CreateTextNode("30");

            //добавляем узлы
            nameAttr.AppendChild(nameText);
            companyElem.AppendChild(companyText);
            ageElem.AppendChild(ageText);
            userElem.Attributes.Append(nameAttr);
            userElem.AppendChild(companyElem);
            userElem.AppendChild(ageElem);
            xRoot.AppendChild(userElem);
            xDoc.Save("users.xml");
            */






            try
            {

                XDocument xdoc = new XDocument();



                // создаем второй элемент
                XElement comp = new XElement("company");

                XAttribute NameCompanyAttr = new XAttribute("name", company.NameCompany);
                comp.Add(NameCompanyAttr);


                XElement logo = new XElement("logo", company.LogoCompany);

                
                XElement country = new XElement("country", company.Location.Country);
                comp.Add(country);

                XElement city = new XElement("city", company.Location.City);
                comp.Add(city);


                XElement year = new XElement("year", company.YearOfFoundation);
                comp.Add(year);

                XElement galaxysPriceElem = new XElement("keypeople", company.KeyPeople);

                XElement capital = new XElement("capital", company.CharterCapital);
                comp.Add(capital);



                // создаем корневой элемент
                XElement companys = new XElement("companys");

                // добавляем в корневой элемент
                companys.Add(comp);


                // добавляем корневой элемент в документ
                xdoc.Add(companys);

                //сохраняем документ
                xdoc.Save(path);

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
