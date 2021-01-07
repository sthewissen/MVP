using System;
using Xamarin.Forms;

namespace MVP.Helpers
{
    public interface IStatusBar
    {
        void SetStatusBarColor(OSAppTheme theme, Color overrideColor = default);
    }
}
