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
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace WindowsFormsApp1
{
    public partial class CreatePrizeForm : Form
    {
        private IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        private void teamNameValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void teamNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text,
                    placeNameValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);
                callingForm.PrizeComplete(model);
                this.Close();

               /* placeNumberValue.Text = "";
                placeNameValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";*/



            }
            else
            {
                MessageBox.Show("The form has invalid information,please check the information and try again");
            }
        }

        private bool ValidateForm()
        {
            bool output = true;

            int placeNumber = 0;
          bool placeNumberValidNumber  = int.TryParse(placeNumberValue.Text, out placeNumber);
            if(!placeNumberValidNumber)
            {
                output = false;
                
            }
            if(placeNumber<0)
            {
                output = false;
                
            }
            if (placeNameValue.Text.Length<0)
            {
                output = false;

            }
            decimal prizeAmount = 0;
            int prizePercentage = 0;
            bool prizeAmountValidAmount = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValidPercentage = int.TryParse(prizePercentageValue.Text, out prizePercentage);
            if(!prizeAmountValidAmount||!prizePercentageValidPercentage)
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <=0)
            {
                output = false;
            }

            if (prizePercentage<0||prizePercentage>100)
            {
                output = false;
            }


            return output;

        }

        private void prizeAmountValue_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
