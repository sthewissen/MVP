using MVP.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class VisibilityViewModel : ObservableObject
    {
        public Visibility Visibility { get; set; }
        public bool IsSelected { get; set; }
    }
}
