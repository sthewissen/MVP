using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels.Data
{
    public class AppIconViewModel : ObservableObject
    {
        public int Key { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsSelected { get; set; }
    }
}
