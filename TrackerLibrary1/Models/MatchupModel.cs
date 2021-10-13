using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        public int Id { get; set; }

        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The id from the database that will be used to look up the winner
        /// </summary>
        public int WinnerId { get; set; }

        public int MatchupRound { get; set; }

        public string DisplayName
        {
            get
            {
                string output = "";
                foreach (var me in Entries)
                {
                    if (me.Teamcompeting!=null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.Teamcompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. {me.Teamcompeting.TeamName}";
                        } 
                    }
                    else
                    {
                        output = "Matchup Not yet determined";
                        break;
                    }
                }
                return output;
            }
        }
        

    }
}
