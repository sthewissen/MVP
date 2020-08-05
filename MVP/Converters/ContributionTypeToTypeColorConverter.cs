using System;
using System.Globalization;
using MVP.Extensions;
using Xamarin.Forms;

namespace MVP.Converters
{
    public class ContributionTypeToTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Guid id))
                return string.Empty;

            return id.GetContributionTypeRequirements().TypeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
