using System;
using System.Globalization;
using Xamarin.Forms;

namespace MVP.Converters
{
    // TODO: This will be in XamarinCommunityToolkit, so replace once that has a release.
    public class VisibilityNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
