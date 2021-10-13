using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>();

        public static void InializeConnections(bool database,bool textfiles)
        {
            if(database)
            {
                //TODO-Setup sql connection properly
                SqlConnector sq1 = new SqlConnector();
                Connections.Add(sq1);
            }

            if(textfiles)
            {
                //TODO-create text connection
                TextConnector text = new TextConnector();
                Connections.Add(text);
            }

            public static string CnnString(string name)
            {
              return  ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
        }


    }
}
