﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerLibrary1;

namespace WindowsFormsApp1
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        private BindingList<int> rounds = new BindingList<int>();
        private BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();
        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();
            tournament = tournamentModel;
            WireUpLists();
            LoadFormData();
            LoadRounds();
        }

        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;
        }

      
        private void WireUpLists()
        {
            
            roundDropdown.DataSource = rounds;
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";

        }



        private void LoadRounds()
        {
            rounds.Clear();
            rounds.Add(1);
            int currRound = 1;
            foreach (var matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                }
            }
            LoadMatchups(1);
        }

        private void TournamentViewerForm_Load(object sender, EventArgs e)
        {

        }

        private void roundDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropdown.SelectedItem);
        }

        private void LoadMatchups(int round)
        {
            foreach (var matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound==round)
                {
                    selectedMatchups.Clear();
                    foreach (var m in matchups)
                    {
                        if (m.Winner == null || !unplayedOnlyCheckBox.Checked)
                        {
                            selectedMatchups.Add(m);
                        }
                    }
                }
            }
           
            if (selectedMatchups.Count > 0)
            {
                LoadMatchup(selectedMatchups.First());
            }
            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = selectedMatchups.Count > 0;

            teamOneName.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void LoadMatchup(MatchupModel m)
        {
            if (m != null)
            {


                for (int i = 0; i < m.Entries.Count; i++)
                {
                    if (i == 0)
                    {
                        if (m.Entries[0].Teamcompeting != null)
                        {
                            teamOneName.Text = m.Entries[0].Teamcompeting.TeamName;
                            teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                            teamTwoName.Text = "<bye>";
                            teamTwoScoreValue.Text = "0";
                        }
                        else
                        {
                            teamOneName.Text = "Not yet set";
                            teamOneScoreValue.Text = "";
                        }
                    }

                    if (i == 1)
                    {
                        if (m.Entries[1].Teamcompeting != null)
                        {
                            teamTwoName.Text = m.Entries[1].Teamcompeting.TeamName;
                            teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                        }
                        else
                        {
                            teamTwoName.Text = "Not yet set";
                            teamTwoScoreValue.Text = "";
                        }
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropdown.SelectedItem);
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;
            if (m != null)
            {

                for (int i = 0; i < m.Entries.Count; i++)
                {
                    if (i == 0)
                    {
                        if (m.Entries[0].Teamcompeting != null)
                        {
                           
                            bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);

                            if (scoreValid)
                            {
                                m.Entries[0].Score = teamOneScore;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1");
                            return;
                        }
                    }

                    if (i == 1)
                    {
                        if (m.Entries[1].Teamcompeting != null)
                        {
                            
                            bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

                            if (scoreValid)
                            {
                                m.Entries[1].Score = teamTwoScore;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1");
                            return;
                        }
                    }
                }
                
                TournamentLogic.UpdateTournamentResults(tournament);
                LoadMatchups((int)roundDropdown.SelectedItem);
                


            }
        }

        
    }
}