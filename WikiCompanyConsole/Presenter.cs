using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WikiCompanyConsole
{
    enum StatusVichislenia
    {
        Stop,
        Pause
    }
    class Presenter
    {

        readonly IModel _model;
        readonly IView _view;


        List<string> companies;
       

        CancellationTokenSource cancelTokenSource;
        CancellationToken token;


    

        public Presenter(IModel model, IView view)
        {
            _model = model;
            _view = view;

            _model.Message += _model_Message;


            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

          
        }


        private void _model_Message(object sender, EventMessage e)
        {
            // _view.ViewMessage(e.Message);
            Debug.WriteLine("Message :  {0}", e.Message);
        }







        private bool GetListCompanies()
        {
           
            companies = _model.GetListCompanyToTxT();

            if (companies == null || companies.Count == 0)
            {
                _view.ViewMessage("Ошибка!!! Список компаний не загружен!");
                return false;
            }
            else
            {
                _view.ViewMessage("Загружен список из " + companies.Count + " компаний");
                return true;
            }

        }



      


        public void Start()
        {
            _view.ViewMessage("Начат поиск по списку компаний");
           
                if (GetListCompanies())
                    _model.GetParallelCompanis(companies,
                                            token, cancelTokenSource);


            _view.ViewMessage("Поиск завершен");
        }

        //public void Pause()
        //{
        //    _model.CancelParallelSaerchCompanies(cancelTokenSource);
        //    status = StatusVichislenia.Pause;
        //    _view.ViewMessage("Поиск приостановлен");
        //}


        //public void Stop()
        //{
        //    _model.CancelParallelSaerchCompanies(cancelTokenSource);
        //    status = StatusVichislenia.Stop;
        //    _view.ViewMessage("Поиск остановлен");
        //}

        public void Exit()
        {
            _view.ViewMessage("Нажмите любую клавишу");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public void SearchCompanu(string company)
        {
            if (!String.IsNullOrWhiteSpace(company.Trim()))
            {
                if (_model.GetCompany(company.Trim()).Item1 == Result.CompanySaved)
                    _view.ViewMessage("Компания " + company + " найдена и созранена в XML файле.");
                else
                    _view.ViewMessage("Компания " + company + " не найдена!!!");
            }
            else
                _view.ViewMessage("Ошибка!!! Название компании введено не верно!");
        }
    }
}
