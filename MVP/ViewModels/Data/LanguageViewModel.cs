using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class LanguageViewModel : ObservableObject
    {
        public string Description { get; set; }
        public string CurrentLanguageDescription { get; set; }
        public string CI { get; set; }
        public bool IsSelected { get; set; }
    }
}
