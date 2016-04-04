using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amur10.Controls.CountdownTimer.Helpers
{
    public class SelectNumberChangedEventArgs
    {
        public int NewNumber { get; set; }
        public int OldNumber { get; set; }

        public SelectNumberChangedEventArgs(int newNumber, int oldNumber)
        {
            this.NewNumber = newNumber;
            this.OldNumber = oldNumber;
        }
    }
}
