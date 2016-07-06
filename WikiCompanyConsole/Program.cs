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

        static string informationText = "Список команд \r\n /start-запуск поиска по списку компаний из файла. \r\n /search-поиск компании по введенному названию \r\n /exit-выход";


        static void Main(string[] args)
        {
            Presenter presenter=new Presenter(new Model.Model(inputpath, xmlpath, errorcompanipath), new Program());

            Console.WriteLine(informationText);

         


            while (true)
            {
               switch( Console.ReadLine())
                {
                    case "/start": Console.WriteLine("Старт:");
                        Task task = Task.Factory.StartNew(() =>
                        {
                            presenter.Start();
                        });
                        
                        break;
                    case "/search":
                        Console.Write("Введите название компании:"); 
                       presenter.SearchCompanu( Console.ReadLine());
                        break;
                    //case "/pause":presenter.Pause(); break;
                   // case "/stop": presenter.Stop(); break;
                    case "/exit": presenter.Exit(); break;

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
