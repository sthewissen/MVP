using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace MVP.ViewModels
{
    public class AppThemeViewModel : ObservableObject
    {
        public OSAppTheme Key { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
