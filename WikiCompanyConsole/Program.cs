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


        static void Main(string[] args)
        {
            Presenter presenter=new Presenter(new Model.Model(inputpath, xmlpath, errorcompanipath), new Program());

            Console.WriteLine(informationText);

            Task task;


         
            Console.ReadKey();

         
            while (true)
            {
               switch( Console.ReadLine())
                {
                    case "/start": Console.WriteLine("Старт:"); task= new Task(presenter.Start); task.Start();
 break;
                    case "/search":
                        Console.Write("Введите название компании:"); 
                       presenter.SearchCompanu( Console.ReadLine());
                        break;
                    case "/pause":presenter.Pause(); break;
                    case "/stop": presenter.Stop(); break;
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
