using AcraData.Models.Acra4;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AcraData.Data;

namespace AcraIDServices
{
    public class DataHelpers
    {
        private DbContextOptions<AcraData.Data.Acra4DbContext> _acra4DbContextOptions;
        private AcraUtils.Logger _logger;

        public DataHelpers(DbContextOptions<AcraData.Data.Acra4DbContext> acra4DbContextOptions, AcraUtils.Logger logger)
        {
            _acra4DbContextOptions = acra4DbContextOptions;
            _logger = logger;
        }

        public BPR_Persons SaveAvvPerson(BPR_Persons person)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbContextOptions))
            {
                person.AVVGetDate = DateTime.Now;                
                context.AddOrUpdate(person);                
                context.SaveChanges();
                return person;
            }
        }
    }
}
