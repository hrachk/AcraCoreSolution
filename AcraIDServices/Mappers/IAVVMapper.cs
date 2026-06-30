using System;
using System.Collections.Generic;
using System.Text;
using AcraData.Models.Acra4;

namespace AcraIDServices.Mappers
{
    interface IAVVMapper
    {
        BPR_Persons ImportPerson(Models.PDataModel pDataModel);
    }
}
