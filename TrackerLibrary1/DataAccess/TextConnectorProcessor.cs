using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

//*Load the text file
//*Convert the text to List<PrizeModel>
//Find the max_ID
//Add the new record with the new ID(max+1)
//Convert the prizes to list<string>
//Save the list<string> to the text file

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string fullFilePath( this string fileName)//PrizeModels.csv
        {
            //C:data/TournamentTracker/PrizeModels.csv
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            if(!File.Exists(file))
            {
                return new List<string>();
            }
            else
            {
                return File.ReadAllLines(file).ToList();

            }
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();
            foreach(string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                if (p.PrizeAmount == 0)
                {
                    p.Prizepercentage = double.Parse(cols[4]);
                }
               
                output.Add(p);
            }
            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount}.{p.Prizepercentage}");
            }
            File.WriteAllLines(GlobalConfig.PrizesFile.fullFilePath(),lines);
        }


        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel person = new PersonModel();
                person.Id = int.Parse(cols[0]);
                person.FirstName = cols[1];
                person.LastName = cols[2];
                person.Email = cols[3];
                person.CellphoneNumber = cols[4];
                output.Add(person);
            }

            return output;
        }

        public static void SaveToPeopleFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();
            foreach (var p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.Email},{p.CellphoneNumber}");
            }
            File.WriteAllLines(GlobalConfig.PeopleFile.fullFilePath(),lines);
        }


        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            //id,team_name,list of ids separated by the pipe
            // 3,WUT3F,1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];
                string[] personIds = cols[2].Split('|');

                foreach (var id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x=>x.Id==int.Parse(id)).First());
                }
                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines,string teamFile,string peopleFile, string prizeFile)
        {
            //id,TournamentName,EntryFee,(id|id|id -Entered Teams),(id|id|id -Prizes),(Rounds -id^id^id|id^id^id|id^id^id)
            List<TournamentModel> output=new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamsFile.fullFilePath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.fullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> mathchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile().ConvertToMatchupModels();
            foreach (var line in lines)
            {
                TournamentModel tm = new TournamentModel();
                string[] cols = line.Split(',');
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);
                string[] TeamIds = cols[3].Split('|');
                foreach (var id in TeamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (cols[4].Length>0)
                {
                    string[] prizeIds = cols[4].Split('|');
                    foreach (var id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.id == int.Parse(id)).First());
                    } 
                }
                
                //capture round information
                string[] rounds = cols[5].Split('|');
               

                foreach (var round in rounds)
                {
                    string[] msText = round.Split('^');
                    List<MatchupModel> ms = new List<MatchupModel>();
                    foreach (var matchupModelTextId in msText)
                    {
                      ms.Add(mathchups.Where(x=>x.Id==int.Parse(matchupModelTextId)).First());   
                    }
                    tm.Rounds.Add(ms);
                }
                output.Add(tm);
            }

            return output;
        }

        public static void SaveToTeamFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (var m in models)
            {
              lines.Add($"{ m.Id },{ m.TeamName },{ ConvertPeopleListToString(m.TeamMembers) }");
            }

            File.WriteAllLines(GlobalConfig.TeamsFile.fullFilePath(),lines);

        }

        public static void SaveRoundsToFile(this TournamentModel model)
        {
            //loop through each round
            //loop through each matchup
            //get the id for new mathcup and save the record
            //loop through each entry,get the id and save it.
            foreach (var round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    //Load all of the matchups from file
                    //get the top id
                    //store the id
                    //save the matchup record
                    matchup.SaveMatchupToFile();

                }
            }

        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            //id=0, TeamCompeting=1 ,Score=2,ParentMatchup=3
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel me = new MatchupEntryModel();
                me.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    me.Teamcompeting = null;
                }
                else
                {
                    me.Teamcompeting = LookupTeamById(int.Parse(cols[1]));
                }
                me.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(int.Parse(cols[3]));
                }
                else
                {
                    me.ParentMatchup = null;
                }
               
                output.Add(me);
            }

            return output;
        }


        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries =
                GlobalConfig.MatchupEntryFile.fullFilePath().LoadFile();

            List<string> matchingEntries = new List<string>();

            foreach (var id in ids)
            {
                foreach (var entry in entries)
                {
                    string[] cols = entry.Split(',');
                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }

                }
            }

            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static TeamModel LookupTeamById(int id)
        {
           List<string> teams=GlobalConfig.TeamsFile.fullFilePath().LoadFile();
           foreach (var team in teams)
           {
               string[] cols = team.Split(',');
               if (cols[0] == id.ToString())
               {
                   List<string> matchingteams = new List<string>();
                   matchingteams.Add(team);
                   return matchingteams.ConvertToTeamModels().First();
               }
           }

           return null;
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile();
            foreach (var matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0]==id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;

        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            //id=0,entries=1(pipe delimited by id),winner=2,matchRound=3
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel p = new MatchupModel();
                p.Id = int.Parse(cols[0]);
                p.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                if (cols[2].Length == 0)
                {
                    p.Winner = null;
                }
                else
                {
                    p.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                p.MatchupRound = int.Parse(cols[3]);

                output.Add(p);

            }

            return output;
        }

        public static void SaveMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile().ConvertToMatchupModels();
            int currentId = 1;
            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile();
            }
            //save to file
            List<string> lines = new List<string>();
            // id=0,entries=1(pipe delimited by id),winner=2,matchupRound=3
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner= m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.fullFilePath(),lines);
        }


        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile().ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();
            foreach (var m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }

            matchups.Remove(oldMatchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntryToFile();
            }
            //save to file
            List<string> lines = new List<string>();
            // id=0,entries=1(pipe delimited by id),winner=2,matchupRound=3
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.fullFilePath(), lines);
        }
        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries =
                GlobalConfig.MatchupEntryFile.fullFilePath().LoadFile().ConvertToMatchupEntryModels();
            int currentId = 1;
            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);
            List<string> lines = new List<string>();

            // id=0,TeamCompeting=1,Score=2,ParentMatchup=3
            foreach (var e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }

                string teamcompeting = "";

                if (e.Teamcompeting != null)
                {
                    teamcompeting = e.Teamcompeting.Id.ToString();
                }

                lines.Add($"{e.Id},{teamcompeting},{e.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.fullFilePath(),lines);

        }

        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries =
                GlobalConfig.MatchupEntryFile.fullFilePath().LoadFile().ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel();
            foreach (var me in entries)
            {
                if (me.Id==entry.Id)
                {
                    oldEntry = me;
                }
            }

            entries.Remove(oldEntry);
            entries.Add(entry);
            List<string> lines = new List<string>();

            // id=0,TeamCompeting=1,Score=2,ParentMatchup=3
            foreach (var e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }

                string teamcompeting = "";

                if (e.Teamcompeting != null)
                {
                    teamcompeting = e.Teamcompeting.Id.ToString();
                }

                lines.Add($"{e.Id},{teamcompeting},{e.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.fullFilePath(), lines);

        }


        public static void SaveToTournamentFile(this List<TournamentModel> models)
        {
            List<string> lines = new List<string>();
            foreach (var tm in models)
            {
                lines.Add($"{tm.Id}," +
                          $"{tm.TournamentName}," +
                          $"{tm.EntryFee}," +
                          $"{ConvertTeamListToString(tm.EnteredTeams)}" +
                          $",{ConvertPrizesListToString(tm.Prizes)},{ConvertRoundListToString(tm.Rounds)}");
            }

            File.WriteAllLines(GlobalConfig.TournamentFile.fullFilePath(),lines);

        }

        public static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";
            if (rounds.Count == 0)
            {
                return output;
            }

            foreach (var t in rounds)
            {
                output += $"{ConvertMatchupListToString(t)}|";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static string ConvertMatchupListToString(List<MatchupModel> entries)
        {
            string output = "";
            if (entries.Count == 0)
            {
                return output;
            }

            foreach (var t in entries)
            {
                output += $"{t.Id}^";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";
            if (entries.Count == 0)
            {
                return output;
            }

            foreach (var t in entries)
            {
                output += $"{t.Id}|";
            }

            output=output.Substring(0, output.Length - 1);

            return output;
        }
        public static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";
            if (teams.Count == 0)
            {
                return output;
            }

            foreach (var t in teams)
            {
                output += $"{t.Id}|";
            }

            output=output.Substring(0, output.Length - 1);

            return output;
        }

        public static string ConvertPrizesListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return output;
            }
            foreach (var p in prizes)
            {
                output += $"{ p.id }|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }


        

        public static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return output;
            }
            foreach (var p in people)
            {
                output += $"{ p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }


        
    }
}
