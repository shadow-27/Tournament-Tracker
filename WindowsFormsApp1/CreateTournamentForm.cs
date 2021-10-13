using System;
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
    public partial class CreateTournamentForm : Form,IPrizeRequester,ITeamRequest
    {
        private List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        private List<TeamModel> selectedTeams = new List<TeamModel>();
        private List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropdown.DataSource = null;
            selectTeamDropdown.DataSource = availableTeams;
            selectTeamDropdown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";


            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
          var tm= (TeamModel) selectTeamDropdown.SelectedItem;
            if (tm!=null)
            {

                selectedTeams.Add(tm);
                availableTeams.Remove(tm);
                WireUpLists();

            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            //call the CreatePrizeForm

            CreatePrizeForm form = new CreatePrizeForm(this);
            form.ShowDialog();


            

        }

        public void PrizeComplete(PrizeModel model)
        {
            
            //Take PrizeModel and put into our list of selectedprizes
            selectedPrizes.Add(model);
            WireUpLists();

        }

        private void createNewLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm teamForm = new CreateTeamForm(this);
            teamForm.ShowDialog();
        }

        public void TeamComplete(TeamModel model)
        {
           selectedTeams.Add(model);
           WireUpLists();
        }

        private void deleteselectedTeamsPlayersButton_Click(object sender, EventArgs e)
        {
            TeamModel tm = (TeamModel) tournamentTeamsListBox.SelectedItem;
            if (tm != null)
            {
                selectedTeams.Remove(tm);
                availableTeams.Add(tm);
                WireUpLists();
            }
            
        }

        private void deleteselectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel) prizesListBox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);
                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            decimal fee = 0;
            bool isValid = decimal.TryParse(entryfeeValue.Text, out fee);

            if (!isValid)
            {
                MessageBox.Show("You need to enter a valid entry fee", "Invalid fee", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            //Create Tournament model

            if (ValidateForm())
            {
                TournamentModel tm = new TournamentModel();
                tm.EntryFee = fee;
                tm.TournamentName = tournamentNameValue.Text;
                tm.EnteredTeams = selectedTeams;
                tm.Prizes = selectedPrizes;

                //Wireup our Matchups
                TournamentLogic.CreateRounds(tm);


                //Create Tournament Entry
                //Create all prizes entries
                //Create all of team entries

                GlobalConfig.Connection.CreateTournament(tm);
                TournamentViewerForm form = new TournamentViewerForm(tm);
                form.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("The form has invalid information,please check the information and try again");

            }
        }

        private bool ValidateForm()
        {
            bool output = true;

            if (tournamentNameValue.Text.Length < 0)
            {
                output = false;

            }

            if (tournamentTeamsListBox.Items.Count==0)
            {
                output = false;
            }
            

            return output;

        }
    }
}
