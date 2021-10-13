using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
   
    public class SqlConnector : IDataConnection
    {
        //TODO-Make the CreateaPrize method and actually save the data in the database
        /// <summary>
        /// saves a new prize in the database
        /// </summary>
        /// <param name="m">The prize information</param>
        /// <returns>The prize information,including the unique id</returns>
        public PrizeModel CreatePrize(PrizeModel m)
        {
            
        }
    }
}
