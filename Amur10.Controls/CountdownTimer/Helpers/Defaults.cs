using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Amur10.Controls.CountdownTimer.Helpers
{
    public static class Defaults
    {
        public const int HOURS_MAX = 12;
        public const int MINUTES_MAX = 60;
        public const int SECONDS_MAX = 60;

        public static Color TIME_BUTTON_BACKGROUND = Colors.Transparent;
        public static Color ITEM_BUTTON_BACKGROUND = Colors.Orange;
        public const double ITEM_BUTTON_HEIGHT = 50.0;
        public const double ITEM_BUTTON_WIDTH = 50.0;
        public const double ITEM_BUTTON_FONTSIZE = 20.0;

        public const string TIMER_NAME = "timer";

        #region MiniCountdownTimer defaults
        public const double MCT_SETTINGS_WIDTH = 200;
        public const double MCT_SETTINGS_HEIGHT = 60;
        #endregion
    }
}
