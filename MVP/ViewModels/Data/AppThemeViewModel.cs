using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class AppThemeViewModel : ObservableObject
    {
        public int Key { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
