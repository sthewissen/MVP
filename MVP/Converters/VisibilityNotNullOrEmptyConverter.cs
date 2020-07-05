using System;
using System.Globalization;
using Xamarin.Forms;

namespace MVP.Converters
{
    // TODO: This will be in XamarinCommunityToolkit, so replace once that has a release.
    public class VisibilityNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return !string.IsNullOrEmpty(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
