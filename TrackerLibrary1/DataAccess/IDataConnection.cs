using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public  interface IDataConnection
    {
        void CreatePrize(PrizeModel m);
        void CreatePerson(PersonModel p);
        List<PersonModel> GetPerson_All();
        void CreateTeam(TeamModel model);
        List<TeamModel> GetTeam_All();
        void CreateTournament(TournamentModel model);
        void UpdateMatchup(MatchupModel model);
        List<TournamentModel> GetTournament_All();
    }
}
