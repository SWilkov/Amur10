using Amur10.Test.CustomPages;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Amur10.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<CustomPage> CustomPages;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (s, args) =>
            {
                this.CustomPages = new List<CustomPage>
                {
                    new CustomPage { Title = "CountdownTimer", PageType = typeof(CountdownTimerPage), Description = "A countdown timer that raises events on start, pause, end. Fully customizable" },
                    new CustomPage { Title = "Mini CountdownTimer", PageType = typeof(MiniCountdownTimerPage), Description = "A mini countdown timer that raises events on start, pause, end. Fully customizable" }
                };

                this.DataContext = CustomPages;
                this.PageFrame.Navigate(typeof(StarterPage));                
            };
        }        

        private void MiniCountdownButton_Click(object sender, RoutedEventArgs e)
        {
            this.PageFrame.Navigate(typeof(MiniCountdownTimerPage), this.CustomPages.FirstOrDefault(p => p.Title == "Mini CountdownTimer"));
        }

        private void CountdownButton_Click(object sender, RoutedEventArgs e)
        {
            this.PageFrame.Navigate(typeof(CountdownTimerPage), this.CustomPages.First());
        }
    }

    public sealed class CustomPage
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
        public string Description { get; set; }
    }
}
