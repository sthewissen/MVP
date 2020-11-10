using System;
using System.Collections.Generic;
using System.Linq;
using MVP.Models;
using MvvmHelpers;

namespace MVP.Extensions
{
    public static class ModelExtensions
    {
        public static IList<Grouping<int, Contribution>> ToGroupedContributions(this IList<Contribution> list)
        {
            var result = new List<Grouping<int, Contribution>>();
            var periodStart = new DateTime(DateTime.Now.Year, 4, 1);

            // If we are before the 1st of April, the period start is last year's.
            if (DateTime.Now < periodStart)
            {
                periodStart = periodStart.AddYears(-1);
            }

            var thisPeriod = list.Where(x => x.StartDate >= periodStart).OrderByDescending(x => x.StartDate);
            var lastPeriod = list.Where(x => x.StartDate < periodStart).OrderByDescending(x => x.StartDate);

            if (thisPeriod.Any())
                result.Add(new Grouping<int, Contribution>(0, thisPeriod));

            if (lastPeriod.Any())
                result.Add(new Grouping<int, Contribution>(1, lastPeriod));

            return result;
        }

        public static ContributionTypeConfig GetContributionTypeRequirements(this Guid contributionType)
        {
            var config = new ContributionTypeConfig();

            var guidString = contributionType.ToString();

            switch (guidString)
            {
                case "e36464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Article"
                    config.AnnualQuantityHeader = "Number of Articles";
                    config.AnnualReachHeader = "Number of Views";
                    config.HasAnnualReach = true;
                    config.TypeColor = "red_light";
                    config.TextColor = "white";
                    break;
                case "df6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Blog/Website Post"
                    config.AnnualQuantityHeader = "Number of Posts";
                    config.SecondAnnualQuantityHeader = "Number of Subscribers";
                    config.AnnualReachHeader = "Annual Unique Visitors";
                    config.IsUrlRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "orange_light";
                    config.TextColor = "white";
                    break;
                case "db6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Author)"
                    config.AnnualQuantityHeader = "Number of Books";
                    config.AnnualReachHeader = "Copies Sold";
                    config.HasAnnualReach = true;
                    config.TypeColor = "yellow_light";
                    config.TextColor = "black";
                    break;
                case "dd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Co-Author)"
                    config.AnnualQuantityHeader = "Number of Books";
                    config.AnnualReachHeader = "Copies Sold";
                    config.HasAnnualReach = true;
                    config.TypeColor = "sand_light";
                    config.TextColor = "black";
                    break;
                case "f16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Conference (Staffing)"
                    config.AnnualQuantityHeader = "Number of Conferences";
                    config.AnnualReachHeader = "Number of Visitors";
                    config.HasAnnualReach = true;
                    config.TypeColor = "navyblue_light";
                    config.TextColor = "white";
                    break;
                case "0ce0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Docs.Microsoft.com Contribution"
                    config.AnnualQuantityHeader = "Pull Requests/Issues/Submissions";
                    config.TypeColor = "magenta_light";
                    config.TextColor = "white";
                    break;
                case "f96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Moderator"
                    config.AnnualQuantityHeader = "Number of Threads moderated";
                    config.TypeColor = "teal_light";
                    config.TextColor = "white";
                    break;
                case "d96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (3rd Party forums)"
                    config.AnnualQuantityHeader = "Number of Answers";
                    config.SecondAnnualQuantityHeader = "Number of Posts";
                    config.AnnualReachHeader = "Views of Answers";
                    config.IsUrlRequired = true;
                    config.IsSecondAnnualQuantityRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "skyblue_light";
                    config.TextColor = "white";
                    break;
                case "d76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (Microsoft Forums)"
                    config.AnnualQuantityHeader = "Number of Answers";
                    config.SecondAnnualQuantityHeader = "Number of Posts";
                    config.AnnualReachHeader = "Views of Answers";
                    config.IsUrlRequired = true;
                    config.HasAnnualReach = true;
                    config.HasSecondAnnualQuantity = true;
                    config.TypeColor = "green_light";
                    config.TextColor = "white";
                    break;
                case "f76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Mentorship"
                    config.AnnualQuantityHeader = "Number of Mentorship Activity";
                    config.AnnualReachHeader = "Number of Mentees";
                    config.HasAnnualReach = true;
                    config.TypeColor = "black_light";
                    config.TextColor = "white";
                    break;
                case "d2d96407-0304-e911-8171-3863bb2bca60": // "EnglishName": "Microsoft Open Source Projects"
                    config.AnnualQuantityHeader = "Number of Projects";
                    config.TypeColor = "mint_light";
                    config.TextColor = "white";
                    break;
                case "414bcf30-e889-e511-8110-c4346bac0abc": // "EnglishName": "Non-Microsoft Open Source Projects"
                    config.AnnualQuantityHeader = "Project(s)";
                    config.AnnualReachHeader = "Contributions";
                    config.HasAnnualReach = true;
                    config.TypeColor = "forestgreen_light";
                    config.TextColor = "white";
                    break;
                case "fd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer (User Group/Meetup/Local Events)"
                    config.AnnualQuantityHeader = "Meetings";
                    config.AnnualReachHeader = "Members";
                    config.HasAnnualReach = true;
                    config.TypeColor = "purple_light";
                    config.TextColor = "white";
                    break;
                case "ef6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer of Conference"
                    config.AnnualQuantityHeader = "Number of Conferences";
                    config.AnnualReachHeader = "Number of Attendees";
                    config.HasAnnualReach = true;
                    config.TypeColor = "watermelon_light";
                    config.TextColor = "white";
                    break;
                case "ff6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Other"
                    config.AnnualQuantityHeader = "Annual Quantity";
                    config.AnnualReachHeader = "Annual Reach";
                    config.HasAnnualReach = true;
                    config.TypeColor = "lime_light";
                    config.TextColor = "white";
                    break;
                case "016564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Product Group Feedback (General)"
                    config.AnnualQuantityHeader = "Number of Events Participated";
                    config.AnnualReachHeader = "Number of Feedbacks Provided";
                    config.HasAnnualReach = true;
                    config.TypeColor = "pink_light";
                    config.TextColor = "white";
                    break;
                case "e96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Sample Code/Projects/Tools"
                    config.AnnualQuantityHeader = "Number of Samples";
                    config.AnnualReachHeader = "Number of Downloads";
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "maroon_light";
                    config.TextColor = "white";
                    break;
                case "fb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Site Owner"
                    config.AnnualQuantityHeader = "Number of Sites";
                    config.AnnualReachHeader = "Number of Visitors";
                    config.HasAnnualReach = true;
                    config.TypeColor = "coffee_light";
                    config.TextColor = "white";
                    break;
                case "d16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (Conference)"
                    config.AnnualQuantityHeader = "Number of Talks";
                    config.SecondAnnualQuantityHeader = "";
                    config.AnnualReachHeader = "Attendees of Talks";
                    config.TypeColor = "powderblue_light";
                    config.TextColor = "white";
                    break;
                case "d56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (User Group/Meetup/Local events)"
                    config.AnnualQuantityHeader = "Number of Talks";
                    config.AnnualReachHeader = "Attendees of Talks";
                    config.HasAnnualReach = true;
                    config.TypeColor = "blue_light";
                    config.TextColor = "white";
                    break;
                case "eb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Technical Social Media (Twitter, Facebook, LinkedIn...)"
                    config.AnnualQuantityHeader = "Number of Talks";
                    config.AnnualReachHeader = "Number of Followers";
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "brown_light";
                    config.TextColor = "white";
                    break;
                case "056564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Translation Review, Feedback and Editing"
                    config.AnnualQuantityHeader = "Annual Quantity";
                    config.TypeColor = "plum_light";
                    config.TextColor = "white";
                    break;
                case "e56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Video/Webcast/Podcast"
                    config.AnnualQuantityHeader = "Number of Videos";
                    config.AnnualReachHeader = "Number of Views";
                    config.HasAnnualReach = true;
                    config.IsUrlRequired = true;
                    config.TypeColor = "white_light";
                    config.TextColor = "black";
                    break;
                case "0ee0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Workshop/Volunteer/Proctor"
                    config.AnnualQuantityHeader = "Number of Events";
                    config.TypeColor = "gray_light";
                    config.TextColor = "white";
                    break;
            }

            return config;
        }
    }
}
