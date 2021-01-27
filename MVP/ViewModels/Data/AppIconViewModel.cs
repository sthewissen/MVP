using MVP.Models.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels.Data
{
    public class AppIconViewModel : ObservableObject
    {
        public AppIcon Key { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsSelected { get; set; }
    }
}
