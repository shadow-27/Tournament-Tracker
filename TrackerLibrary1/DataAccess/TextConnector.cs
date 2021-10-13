using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Linq;
using TrackerLibrary1;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector :  IDataConnection
    {
        
        public void CreatePerson(PersonModel p)
        {
            //Load the text file
            //Covert the text to List<PersonModel>
            List<PersonModel> people = GlobalConfig.PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();

            //find max_ID
            int currentID = 1;
            if (people.Count > 0)
            {
                currentID = people.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }

            p.Id = currentID;

            //Add the new record with new Id(max+1)
            people.Add(p);

            //save the Personmodel to PeopleFile
            people.SaveToPeopleFile();

          

        }

        
        public void CreatePrize(PrizeModel m)
        {
            //Load the text file
            //Convert the text to List<PrizeModel>
            List<PrizeModel> prizes=GlobalConfig.PrizesFile.fullFilePath().LoadFile().ConvertToPrizeModels();
            
            //Find the max_ID
            int currentId = 0+1;

            if(prizes.Count()>0)
            {
                currentId = prizes.OrderByDescending(x => x.id).FirstOrDefault().id + 1;
            }
            m.id = currentId;
            //Add the new record with the new ID(max+1)
            prizes.Add(m);


            //Convert the prizes to list<string>
            //Save the list<string> to the text file
            prizes.SaveToPrizeFile();

            //return m;
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.fullFilePath().LoadFile().ConvertToTeamModels();

            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).FirstOrDefault().Id+1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile();

           // return model;

        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.fullFilePath().LoadFile()
                .ConvertToTournamentModels(GlobalConfig.TeamsFile,GlobalConfig.PeopleFile, GlobalConfig.PrizesFile);
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentFile();
            TournamentLogic.UpdateTournamentResults(model);



        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamsFile.fullFilePath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile.fullFilePath().LoadFile()
                .ConvertToTournamentModels(GlobalConfig.TeamsFile, GlobalConfig.PeopleFile, GlobalConfig.PrizesFile);
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
