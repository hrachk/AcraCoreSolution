using AcraUtils;
using AcraUtils.Configuration;
using System.Threading;
using AcraUtils.Services;
using Microsoft.EntityFrameworkCore;
using AcraData.Data;
using System;
using AcraIDServices;

namespace AcraIDWebService
{
    public class AcraGroupService : AcraService
    {
       
        private DbContextOptions<Acra3DbContext> _acra3dbContextOptions;        


        public AcraGroupService(DbContextOptions<Acra3DbContext> acra3DbContextOptions, Logger logger) : base(logger)
        {
            _acra3dbContextOptions = acra3DbContextOptions;            

            
        }

        protected override void process()
        {
            try
            {
                _logger.Log.Info("AcraGroup Service has been started");

                while (true)
                {
                    CheckCancel();

                    processItems();

                    CheckCancel();

                    Thread.Sleep(10000);
                }
            }
            catch (AggregateException aggregateEx)
            {
                _logger.Log.Info("Task Cancelled");
                aggregateEx.Handle(cancelEx => true);
            }
            catch (Exception ex)
            {
                _logger.Log.FatalFormat("AcraGroup RefreshPersons Service process(): ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
            }

            base.process();
        }

        protected void processItems()
        {
            try
            {
                ;//Model.Ac(); 
            }
            catch (Exception ex)
            {
                _logger.Log.ErrorFormat("processItems: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
            }
        }
    }
}
