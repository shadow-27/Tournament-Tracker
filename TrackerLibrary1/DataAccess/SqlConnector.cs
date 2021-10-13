using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary1;


//@PlaceNumber int,
//@PrizeAmount money,
//@PrizePercentage float,
//@PlaceName nvarchar(50) ,
//@id int=0 output



namespace TrackerLibrary.DataAccess
{
   
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        public void CreatePerson(PersonModel p)
        {
            using (IDbConnection connection=new SqlConnection(GlobalConfig.CnnString(db)))
            {
                var par = new DynamicParameters();
                par.Add("@FirstName", p.FirstName);
                par.Add("@LastName", p.LastName);
                par.Add("@Email", p.Email);
                par.Add("@CellphoneNumber", p.CellphoneNumber);
                par.Add("@id",dbType:DbType.Int32,direction:ParameterDirection.Output);
                connection.Execute("dbo.spPeople_Insert", par, commandType: CommandType.StoredProcedure);
                p.Id = par.Get<int>("@id");
                //return p;
            }
            



        }

        //TODO-Make the Create
        //Prize method and actually save the data in the database
        /// <summary>
        /// saves a new prize in the database
        /// </summary>
        /// <param name="m">The prize information</param>
        /// <returns>The prize information,including the unique id</returns>
        public void CreatePrize(PrizeModel m)
        {
            using(IDbConnection connection=new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", m.PlaceNumber);
                p.Add("@PrizeAmount", m.PrizeAmount);
                p.Add("@PrizePercentage", m.Prizepercentage);
                p.Add("@PlaceName", m.PlaceName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                m.id = p.Get<int>("@id");
               // return m;
            }
        }


        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
                return output;
            }
        }

        public void CreateTeam(TeamModel model)
        {
         
            using (IDbConnection connection=new SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName",model.TeamName);
                p.Add("@id",0,DbType.Int32,ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("@id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId",model.Id);
                    p.Add("@PersonId",tm.Id);
                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);

                }

                //return model;
            }
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection=new SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll", commandType: CommandType.StoredProcedure).ToList();
                foreach (var team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId",team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam",p,
                        commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection=new SqlConnection(GlobalConfig.CnnString(db)))
            {
               SaveTournament(connection,model);
                SaveTournamentPrizes(connection,model);
                SaveTournamentEnteredTeams(connection,model);
                SaveTournamentRounds(connection, model);
                TournamentLogic.UpdateTournamentResults(model);

            }


        }


        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, ParameterDirection.Output);
            connection.Execute("[dbo].[spTournaments_Insert]", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");
        }

        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pm in model.Prizes)
            {
               var  p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pm.id);
                p.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
                connection.Execute("[dbo].[spTournamentPrizes_Insert]", p,
                    commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEnteredTeams(IDbConnection connection, TournamentModel model)
        {
            foreach (var tm in model.EnteredTeams)
            {
              var  p = new DynamicParameters();
                p.Add("@TournamentID", model.Id);
                p.Add("@TeamID", tm.Id);
                p.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
                connection.Execute("[dbo].[spTournamentEntries_Insert]", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            // List<List<MatchUpModel>> Rounds
            //List<MathchupEntryModel> Entries

            //Loop through the rounds
            //Loop through matchups
            //Save the matchup
            //Loop through the entries and save them

            foreach (var round  in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId",model.Id);
                    p.Add("@MatchupRound",matchup.MatchupRound);
                    p.Add("@id",0,DbType.Int32,ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);
                    matchup.Id = p.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId",matchup.Id);
                        if (entry.ParentMatchup != null)
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", null);
                        }

                        if (entry.Teamcompeting != null)
                        {
                            p.Add("TeamCompetingId", entry.Teamcompeting.Id);
                        }
                        else
                        {
                            p.Add("TeamCompetingId", null);
                        }
                        
                        p.Add("@id",0,DbType.Int32,ParameterDirection.Output);

                        connection.Execute("dbo.MatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);

                    }

                }
            }

        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll", commandType: CommandType.StoredProcedure).ToList();
                foreach (var tournament in output)
                {
                    var t = new DynamicParameters();
                    t.Add("@TournamentId",tournament.Id);
                    //populate prizes
                    tournament.Prizes = connection.Query<PrizeModel>("[dbo].[spPrizes_GetByTournament]",t,
                        commandType: CommandType.StoredProcedure).ToList();

                    //populate teams

                    tournament.EnteredTeams = connection.Query<TeamModel>("[dbo].[spTeam_GetByTournament]",t, commandType: CommandType.StoredProcedure).ToList();
                    foreach (var team in tournament.EnteredTeams)
                    {
                        var p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p,
                            commandType: CommandType.StoredProcedure).ToList();
                    }

                    //populate rounds
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", t,
                        commandType: CommandType.StoredProcedure).ToList();
                    foreach (var m in matchups)
                    {
                        var p = new DynamicParameters();
                        p.Add("@MatchupId",m.Id);
                        m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchUpEntries_GetByMatchUp", p,
                            commandType: CommandType.StoredProcedure).ToList();
                        
                        //populate each entry(2 models)
                        //populate each matchup(1 model)
                        List<TeamModel> allTeams = GetTeam_All();
                        if (m.WinnerId > 0)
                        {
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }

                        foreach (var me in m.Entries)
                        {
                            if (me.TeamcompetingId > 0)
                            {
                                me.Teamcompeting = allTeams.Where(x => x.Id == me.TeamcompetingId).First();
                            }

                            if (me.ParentMatchupId > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }

                        }
                    }

                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;

                    foreach (var m in matchups)
                    {
                        if (m.MatchupRound > currRound)
                        {
                            tournament.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound++;
                        }
                        currRow.Add(m);
                    }
                    tournament.Rounds.Add(currRow);

                }




            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            // spMatchups_Update @id,@WinnerId
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();

                if (model.Winner!=null)
                {
                    p.Add("@id", model.Id);
                    p.Add("@WinnerId", model.Winner.Id);

                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure); 
                }
                //spMatchupEntries_Update @id,@TeamCompetingId,@Score
                foreach (var me in model.Entries)
                {
                    if (me.Teamcompeting!=null)
                    {
                        p = new DynamicParameters();
                        p.Add("@id", me.Id);
                        p.Add("@TeamCompetingId", me.Teamcompeting.Id);
                        p.Add("@Score", me.Score);

                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }
            }

        }

    }
}
