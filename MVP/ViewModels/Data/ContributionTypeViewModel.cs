using MVP.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class ContributionTypeViewModel : ObservableObject
    {
        public ContributionType ContributionType { get; set; }
        public bool IsSelected { get; set; }
    }
}
