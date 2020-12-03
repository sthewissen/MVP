using System;
using System.Collections.ObjectModel;
using MVP.Extensions;
using MVP.Models;
using MVP.Validation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.ViewModels.Data
{
    public class ContributionViewModel : ObservableObject
    {
        ValidatableObject<ContributionType> contributionType = new ValidatableObject<ContributionType>();

        public int? ContributionId { get; set; }

        public ValidatableObject<ContributionType> ContributionType
        {
            get => contributionType; set
            {
                contributionType = value;
                AddVariableValidationRules(value);
                OnPropertyChanged(nameof(ContributionType));
            }
        }

        public ValidatableObject<ContributionTechnology> ContributionTechnology { get; set; } = new ValidatableObject<ContributionTechnology>();
        public ObservableCollection<ContributionTechnology> AdditionalTechnologies { get; set; } = new ObservableCollection<ContributionTechnology>();
        public DateTime StartDate { get; set; } = DateTime.Now;
        public ValidatableObject<string> Title { get; set; } = new ValidatableObject<string>();
        public ValidatableObject<string> ReferenceUrl { get; set; } = new ValidatableObject<string>();
        public ValidatableObject<Visibility> Visibility { get; set; } = new ValidatableObject<Visibility>();
        public ValidatableObject<int?> AnnualQuantity { get; set; } = new ValidatableObject<int?>();
        public ValidatableObject<int?> SecondAnnualQuantity { get; set; } = new ValidatableObject<int?>();
        public ValidatableObject<int?> AnnualReach { get; set; } = new ValidatableObject<int?>();
        public string Description { get; set; }

        public void AddValidationRules()
        {
            Title.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Needs a title!" });
            ContributionTechnology.Validations.Add(new IsNotNullOrEmptyRule<ContributionTechnology> { ValidationMessage = "Needs a ContributionTechnology!" });
            Visibility.Validations.Add(new IsNotNullOrEmptyRule<Visibility> { ValidationMessage = "Needs a Visibility!" });
            ContributionType.Validations.Add(new IsNotNullOrEmptyRule<ContributionType> { ValidationMessage = "Needs a ContributionType!" });
        }

        void AddVariableValidationRules(ValidatableObject<ContributionType> value)
        {
            ReferenceUrl.Validations.Clear();
            AnnualQuantity.Validations.Clear();
            AnnualReach.Validations.Clear();
            SecondAnnualQuantity.Validations.Clear();

            if (value.Value == null)
                return;

            var contributionType = value.Value.Id.Value.GetContributionTypeRequirements();

            if (contributionType.IsUrlRequired)
                ReferenceUrl.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Needs a URL!" });

            if (contributionType.IsAnnualQuantityRequired)
                AnnualQuantity.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = $"Needs {contributionType.AnnualQuantityHeader}!" });

            if (contributionType.IsAnnualReachRequired)
                AnnualReach.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = $"Needs {contributionType.AnnualReachHeader}!" });

            if (contributionType.IsSecondAnnualQuantityRequired)
                SecondAnnualQuantity.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = $"Needs {contributionType.SecondAnnualQuantityHeader}!" });
        }

        public bool IsValid()
        {
            var isTitleValid = Title.Validate();
            var isContributionTechnologyValid = ContributionTechnology.Validate();
            var isContributionTypeValid = ContributionType.Validate();
            var isReferenceUrlValid = ReferenceUrl.Validate();
            var isAnnualQuantityValid = AnnualQuantity.Validate();
            var isAnnualReachValid = AnnualReach.Validate();
            var isSecondAnnualQuantityValid = SecondAnnualQuantity.Validate();
            var isVisibilityValid = Visibility.Validate();

            return isAnnualQuantityValid && isAnnualReachValid && isContributionTechnologyValid && isContributionTypeValid &&
                isReferenceUrlValid && isSecondAnnualQuantityValid && isTitleValid && isVisibilityValid;
        }
    }
}
