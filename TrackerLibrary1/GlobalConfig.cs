using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public static IDataConnection Connection { get; private set; }
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModel.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchupFile = "MatchupModel.csv";
        public const string MatchupEntryFile = "MatchupEntryModel.csv";

        public static void InializeConnections(DataBaseType connectionType)
        {
         

            if(connectionType==DataBaseType.Sql)
            {
                //TODO-Setup sql connection properly
                SqlConnector sq1 = new SqlConnector();
                Connection=sq1;
            }

            else if(connectionType == DataBaseType.Textfile)
            {
                //TODO-create text connection
                TextConnector text = new TextConnector();
                Connection=text;
            }

           
        }
        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }


    }
}
