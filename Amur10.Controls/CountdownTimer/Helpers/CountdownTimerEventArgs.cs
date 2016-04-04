using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amur10.Controls.CountdownTimer.Helpers
{
    public class CountdownTimerEventArgs
    {
        public DateTime StartTime { get; set; }
        public DateTime PauseTime { get; set; }
        public DateTime EndTime { get; set; }

        public CountdownTimerEventArgs()
        {

        }

        public CountdownTimerEventArgs(DateTime startTime = default(DateTime),
                                       DateTime pauseTime = default(DateTime),
                                       DateTime endTime = default(DateTime))
        {
            this.StartTime = startTime;
            this.PauseTime = pauseTime;
            this.EndTime = endTime;
        }
    }
}
