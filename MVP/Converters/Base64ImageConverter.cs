using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace MVP.Converters
{
    public class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return null;

            var byteArray = System.Convert.FromBase64String(value as string);
            var stream = new MemoryStream(byteArray);
            var imageSource = ImageSource.FromStream(() => stream);

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
