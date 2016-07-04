using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace WikiCompanyConsole
{
    public interface IView
    {
      void  ViewMessage(string message);
    }

    class Program:IView
    {
        static string directory = Directory.GetCurrentDirectory();

        static string inputpath = directory + @"\input.txt";
        static string xmlpath = directory + @"\companies.xml";
        static string errorcompanipath = directory + @"\error.txt";

        static string informationText = "bla-bla-bla";


        static long stpopedvichislenie = 0;


        static void Main(string[] args)
        {
            Presenter presenter=new Presenter(new Model.Model(inputpath, xmlpath, errorcompanipath), new Program());

            Console.WriteLine(informationText);

            while (true)
            {
               switch( Console.ReadLine())
                {
                    case "/start": Console.WriteLine("Старт:"); presenter.Vichislenie(); break;
                    case "/search":
                        Console.Write("Введите название компании:"); Console.ReadLine();
                        break;
                    case "/pause":break;
                    case "/stop": break;
                    case "/exit": Console.WriteLine("Нажмите любую клавишу"); Console.ReadKey(); Environment.Exit(0); break;

                    default: Console.WriteLine("Неизвестная команда."); break;
                }
              
            }

            

        }

        public void ViewMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
