using System;
using MVP.Models;
using MVP.Resources;
using MVP.Validation;
using MVP.ViewModels.Data;

namespace MVP.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts an API returned contribution to a useable view model.
        /// </summary>
        public static ContributionViewModel ToContributionViewModel(this Contribution contribution)
        {
            if (contribution == null)
                return null;

            return new ContributionViewModel
            {
                AdditionalTechnologies = new Xamarin.CommunityToolkit.ObjectModel.ObservableRangeCollection<ContributionTechnology>(contribution.AdditionalTechnologies),
                AnnualQuantity = new ValidatableObject<int?> { Value = contribution.AnnualQuantity },
                AnnualReach = new ValidatableObject<int?> { Value = contribution.AnnualReach },
                ContributionId = contribution.ContributionId,
                ContributionTechnology = new ValidatableObject<ContributionTechnology> { Value = contribution.ContributionTechnology },
                ContributionType = new ValidatableObject<ContributionType> { Value = contribution.ContributionType },
                Description = contribution.Description,
                ReferenceUrl = new ValidatableObject<string> { Value = contribution.ReferenceUrl },
                SecondAnnualQuantity = new ValidatableObject<int?> { Value = contribution.SecondAnnualQuantity },
                StartDate = contribution.StartDate ?? DateTime.Now.Date,
                Title = new ValidatableObject<string> { Value = contribution.Title },
                Visibility = new ValidatableObject<Visibility> { Value = contribution.Visibility }
            };
        }

        /// <summary>
        /// Converts a local view model to an API compatible contribution.
        /// </summary>
        public static Contribution ToContribution(this ContributionViewModel contribution)
        {
            if (contribution == null)
                return null;

            return new Contribution
            {
                AdditionalTechnologies = contribution.AdditionalTechnologies,
                AnnualQuantity = contribution.AnnualQuantity?.Value ?? 0,
                AnnualReach = contribution.AnnualReach?.Value ?? 0,
                ContributionId = contribution.ContributionId,
                ContributionTechnology = contribution.ContributionTechnology.Value,
                ContributionType = contribution.ContributionType.Value,
                Description = contribution.Description,
                ReferenceUrl = contribution.ReferenceUrl.Value,
                SecondAnnualQuantity = contribution.SecondAnnualQuantity?.Value ?? 0,
                StartDate = contribution.StartDate.Date,
                Title = contribution.Title.Value,
                Visibility = contribution.Visibility.Value,
            };
        }

        /// <summary>
        /// Retrieves contribution type information based on the provided Guid.
        /// </summary>
        public static ContributionTypeConfig GetContributionTypeRequirements(this Guid contributionType)
        {
            var config = new ContributionTypeConfig();

            var guidString = contributionType.ToString();

            switch (guidString)
            {
                case "e36464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Article"
                    config.AnnualQuantityHeader = Translations.field_number_of_articles;
                    config.AnnualReachHeader = Translations.field_number_of_views;
                    config.HasAnnualReach = true;
                    config.TypeColor = "red_light";
                    config.TextColor = "white";
                    break;
                case "df6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Blog/Website Post"
                    config.AnnualQuantityHeader = Translations.field_number_of_posts;
                    config.SecondAnnualQuantityHeader = Translations.field_number_of_subscribers;
                    config.AnnualReachHeader = Translations.field_annual_unique_visitors;
                    config.IsUrlRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "orange_light";
                    config.TextColor = "white";
                    break;
                case "db6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Author)"
                    config.AnnualQuantityHeader = Translations.field_number_of_books;
                    config.AnnualReachHeader = Translations.field_copies_sold;
                    config.HasAnnualReach = true;
                    config.TypeColor = "yellow_light";
                    config.TextColor = "black";
                    break;
                case "dd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Co-Author)"
                    config.AnnualQuantityHeader = Translations.field_number_of_books;
                    config.AnnualReachHeader = Translations.field_copies_sold;
                    config.HasAnnualReach = true;
                    config.TypeColor = "sand_light";
                    config.TextColor = "black";
                    break;
                case "f16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Conference (Staffing)"
                    config.AnnualQuantityHeader = Translations.field_number_of_conferences;
                    config.AnnualReachHeader = Translations.field_number_of_visitors;
                    config.HasAnnualReach = true;
                    config.TypeColor = "navyblue_light";
                    config.TextColor = "white";
                    break;
                case "0ce0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Docs.Microsoft.com Contribution"
                    config.AnnualQuantityHeader = Translations.field_prs_issues_submissions;
                    config.TypeColor = "magenta_light";
                    config.TextColor = "white";
                    break;
                case "f96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Moderator"
                    config.AnnualQuantityHeader = Translations.field_number_of_threads_moderated;
                    config.TypeColor = "teal_light";
                    config.TextColor = "white";
                    break;
                case "d96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (3rd Party forums)"
                    config.AnnualQuantityHeader = Translations.field_number_of_answers;
                    config.SecondAnnualQuantityHeader = Translations.field_number_of_posts;
                    config.AnnualReachHeader = Translations.field_views_of_answers;
                    config.IsUrlRequired = true;
                    config.IsSecondAnnualQuantityRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "skyblue_light";
                    config.TextColor = "white";
                    break;
                case "d76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (Microsoft Forums)"
                    config.AnnualQuantityHeader = Translations.field_number_of_answers;
                    config.SecondAnnualQuantityHeader = Translations.field_number_of_posts;
                    config.AnnualReachHeader = Translations.field_views_of_answers;
                    config.IsUrlRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "green_light";
                    config.TextColor = "white";
                    break;
                case "f76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Mentorship"
                    config.AnnualQuantityHeader = Translations.field_number_of_mentorship_activity;
                    config.AnnualReachHeader = Translations.field_number_of_mentees;
                    config.HasAnnualReach = true;
                    config.TypeColor = "black_light";
                    config.TextColor = "white";
                    break;
                case "d2d96407-0304-e911-8171-3863bb2bca60": // "EnglishName": "Microsoft Open Source Projects"
                    config.AnnualQuantityHeader = Translations.field_number_of_projects;
                    config.TypeColor = "mint_light";
                    config.TextColor = "white";
                    break;
                case "414bcf30-e889-e511-8110-c4346bac0abc": // "EnglishName": "Non-Microsoft Open Source Projects"
                    config.AnnualQuantityHeader = Translations.field_projects;
                    config.AnnualReachHeader = Translations.field_contributions;
                    config.HasAnnualReach = true;
                    config.TypeColor = "forestgreen_light";
                    config.TextColor = "white";
                    break;
                case "fd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer (User Group/Meetup/Local Events)"
                    config.AnnualQuantityHeader = Translations.field_meetings;
                    config.AnnualReachHeader = Translations.field_members;
                    config.HasAnnualReach = true;
                    config.TypeColor = "purple_light";
                    config.TextColor = "white";
                    break;
                case "ef6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer of Conference"
                    config.AnnualQuantityHeader = Translations.field_number_of_conferences;
                    config.AnnualReachHeader = Translations.field_number_of_attendees;
                    config.HasAnnualReach = true;
                    config.TypeColor = "white_light";
                    config.TextColor = "black";
                    break;
                case "ff6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Other"
                    config.AnnualQuantityHeader = Translations.field_annual_quantity;
                    config.AnnualReachHeader = Translations.field_annual_reach;
                    config.HasAnnualReach = true;
                    config.TypeColor = "lime_light";
                    config.TextColor = "white";
                    break;
                case "016564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Product Group Feedback (General)"
                    config.AnnualQuantityHeader = Translations.field_number_of_events_participated;
                    config.AnnualReachHeader = Translations.field_number_of_feedbacks_provided;
                    config.HasAnnualReach = true;
                    config.TypeColor = "pink_light";
                    config.TextColor = "white";
                    break;
                case "e96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Sample Code/Projects/Tools"
                    config.AnnualQuantityHeader = Translations.field_number_of_samples;
                    config.AnnualReachHeader = Translations.field_number_of_downloads;
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "maroon_light";
                    config.TextColor = "white";
                    break;
                case "fb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Site Owner"
                    config.AnnualQuantityHeader = Translations.field_number_of_sites;
                    config.AnnualReachHeader = Translations.field_number_of_visitors;
                    config.HasAnnualReach = true;
                    config.TypeColor = "coffee_light";
                    config.TextColor = "white";
                    break;
                case "d16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (Conference)"
                    config.AnnualQuantityHeader = Translations.field_number_of_talks;
                    config.SecondAnnualQuantityHeader = "";
                    config.AnnualReachHeader = Translations.field_attendees_of_talks;
                    config.TypeColor = "powderblue_light";
                    config.TextColor = "black";
                    break;
                case "d56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (User Group/Meetup/Local events)"
                    config.AnnualQuantityHeader = Translations.field_number_of_talks;
                    config.AnnualReachHeader = Translations.field_attendees_of_talks;
                    config.HasAnnualReach = true;
                    config.TypeColor = "blue_light";
                    config.TextColor = "white";
                    break;
                case "eb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Technical Social Media (Twitter, Facebook, LinkedIn...)"
                    config.AnnualQuantityHeader = Translations.field_number_of_talks;
                    config.AnnualReachHeader = Translations.field_number_of_followers;
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "brown_light";
                    config.TextColor = "white";
                    break;
                case "056564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Translation Review, Feedback and Editing"
                    config.AnnualQuantityHeader = Translations.field_annual_quantity;
                    config.TypeColor = "plum_light";
                    config.TextColor = "white";
                    break;
                case "e56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Video/Webcast/Podcast"
                    config.AnnualQuantityHeader = Translations.field_number_of_videos;
                    config.AnnualReachHeader = Translations.field_number_of_views;
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "watermelon_light";
                    config.TextColor = "white";
                    break;
                case "0ee0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Workshop/Volunteer/Proctor"
                    config.AnnualQuantityHeader = Translations.field_number_of_events;
                    config.TypeColor = "gray_light";
                    config.TextColor = "white";
                    break;
            }

            return config;
        }
    }
}
