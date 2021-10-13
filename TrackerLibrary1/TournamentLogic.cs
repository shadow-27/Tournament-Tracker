using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerLibrary1
{
    public static  class TournamentLogic
    {
        //TODO:-Creating Tournament logic
        //Order our list randomly of teams
        //check if it is big enough-if not,add in byes-Number of teams=2^n teams
        //create our first round of matchups
        //create every round after that - 8 teams -4 matchups -2 matchups - 1 matchups
        public static void CreateRounds(TournamentModel tm)
        {
            List<TeamModel> randomizedTeams = Shuffle(tm.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds,randomizedTeams.Count);
            tm.Rounds.Add(CreateFirstRound(byes,randomizedTeams));
            CreateOtherRounds(tm,rounds);
            
        }

        public static void UpdateTournamentResults(TournamentModel model)
        {

            List<MatchupModel> toScore = new List<MatchupModel>();
            foreach (var round in model.Rounds)
            {
                foreach (var rm in round)
                {
                    if (rm.Winner==null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1))
                    {
                        toScore.Add(rm);
                    }
                }
            }

            MarkWinnerInMatchups(toScore);
            AdvanceWinners(toScore,model);
            toScore.ForEach(x=>GlobalConfig.Connection.UpdateMatchup(x));

            
        }

        private static void MarkWinnerInMatchups(List<MatchupModel> model)
        {
            //greater or lesser
            string scoreDirection = ConfigurationManager.AppSettings["greaterWins"];

            foreach(MatchupModel m in model)
            {
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].Teamcompeting;
                    continue;
                }
                //0 means false,or low score wins
                if (scoreDirection == "0")
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].Teamcompeting;
                    }
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].Teamcompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in the application");
                    }
                }
                else
                {
                    //1 means true,or high score wins
                    if (scoreDirection == "0")
                    {
                        if (m.Entries[0].Score > m.Entries[1].Score)
                        {
                            m.Winner = m.Entries[0].Teamcompeting;
                        }
                        else if (m.Entries[1].Score > m.Entries[0].Score)
                        {
                            m.Winner = m.Entries[1].Teamcompeting;
                        }
                        else
                        {
                            throw new Exception("We do not allow ties in the application");
                        }

                    }
                }
            }

        }

        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (var m in models)
            {
                foreach (var round in tournament.Rounds)
                {
                    foreach (var rm in round)
                    {
                        foreach (var me in rm.Entries)
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.Teamcompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void CreateOtherRounds(TournamentModel model,int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round<=rounds)
            {
                foreach (var match in previousRound)
                {
                  currMatchup.Entries.Add(new MatchupEntryModel {ParentMatchup = match});
                  if (currMatchup.Entries.Count > 1)
                  {
                      currMatchup.MatchupRound = round;
                      currentRound.Add(currMatchup);
                      currMatchup = new MatchupModel();
                  }
                }
                model.Rounds.Add(currentRound);
                previousRound = currentRound;
                currentRound = new List<MatchupModel>();
                round++;
            }
        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();
           

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel {Teamcompeting = team});

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();
                    if (byes > 0)
                    {
                        byes--;
                    }
                }
            }

            return output;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output++;
                val *= 2;
            }

            return output;
        }


        private static int NumberOfByes(int rounds,int teamCount)
        {
            int countbyes = 0;
            int total_teams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                total_teams *= 2;
            }

            countbyes = total_teams - teamCount;

            return countbyes;

        }

        public static List<TeamModel> Shuffle(List<TeamModel> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
