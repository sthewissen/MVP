using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace MVP.Converters
{
    public class StringToColorResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Color.Default;

            var valueAsString = value.ToString();

            switch (valueAsString)
            {
                case "":
                    return Color.Default;
                default:
                    var c = LookupColor(valueAsString);
                    return c;
            }
        }

        public Color LookupColor(string key)
        {
            try
            {
                Application.Current.Resources.TryGetValue(key, out var newColor);
                return (Color)newColor;
            }
            catch
            {
                return Color.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
