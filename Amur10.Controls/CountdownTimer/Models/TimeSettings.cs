using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amur10.Controls.CountdownTimer.Models
{
    public class TimeSettings
    {
        public DateTime StartTime { get; set; }
        public DateTime UnloadedTime { get; set; }
        public TimeSpan UnloadedPausedTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsPaused { get; set; } = false;

        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
    }
}
