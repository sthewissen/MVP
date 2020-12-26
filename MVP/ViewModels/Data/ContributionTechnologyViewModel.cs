using MVP.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels
{
    public class ContributionTechnologyViewModel : ObservableObject
    {
        public ContributionTechnology ContributionTechnology { get; set; }
        public bool IsSelected { get; set; }
    }
}
