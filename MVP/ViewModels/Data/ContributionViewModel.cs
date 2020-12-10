using System;
using System.Collections.Generic;
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

        public ContributionTypeConfig Requirements { get; set; }
        public int? ContributionId { get; set; }

        public ValidatableObject<ContributionType> ContributionType
        {
            get => contributionType; set
            {
                contributionType = value;

                if (value != null)
                {
                    Requirements = value.Value.Id.Value.GetContributionTypeRequirements();
                    AddVariableValidationRules(value);
                }

                OnPropertyChanged(nameof(ContributionType));
                OnPropertyChanged(nameof(Requirements));
            }
        }

        public ValidatableObject<ContributionTechnology> ContributionTechnology { get; set; } = new ValidatableObject<ContributionTechnology>();
        public IList<ContributionTechnology> AdditionalTechnologies { get; set; } = new List<ContributionTechnology>();
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
            Title.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Resources.Translations.field_title) });
            ContributionTechnology.Validations.Add(new IsNotNullOrEmptyRule<ContributionTechnology> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Resources.Translations.field_primary_contribution_area) });
            Visibility.Validations.Add(new IsNotNullOrEmptyRule<Visibility> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Resources.Translations.field_visibility) });
            ContributionType.Validations.Add(new IsNotNullOrEmptyRule<ContributionType> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Resources.Translations.field_activity_type) });
        }

        void AddVariableValidationRules(ValidatableObject<ContributionType> value)
        {
            ReferenceUrl.Validations.Clear();
            AnnualQuantity.Validations.Clear();
            AnnualReach.Validations.Clear();
            SecondAnnualQuantity.Validations.Clear();

            if (value.Value == null)
                return;

            if (Requirements.IsUrlRequired)
                ReferenceUrl.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Resources.Translations.field_url) });

            if (Requirements.IsAnnualQuantityRequired)
                AnnualQuantity.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Requirements.AnnualQuantityHeader) });

            if (Requirements.IsAnnualReachRequired)
                AnnualReach.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Requirements.AnnualReachHeader) });

            if (Requirements.IsSecondAnnualQuantityRequired)
                SecondAnnualQuantity.Validations.Add(new IsNotNullOrEmptyRule<int?> { ValidationMessage = string.Format(Resources.Translations.validation_variablefield, Requirements.SecondAnnualQuantityHeader) });
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
