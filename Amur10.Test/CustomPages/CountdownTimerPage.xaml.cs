using Amur10.Shared.Models;
using Amur10.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Amur10.Test.CustomPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CountdownTimerPage : Page
    {
        private const string ctrlName = "Countdown Timer";
        
        public CountdownTimerPage()
        {
            this.InitializeComponent();

            Loaded += (sender, e) =>
            {                
                ct.TimerStarted += (s, args) =>
                {
                    var msg = new LogMessage
                    {
                        CreatedDate = DateTime.Now,
                        Content = $"{ctrlName} started at {args.StartTime}"
                    };
                    LoggingViewModel.Instance.AddMessage(msg);
                };

                ct.TimerPaused += (s, args) =>
                {
                    var msg = new LogMessage
                    {
                        CreatedDate = DateTime.Now,
                        Content = $"{ctrlName} paused at {args.PauseTime}"
                    };
                    LoggingViewModel.Instance.AddMessage(msg);
                };

                ct.TimerEnded += (s, args) =>
                {
                    var msg = new LogMessage
                    {
                        CreatedDate = DateTime.Now,
                        Content = $"{ctrlName} ended at {args.EndTime}"
                    };
                    LoggingViewModel.Instance.AddMessage(msg);
                };
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                var page = e.Parameter as CustomPage;
                this.DataContext = page;
            }
        }
    }
}
