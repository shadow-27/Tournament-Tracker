using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace WindowsFormsApp1
{
    public interface IPrizeRequester
    {
        void PrizeComplete(PrizeModel model);
    }
}
