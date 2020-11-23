using System;
using System.Collections.ObjectModel;
using MVP.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels.Data
{
    public class ContributionViewModel : ObservableObject
    {
        public int? ContributionId { get; set; }
        public ContributionType ContributionType { get; set; }
        public ContributionTechnology ContributionTechnology { get; set; }
        public ObservableCollection<ContributionTechnology> AdditionalTechnologies { get; set; } = new ObservableCollection<ContributionTechnology>();
        public DateTime StartDate { get; set; }
        public string Title { get; set; }
        public string ReferenceUrl { get; set; }
        public Visibility Visibility { get; set; }
        public int? AnnualQuantity { get; set; }
        public int? SecondAnnualQuantity { get; set; }
        public int? AnnualReach { get; set; }
        public string Description { get; set; }
    }
}
