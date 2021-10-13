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

namespace WindowsFormsApp1
{
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private ITeamRequest callingform;

        public CreateTeamForm(ITeamRequest caller)
        {
            InitializeComponent();

            callingform = caller;

            WireUpLists(); 
            
        }

       

        private void WireUpLists()
        {
            selectTeamMemberDropdown.DataSource = null;
           selectTeamMemberDropdown.DataSource = availableTeamMembers;
            selectTeamMemberDropdown.DisplayMember = "FullName";


            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
            
        }
        private void selectTeamMemberLabel_Click(object sender, EventArgs e)
        {

        }

        private void lastNameValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void firstNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void LastNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();
                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.Email = email.Text;
                p.CellphoneNumber = cellphoneValue.Text;
                GlobalConfig.Connection.CreatePerson(p);

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                email.Text = "";
                cellphoneValue.Text = "";
                availableTeamMembers.Add(p);
                WireUpLists();



            }
            else
            {
                MessageBox.Show("You need to fill in all the fields");
            }
        }

        private bool ValidateForm()
        {
          
           

            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }
            if (emailValue.Text.Length == 0)
            {
                return false;
            }

            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p =(PersonModel)selectTeamMemberDropdown.SelectedItem;
            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
                WireUpLists();

            }
        }

        private void removeselectedTeamMembersButton_Click(object sender, EventArgs e)
        {
            PersonModel p =(PersonModel)teamMembersListBox.SelectedItem;

            if (p!=null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);
                WireUpLists();

            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {

            if (teamNameValue.Text.Length == 0)
            {
                MessageBox.Show("You need to fill in team name to create a team");
            }
            else
            {
                TeamModel team = new TeamModel();
                team.TeamName = teamNameValue.Text;
                team.TeamMembers = selectedTeamMembers;

                GlobalConfig.Connection.CreateTeam(team);

                callingform.TeamComplete(team);

                this.Close();
            }
           
        }
    }
}
