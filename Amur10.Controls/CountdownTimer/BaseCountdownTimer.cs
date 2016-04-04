using Amur10.Controls.CountdownTimer.Helpers;
using Amur10.Controls.CountdownTimer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Amur10.Controls.CountdownTimer
{
    public class BaseCountdownTimer : Control
    {
        protected DispatcherTimer _timer;

        protected DateTime _startTime;
        protected DateTime _endTime;
        protected DateTime _pausedTime;

        #region Constructor
        public BaseCountdownTimer()
        {
            this.Loaded += (s, args) =>
            {
                //always setup the timer whether we're reloading the page or fresh copy
                if (_timer == null)
                    SetupTimer();

                //try and get time settings from already running timer
                timeSettings = GetTimeSettings(controlName);

                var reloadedTime = new DateTime();
                var timeLeft = new TimeSpan();

                if (timeSettings != null)
                {
                    if (timeSettings.IsPaused)
                    {
                        var time = new TimeSpan(timeSettings.Hours, timeSettings.Minutes, timeSettings.Seconds);
                        var endTime = DateTime.UtcNow.Add(time);
                        var startTime = timeSettings.StartTime;

                        this.Hours = timeSettings.Hours;
                        this.Minutes = timeSettings.Minutes;
                        this.Seconds = timeSettings.Seconds;

                        this.IsPaused = true;
                    }
                    else
                    {
                        reloadedTime = DateTime.UtcNow;
                        //End time stays the same but we need to work out new time left accounting for the time away 
                        timeLeft = timeSettings.EndTime.Subtract(reloadedTime);

                        this.Hours = timeLeft.Hours < 0 ? 0 : timeLeft.Hours;
                        this.Minutes = timeLeft.Minutes < 0 ? 0 : timeLeft.Minutes;
                        this.Seconds = timeLeft.Seconds < 0 ? 0 : timeLeft.Seconds;

                        _startTime = timeSettings.StartTime;
                        _endTime = timeSettings.EndTime;

                        //check if timer has ended whilst we were away
                        if (this.Hours == 0 && this.Minutes == 0 && this.Seconds == 0)
                        {
                            //TODO possible event for end of countdown?


                            //countdown has complete while we were away
                            ClearTimeSettings(controlName);
                        }
                        else
                        {
                            //restart the timer
                            this.IsRunning = true;
                            _timer.Start();
                        }
                    }
                }
            };

            this.Unloaded += (s, args) =>
            {
                //TDOD null check                
                if (timeSettings != null)
                {
                    timeSettings.UnloadedTime = DateTime.UtcNow;
                    timeSettings.EndTime = _endTime;
                    timeSettings.IsPaused = this.IsPaused;

                    if (this.IsPaused)
                    {
                        timeSettings.Hours = this.Hours;
                        timeSettings.Minutes = this.Minutes;
                        timeSettings.Seconds = this.Seconds;
                    }
                }

                SaveTimeSettings(controlName, timeSettings);
            };

        }

        #endregion

        #region Timer Events
        public event EventHandler<CountdownTimerEventArgs> TimerStarted;
        public event EventHandler<CountdownTimerEventArgs> TimerPaused;
        public event EventHandler<CountdownTimerEventArgs> TimerEnded;
        #endregion

        #region virtual Timer Start,Paused, End methods to be overridden in derived classes
        protected virtual void OnTimerStarted(object sender, CountdownTimerEventArgs e)
        {
            EventHandler<CountdownTimerEventArgs> eh = TimerStarted;
            if (eh != null)
            {
                TimerStarted(sender, e);
            }
        }

        protected virtual void OnTimerPaused(object sender, CountdownTimerEventArgs e)
        {
            EventHandler<CountdownTimerEventArgs> eh = TimerPaused;
            if (eh != null)
            {
                TimerPaused(sender, e);
            }
        }

        protected virtual void OnTimerEnded(object sender, CountdownTimerEventArgs e)
        {
            EventHandler<CountdownTimerEventArgs> eh = TimerEnded;
            if (eh != null)
            {
                TimerEnded(sender, e);
            }
        }
        #endregion

        #region Dependency properties

        public bool DisplayLabels
        {
            get { return (bool)GetValue(DisplayLabelsProperty); }
            set { SetValue(DisplayLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayLabelsProperty =
            DependencyProperty.Register("DisplayLabels", typeof(bool), typeof(BaseCountdownTimer), new PropertyMetadata(false));

        

        public Brush ItemButtonBackground
        {
            get { return (Brush)GetValue(ItemButtonBackgroundProperty); }
            set { SetValue(ItemButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersPanelButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonBackgroundProperty =
            DependencyProperty.Register("ItemButtonBackground",
                                        typeof(Brush),
                                        typeof(BaseCountdownTimer),
                                        new PropertyMetadata(new SolidColorBrush(Defaults.ITEM_BUTTON_BACKGROUND)));

        public double ItemButtonWidth
        {
            get { return (double)GetValue(ItemButtonWidthProperty); }
            set { SetValue(ItemButtonWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumberPanelButtonWidthHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonWidthProperty =
            DependencyProperty.Register("ItemButtonWidth", typeof(double), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.ITEM_BUTTON_WIDTH));

        public double ItemButtonHeight
        {
            get { return (double)GetValue(ItemButtonHeightProperty); }
            set { SetValue(ItemButtonHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersPanelButtonHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonHeightProperty =
            DependencyProperty.Register("ItemButtonHeight", typeof(double), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.ITEM_BUTTON_HEIGHT));

        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register("ItemFontSize", typeof(double), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.ITEM_BUTTON_FONTSIZE));

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Hours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register("Hours", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));

        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Seconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register("Seconds", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));

        public int HoursMaximum
        {
            get { return (int)GetValue(HoursMaximumProperty); }
            set { SetValue(HoursMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoursMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoursMaximumProperty =
            DependencyProperty.Register("HoursMax", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.HOURS_MAX));

        public int HoursMinimum
        {
            get { return (int)GetValue(HoursMinimumProperty); }
            set { SetValue(HoursMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoursMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoursMinimumProperty =
            DependencyProperty.Register("HoursMin", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));



        public int MinutesMaximum
        {
            get { return (int)GetValue(MinutesMaximumProperty); }
            set { SetValue(MinutesMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinutesMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinutesMaximumProperty =
            DependencyProperty.Register("MinutesMaximum", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.MINUTES_MAX));

        public int MinutesMinimum
        {
            get { return (int)GetValue(MinutesMinimumProperty); }
            set { SetValue(MinutesMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinutesMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinutesMinimumProperty =
            DependencyProperty.Register("MinutesMinimum", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));


        public int SecondsMaximum
        {
            get { return (int)GetValue(SecondsMaximumProperty); }
            set { SetValue(SecondsMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondsMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsMaximumProperty =
            DependencyProperty.Register("SecondsMaximum", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(Defaults.SECONDS_MAX));



        public int SecondsMinimum
        {
            get { return (int)GetValue(SecondsMinimumProperty); }
            set { SetValue(SecondsMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondsMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondsMinimumProperty =
            DependencyProperty.Register("SecondsMinimum", typeof(int), typeof(BaseCountdownTimer), new PropertyMetadata(0));

        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRunning.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(BaseCountdownTimer), new PropertyMetadata(false));


        public bool IsPaused
        {
            get { return (bool)GetValue(IsPausedProperty); }
            set { SetValue(IsPausedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPaused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPausedProperty =
            DependencyProperty.Register("IsPaused", typeof(bool), typeof(BaseCountdownTimer), new PropertyMetadata(false));

        #endregion

        public bool Init { get; set; } = false;
        protected TimeSettings timeSettings;
        protected ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        protected string controlName = Defaults.TIMER_NAME;

        protected Func<int, int, List<int>> SetRange = SetRangeOfNumbers;

        #region helper methods
        protected void SetupTimer()
        {
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            _timer.Tick += (sender, e) =>
            {
                if (this.Hours == 0 && this.Minutes == 0 && this.Seconds == 0)
                {
                    //Stop the timer
                    _timer.Stop();

                    this.IsRunning = this.IsPaused = false;

                    //TODO this isnt correct
                    _endTime = DateTime.UtcNow;
                    var timerArgs = new CountdownTimerEventArgs
                    {
                        StartTime = _startTime,
                        EndTime = _endTime
                    };
                    TimerEnded?.Invoke(this, timerArgs);

                    //TODO clear out timer in AppSettings
                    ClearTimeSettings(controlName);
                    return;
                }

                if (this.Seconds == 0 && this.Minutes > 0)
                {
                    this.Minutes--;
                    this.Seconds = 60;
                }

                if (this.Minutes == 0 && this.Hours > 0)
                {
                    this.Hours--;
                    this.Minutes = 59;
                    this.Seconds = 60;
                }
                this.Seconds--;
            };
        }
        private static List<int> SetRangeOfNumbers(int minValue, int maxValue)
        {
            var range = new List<int>();

            for (int i = minValue; i <= maxValue; i++)
                range.Add(i);

            return range;
        }
        #endregion

        #region Time Settings methods
        protected TimeSettings GetTimeSettings(string controlName)
        {
            //Get the json from app settings
            var json = localSettings.Values[controlName] != null ? localSettings.Values[controlName].ToString() : string.Empty;

            //Deserialize
            return JsonConvert.DeserializeObject<TimeSettings>(json);
        }

        protected void SaveTimeSettings(string controlName, TimeSettings timeSettings)
        {
            //serialize to json
            var serializedTimer = JsonConvert.SerializeObject(timeSettings);

            //save to app settings
            if (!string.IsNullOrEmpty(serializedTimer))
                localSettings.Values[controlName] = serializedTimer;
        }

        /// <summary>
        /// Clear settings from local storage
        /// </summary>
        /// <param name="controlName"></param>
        protected void ClearTimeSettings(string controlName)
        {
            if (localSettings.Values[controlName] != null)
                localSettings.Values[controlName] = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newNumber"></param>
        /// <param name="oldNumber"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected DateTime RefreshEndTime(int newNumber, int oldNumber, TimeSpan time)
        {
            return newNumber > oldNumber ? _endTime.Add(time) : _endTime.Subtract(time);
        }
        #endregion
    }
}
