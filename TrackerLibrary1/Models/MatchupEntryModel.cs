using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {

        public int Id { get; set; }

        /// <summary>
        /// A unique identifier for a team
        /// </summary>
        public int TeamcompetingId { get; set; }

        /// <summary>
        /// Represents one team in the matchup
        /// </summary>
        public TeamModel Teamcompeting { get; set; }

        /// <summary>
        /// Reprsents the score for this particular team
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Represents the matchup that this team came 
        /// from as the winner
        /// </summary>
        public MatchupModel ParentMatchup { get; set; } = new MatchupModel();

        /// <summary>
        /// A unique identifier for parent matchup
        /// </summary>
        public int ParentMatchupId { get; set; }


    }
}
