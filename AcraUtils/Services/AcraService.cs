using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AcraUtils;

namespace AcraUtils.Services
{
    public abstract class AcraService : IAcraService
    {
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private Task processor;
        protected Logger _logger;

        public AcraService(Logger logger)
        {
            _logger = logger;
        }

        virtual protected void process()
        {
            processor = null;
        }

        public bool IsStarted
        {
            get
            {
                return processor != null;
            }
        }

        virtual public void Start()
        {
            if (processor == null)
            {
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;

                processor = new Task(process, token);

                processor.Start();
            }
        }        

        virtual public void Stop()
        {
            if (processor != null)
            {
                tokenSource.Cancel();
                try
                {
                    processor.Wait();
                }
                catch (AggregateException aggregateEx)
                {
                    _logger.Log.Info("Task Cancelled");
                    aggregateEx.Handle(cancelEx => true);
                }
                finally
                {
                    processor = null;
                }
            }
            _logger.Log.Info("Service has been stoped");
        }

        public void Wait()
        {
            try
            {
                if (processor != null)
                {
                    processor.Wait();
                }
            }
            catch (AggregateException aggregateEx)
            {
                _logger.Log.Info("Task Cancelled.");
                aggregateEx.Handle(cancelEx => true);
            }
        }

        public void Wait(int to = 0)
        {
            try
            {
                if (processor != null)
                {
                    processor.Wait(to);
                }
            }
            catch (AggregateException aggregateEx)
            {
                _logger.Log.Info("Task Cancelled.");
                aggregateEx.Handle(cancelEx => true);
            }
        }
        protected void CheckCancel()
        {
            token.ThrowIfCancellationRequested();
        }

        
    }
}
