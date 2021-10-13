using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //intialize the database connections
            TrackerLibrary.GlobalConfig.InializeConnections(DataBaseType.Sql);
            Application.Run(new TournamentDashboardForm());

            // Application.Run(new TournamentDashboardForm());
        }
    }
}
