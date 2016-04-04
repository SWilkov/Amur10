using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Amur10.Controls.Converters
{
    public class NotEqualsToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = false;
            bool isEnabled = true;
            if (bool.TryParse(value.ToString(), out result))
            {
                if (result)
                {
                    isEnabled = false;
                }
            }
            return isEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
