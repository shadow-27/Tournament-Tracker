using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        //TODO-Wireup the CreateaPrize method and actually save the data in the text file
        public PrizeModel CreatePrize(PrizeModel m)
        {
            m.id = 1;
            return m;
        }
    }
}
