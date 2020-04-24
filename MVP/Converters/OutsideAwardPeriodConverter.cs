using System;
using System.Globalization;
using MVP.Extensions;
using Xamarin.Forms;

namespace MVP.Converters
{
    public class OutsideAwardPeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
                return !dateTime.IsWithinCurrentAwardPeriod();
            else if (value is DateTime?)
                return !(value as DateTime?).IsWithinCurrentAwardPeriod();
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
