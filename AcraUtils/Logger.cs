using Easy.Logger.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AcraUtils
{
    public class Logger
    {
        private IEasyLogger _logger = null;
        private IEasyLogger security = null;

        public Logger(ILogService logService)
        {
            _logger = logService.GetLogger(this.GetType());
        }

        public IEasyLogger Log
        {
            get { return _logger; }
        }

        public IEasyLogger Security
        {
            get { return security; }
        }

        public void LogException(DbUpdateException e)
        {
            _logger.Error("Error: " + e.Message);
            foreach (var entry in e.Entries)
            {
                _logger.Error("Name " + entry.Entity.GetType().Name);
                _logger.Error("State " + entry.State);
            }

            throw e;
        }
    }
}