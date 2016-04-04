using Amur10.Controls.CountdownTimer.Animations;
using Amur10.Controls.CountdownTimer.Helpers;
using Amur10.Controls.CountdownTimer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Amur10.Controls.CountdownTimer
{
    [TemplatePart(Name = "OpenCloseSettingsButton", Type = typeof(AppBarButton))]
    [TemplatePart(Name = "SettingsGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "HoursButton", Type = typeof(Button))]
    [TemplatePart(Name = "MinutesButton", Type = typeof(Button))]
    [TemplatePart(Name = "SecondsButton", Type = typeof(Button))]
    [TemplatePart(Name = "StartTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "PauseTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "ClearTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "HoursPanel", Type = typeof(NumbersPanel))]
    [TemplatePart(Name = "HoursFlyout", Type = typeof(Flyout))]
    [TemplatePart(Name = "MinutesFlyout", Type = typeof(Flyout))]
    [TemplatePart(Name = "SecondsFlyout", Type = typeof(Flyout))]
    public sealed class MiniCountdownTimer : BaseCountdownTimer
    {
        private OpenCloseSettingsAnimation openCloseAnimation = new OpenCloseSettingsAnimation();
        private Storyboard openSettingsSb;
        private Storyboard closeSettingsSb;

        public MiniCountdownTimer()
        {
            this.DefaultStyleKey = typeof(MiniCountdownTimer);

            Loaded += (sender, e) =>
            {
                if (timeSettings == null)
                {
                    //Set defaults as fresh initialisation
                    this.Hours = this.HoursPanel.Selected;
                    this.Minutes = this.MinutesPanel.Selected;
                    this.Seconds = this.SecondsPanel.Selected;
                }

                //Set the range of each number panel (hours, minutes, seconds)
                this.HoursPanel.Items = SetRange(HoursMinimum, HoursMaximum);
                this.MinutesPanel.Items = SetRange(MinutesMinimum, MinutesMaximum);
                this.SecondsPanel.Items = SetRange(SecondsMinimum, SecondsMaximum);

                this.OpenCloseSettingsButton.Click += (s, args) =>
                {
                    if (SettingsOpen)
                    {
                        if (closeSettingsSb == null)
                            closeSettingsSb = openCloseAnimation.GetOpenCloseSettingsStoryboard(this.SettingsGrid, 500, 0);

                        closeSettingsSb.Begin();
                        SettingsOpen = false;
                    }
                    else
                    {
                        if (openSettingsSb == null)
                            openSettingsSb = openCloseAnimation.GetOpenCloseSettingsStoryboard(this.SettingsGrid, 500, this.SettingsGrid.Width);

                        openSettingsSb.Begin();
                        SettingsOpen = true;
                    }
                };

                #region Timer Start, pause, clear events
                this.StartTimerButton.Click += (s, args) =>
                {
                    var timeNow = DateTime.UtcNow;

                    if (_timer == null)
                    {
                        SetupTimer();
                    }

                    _timer.Start();

                    if (!Init)
                    {
                        _startTime = timeNow;
                        _endTime = timeNow + new TimeSpan(this.Hours, this.Minutes, this.Seconds);

                        this.Init = true;
                        if (timeSettings == null)
                        {
                            timeSettings = new TimeSettings() { StartTime = _startTime, EndTime = _endTime };
                        }
                        SaveTimeSettings(controlName, timeSettings);
                    }

                    if (this.IsPaused)
                    {
                        var extraTime = new TimeSpan(this.Hours, this.Minutes, this.Seconds);
                        _endTime = timeNow.Add(extraTime);

                        timeSettings.IsPaused = false;
                        timeSettings.EndTime = _endTime;
                        SaveTimeSettings(controlName, timeSettings);
                    }

                    this.IsRunning = true;
                    this.IsPaused = false;

                    var timerArgs = new CountdownTimerEventArgs
                    {
                        StartTime = _startTime
                    };

                    OnTimerStarted(this, timerArgs);
                };

                this.PauseTimerButton.Click += (s, args) =>
                {
                    _pausedTime = DateTime.UtcNow;
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    this.IsRunning = false;
                    this.IsPaused = true;

                    if (timeSettings != null)
                    {
                        timeSettings.EndTime = _endTime;
                        timeSettings.IsPaused = this.IsPaused;

                        timeSettings.Hours = this.Hours;
                        timeSettings.Minutes = this.Minutes;
                        timeSettings.Seconds = this.Seconds;
                    }

                    SaveTimeSettings(controlName, timeSettings);

                    var timerArgs = new CountdownTimerEventArgs
                    {
                        StartTime = _startTime,
                        PauseTime = _pausedTime
                    };
                    OnTimerPaused(this, timerArgs);
                };

                this.ClearTimerButton.Click += (s, args) =>
                {
                    _timer.Stop();
                    this.IsRunning = this.IsPaused = false;
                    this.Init = false; //reinitialise so we start a new timer

                    this.Hours = this.Minutes = this.Seconds = 0;
                    this.HoursPanel.Selected = this.MinutesPanel.Selected =
                        this.SecondsPanel.Selected = 0;

                    ClearTimeSettings(controlName);
                };
                #endregion

                #region Hour, Minute, Second selection changed events               

                this.HoursPanel.SelectedNumberChanged += (s, args) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(args.NewNumber, 0, 0);
                        timeSettings.EndTime = _endTime = RefreshEndTime(args.NewNumber, args.OldNumber, ts);

                        this.SaveTimeSettings(controlName, timeSettings);
                    }

                    this.Hours = args.NewNumber;
                    this.HoursFlyout.Hide();
                };

                this.MinutesPanel.SelectedNumberChanged += (s, args) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(0, args.NewNumber, 0);
                        timeSettings.EndTime = _endTime = RefreshEndTime(args.NewNumber, args.OldNumber, ts);

                        SaveTimeSettings(controlName, timeSettings);
                    }
                    this.Minutes = args.NewNumber;
                    this.MinutesFlyout.Hide();
                };

                this.SecondsPanel.SelectedNumberChanged += (s, args) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(0, 0, args.NewNumber);
                        timeSettings.EndTime = _endTime = RefreshEndTime(args.NewNumber, args.OldNumber, ts);

                        SaveTimeSettings(controlName, timeSettings);
                    }
                    this.Seconds = args.NewNumber;
                    this.SecondsFlyout.Hide();
                };
                #endregion
            };

            Unloaded += (sender, e) =>
            {
                this.OpenCloseSettingsButton.Click -= (s, args) => { };
                this.HoursPanel.SelectedNumberChanged -= (s, args) => { };
                this.MinutesPanel.SelectedNumberChanged -= (s, args) => { };
                this.SecondsPanel.SelectedNumberChanged -= (s, args) => { };
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            controlName = this.Name == string.Empty ? "timer" : this.Name;
            this.OpenCloseSettingsButton = this.GetTemplateChild("OpenCloseSettingsButton") as AppBarButton;
            this.SettingsGrid = this.GetTemplateChild("SettingsGrid") as Grid;

            this.HoursButton = this.GetTemplateChild("HoursButton") as Button;
            this.MinutesButton = this.GetTemplateChild("MinutesButton") as Button;
            this.SecondsButton = this.GetTemplateChild("SecondsButton") as Button;

            this.StartTimerButton = this.GetTemplateChild("StartTimerButton") as AppBarButton;
            this.PauseTimerButton = this.GetTemplateChild("PauseTimerButton") as AppBarButton;
            this.ClearTimerButton = this.GetTemplateChild("ClearTimerButton") as AppBarButton;

            this.HoursPanel = this.GetTemplateChild("HoursPanel") as NumbersPanel;
            this.MinutesPanel = this.GetTemplateChild("MinutesPanel") as NumbersPanel;
            this.SecondsPanel = this.GetTemplateChild("SecondsPanel") as NumbersPanel;

            this.HoursFlyout = this.GetTemplateChild("HoursFlyout") as Flyout;
            this.MinutesFlyout = this.GetTemplateChild("MinutesFlyout") as Flyout;
            this.SecondsFlyout = this.GetTemplateChild("SecondsFlyout") as Flyout;

            if (DisplayLabels)
            {
                this.StartTimerButton.Label = "play";
                this.PauseTimerButton.Label = "pause";
                this.ClearTimerButton.Label = "clear";
                this.OpenCloseSettingsButton.Label = "settings";
            }
        }

        #region invoke base Timer events
        protected override void OnTimerStarted(object sender, CountdownTimerEventArgs e)
        {
            base.OnTimerStarted(sender, e);
        }

        protected override void OnTimerPaused(object sender, CountdownTimerEventArgs e)
        {
            base.OnTimerPaused(sender, e);
        }

        protected override void OnTimerEnded(object sender, CountdownTimerEventArgs e)
        {
            base.OnTimerEnded(sender, e);
        }
        #endregion

        #region Template Properties

        public AppBarButton OpenCloseSettingsButton { get; set; }
        public Grid SettingsGrid { get; set; }
        public Button HoursButton { get; private set; }
        public Button MinutesButton { get; private set; }
        public Button SecondsButton { get; private set; }
        public AppBarButton StartTimerButton { get; set; }
        public AppBarButton PauseTimerButton { get; set; }
        public AppBarButton ClearTimerButton { get; set; }
        public NumbersPanel HoursPanel { get; private set; }
        public NumbersPanel MinutesPanel { get; private set; }
        public NumbersPanel SecondsPanel { get; private set; }
        public Flyout HoursFlyout { get; set; }
        public Flyout MinutesFlyout { get; set; }
        public Flyout SecondsFlyout { get; set; }

        #endregion

        #region Dependency Properties

        public Brush SettingsBackground
        {
            get { return (Brush)GetValue(SettingsBackgroundProperty); }
            set { SetValue(SettingsBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingsBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsBackgroundProperty =
            DependencyProperty.Register("SettingsBackground",
                                        typeof(Brush),
                                        typeof(MiniCountdownTimer),
                                        new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public bool SettingsOpen
        {
            get { return (bool)GetValue(SettingsOpenProperty); }
            set { SetValue(SettingsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsOpenProperty =
            DependencyProperty.Register("SettingsOpen", typeof(bool), typeof(MiniCountdownTimer), new PropertyMetadata(false));

        public double NumbersFlyoutWidth
        {
            get { return (double)GetValue(NumbersFlyoutWidthProperty); }
            set { SetValue(NumbersFlyoutWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersFlyoutWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumbersFlyoutWidthProperty =
            DependencyProperty.Register("NumbersFlyoutWidth", typeof(double), typeof(MiniCountdownTimer), new PropertyMetadata(400));

        public double NumbersFlyoutHeight
        {
            get { return (double)GetValue(NumbersFlyoutHeightProperty); }
            set { SetValue(NumbersFlyoutHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersFlyoutHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumbersFlyoutHeightProperty =
            DependencyProperty.Register("NumbersFlyoutHeight", typeof(double), typeof(MiniCountdownTimer), new PropertyMetadata(120));

        #endregion
    }
}
