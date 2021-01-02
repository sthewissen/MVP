using System;
using System.Globalization;

namespace MVP.Extensions
{
    public static class CultureExtensions
    {
        public static string GetNativeName(this string ci)
        {
            var cult = new CultureInfo(ci);

            return cult != null ? cult.NativeName : string.Empty;
        }
    }
}
