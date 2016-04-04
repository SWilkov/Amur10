using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Amur10.Controls.CountdownTimer.Animations
{
    public class OpenCloseSettingsAnimation
    {
        public Storyboard GetOpenCloseSettingsStoryboard(Grid settingsGrid, double duration, double to)
        {
            var sb = new Storyboard();
            var animationDuration = new Duration(TimeSpan.FromMilliseconds(duration));

            var animation = new DoubleAnimation()
            {
                To = to,
                Duration = animationDuration
            };

            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(animation, settingsGrid);
            sb.Children.Add(animation);
            return sb;
        }
    }
}
