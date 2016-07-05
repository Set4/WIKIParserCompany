using Model;
using System;
using System.Collections.Generic;
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

        ParallelLoopResult result;

        StatusVichislenia status;
        

        public Presenter(IModel model, IView view)
        {
            _model = model;
            _view = view;

            _model.Message += _model_Message;


            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

          
            result = new ParallelLoopResult();

            status = StatusVichislenia.Stop;
           
        }


        private void _model_Message(object sender, EventMessage e)
        {
            _view.ViewMessage(e.Message);
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
            if (result.IsCompleted == true || status==StatusVichislenia.Stop)
            {
                if (GetListCompanies())
                    result = _model.GetParallelCompanis(companies,
                                            token, cancelTokenSource);
            }
            else
            {
                if (companies != null && companies.Count > 0)
                {
                    result = _model.GetParallelCompanis(companies.GetRange((int)result.LowestBreakIteration, companies.Count - (int)result.LowestBreakIteration),
                        token, cancelTokenSource);
                }
            }

        }

        public void Pause()
        {
            _model.Pririvanie(cancelTokenSource);
            status = StatusVichislenia.Pause;
        }


        public void Stop()
        {
            _model.Pririvanie(cancelTokenSource);
            status = StatusVichislenia.Stop;
        }

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
