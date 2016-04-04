using Amur10.Controls.CountdownTimer.Helpers;
using Amur10.Controls.CountdownTimer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Amur10.Controls.CountdownTimer
{
    [TemplatePart(Name = "MainBorder", Type = typeof(Border))]
    [TemplatePart(Name = "HoursButton", Type = typeof(Button))]
    [TemplatePart(Name = "MinutesButton", Type = typeof(Button))]
    [TemplatePart(Name = "SecondsButton", Type = typeof(Button))]
    [TemplatePart(Name = "StartTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "PauseTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "ClearTimerButton", Type = typeof(Button))]
    [TemplatePart(Name = "HoursPanel", Type = typeof(NumbersPanel))]
    [TemplatePart(Name = "MinutesPanel", Type = typeof(NumbersPanel))]
    [TemplatePart(Name = "SecondsPanel", Type = typeof(NumbersPanel))]
    public sealed class CountdownTimer : BaseCountdownTimer
    {
        #region Dependency Properties       

        public SolidColorBrush TimeButtonBackground
        {
            get { return (SolidColorBrush)GetValue(TimeButtonBackgroundProperty); }
            set { SetValue(TimeButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeButtonBackgroundProperty =
            DependencyProperty.Register("TimeButtonBackground",
                                        typeof(SolidColorBrush),
                                        typeof(CountdownTimer),
                                        new PropertyMetadata(new SolidColorBrush(Defaults.TIME_BUTTON_BACKGROUND)));

        #endregion

        #region Template Properties
        public Border MainBorder { get; private set; }
        public Button HoursButton { get; private set; }
        public Button MinutesButton { get; private set; }
        public Button SecondsButton { get; private set; }
        public AppBarButton StartTimerButton { get; set; }
        public AppBarButton PauseTimerButton { get; set; }
        public AppBarButton ClearTimerButton { get; set; }
        public NumbersPanel HoursPanel { get; private set; }
        public NumbersPanel MinutesPanel { get; private set; }
        public NumbersPanel SecondsPanel { get; private set; }
        #endregion



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
        public CountdownTimer()
        {
            this.DefaultStyleKey = typeof(CountdownTimer);

            this.Loaded += (s, args) =>
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

                #region Timer Start, pause, clear events
                this.StartTimerButton.Click += (sender, e) =>
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
                    }

                    this.IsRunning = true;
                    this.IsPaused = false;

                    var timerArgs = new CountdownTimerEventArgs
                    {
                        StartTime = _startTime
                    };

                    OnTimerStarted(this, timerArgs);
                };

                this.PauseTimerButton.Click += (sender, e) =>
                {
                    _pausedTime = DateTime.UtcNow;
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    this.IsRunning = false;
                    this.IsPaused = true;
                    var timerArgs = new CountdownTimerEventArgs
                    {
                        StartTime = _startTime,
                        PauseTime = _pausedTime
                    };
                    OnTimerPaused(this, timerArgs);
                };

                this.ClearTimerButton.Click += (sender, e) =>
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

                #region Hour, Minute, Second click, selection changed events
                this.HoursButton.Click += (sender, e) =>
                {
                    this.HoursPanel.IsExpanded = !this.HoursPanel.IsExpanded;
                    if (this.HoursPanel.IsExpanded)
                    {
                        this.MinutesPanel.IsExpanded = this.SecondsPanel.IsExpanded = false;
                    }
                };

                this.HoursPanel.SelectedNumberChanged += (sender, e) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(e.NewNumber, 0, 0);
                        timeSettings.EndTime = _endTime = RefreshEndTime(e.NewNumber, e.OldNumber, ts);

                        this.SaveTimeSettings(controlName, timeSettings);
                    }

                    this.Hours = e.NewNumber;
                };

                this.MinutesButton.Click += (sender, e) =>
                {
                    this.MinutesPanel.IsExpanded = !this.MinutesPanel.IsExpanded;
                    if (this.MinutesPanel.IsExpanded)
                    {
                        this.HoursPanel.IsExpanded = this.SecondsPanel.IsExpanded = false;
                    }
                };

                this.MinutesPanel.SelectedNumberChanged += (sender, e) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(0, e.NewNumber, 0);
                        timeSettings.EndTime = _endTime = RefreshEndTime(e.NewNumber, e.OldNumber, ts);

                        SaveTimeSettings(controlName, timeSettings);
                    }
                    this.Minutes = e.NewNumber;
                };

                this.SecondsButton.Click += (sender, e) =>
                {
                    this.SecondsPanel.IsExpanded = !this.SecondsPanel.IsExpanded;
                    if (this.SecondsPanel.IsExpanded)
                    {
                        this.HoursPanel.IsExpanded = this.MinutesPanel.IsExpanded = false;
                    }
                };

                this.SecondsPanel.SelectedNumberChanged += (sender, e) =>
                {
                    if (timeSettings != null)
                    {
                        var ts = new TimeSpan(0, 0, e.NewNumber);
                        timeSettings.EndTime = _endTime = RefreshEndTime(e.NewNumber, e.OldNumber, ts);

                        SaveTimeSettings(controlName, timeSettings);
                    }
                    this.Seconds = e.NewNumber;
                };
                #endregion
            };

            this.Unloaded += (s, args) =>
            {
                this.HoursButton.Click -= (sender, e) => { };
                this.HoursPanel.SelectedNumberChanged -= (sender, e) => { };
                this.MinutesButton.Click -= (sender, e) => { };
                this.MinutesPanel.SelectedNumberChanged -= (sender, e) => { };
                this.SecondsButton.Click -= (sender, e) => { };
                this.SecondsPanel.SelectedNumberChanged -= (sender, e) => { };
                this.StartTimerButton.Click -= (sender, e) => { };
                this.PauseTimerButton.Click -= (sender, e) => { };
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            controlName = this.Name == string.Empty ? "timer" : this.Name;

            this.MainBorder = this.GetTemplateChild("MainBorder") as Border;
            this.HoursButton = this.GetTemplateChild("HoursButton") as Button;
            this.MinutesButton = this.GetTemplateChild("MinutesButton") as Button;
            this.SecondsButton = this.GetTemplateChild("SecondsButton") as Button;

            this.StartTimerButton = this.GetTemplateChild("StartTimerButton") as AppBarButton;
            this.PauseTimerButton = this.GetTemplateChild("PauseTimerButton") as AppBarButton;
            this.ClearTimerButton = this.GetTemplateChild("ClearTimerButton") as AppBarButton;

            this.HoursPanel = this.GetTemplateChild("HoursPanel") as NumbersPanel;
            this.MinutesPanel = this.GetTemplateChild("MinutesPanel") as NumbersPanel;
            this.SecondsPanel = this.GetTemplateChild("SecondsPanel") as NumbersPanel;
        }
    }
}
