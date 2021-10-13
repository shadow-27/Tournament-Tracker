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

namespace WindowsFormsApp1
{
    public partial class TournamentDashboardForm : Form
    {
        private List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournament_All();
        public TournamentDashboardForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void WireUpLists()
        {
            loadExistingTournamentDropdown.DataSource = tournaments;
            loadExistingTournamentDropdown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm form = new CreateTournamentForm();
            form.Show();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentModel tm = (TournamentModel)loadExistingTournamentDropdown.SelectedItem;
            TournamentViewerForm form = new TournamentViewerForm(tm);
            form.Show();

        }
    }
}
