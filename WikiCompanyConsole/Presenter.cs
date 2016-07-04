using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WikiCompanyConsole
{
    class Presenter
    {

        readonly IModel _model;
        readonly IView _view;

        List<string> companies;
        int schetcik;
        CancellationTokenSource cancelTokenSource;
        CancellationToken token;
        ParallelLoopResult result;

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
            _view.ViewMessage(e.Message);
        }

        public void GetListCompanies()
        {
           
            companies = _model.GetListCompanyToTxT();

            if (companies == null || companies.Count == 0)
            {
                _view.ViewMessage("Ошибка!!! Список компаний не загружен!");

            }
            else
            {
                _view.ViewMessage("Загружен список из " + companies.Count + " компаний");
            }

        }


        public long Pririvanie()
        {
            cancelTokenSource.Cancel();
            return (long)result.LowestBreakIteration;
        }


        public void Vichisl(List<string> companies)
        {
            try
            {
                result = Parallel.ForEach<string>(companies, new ParallelOptions { CancellationToken = token }, _model.Get);
             
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Операция прервана");
            }
            finally
            {
                cancelTokenSource.Dispose();
            }

        }

        public void Vichislenie()
        {
            List<string> companis = null;

            companis = _model.GetListCompanyToTxT();

            if (companis == null || companis.Count == 0)
            {
                _view.ViewMessage("Ошибка!!! Список компаний не загружен!");

            }
            else
            {
                _view.ViewMessage("Загружен список из " + companis.Count + " компаний");

                _model.GetCompanis(companis);
            }

        }


        public void VichislenieOne(string compani)
        {
            if (String.IsNullOrWhiteSpace(compani.Trim()))
                _model.GetCompany(compani);
            else
                _view.ViewMessage("Ошибка!!! Название компании не введено!");
        }
    }
}
