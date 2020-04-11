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
            DateTime periodStart = new DateTime(DateTime.Now.Year, 4, 1);

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

        public static Tuple<string, string, string, bool> GetContributionTypeRequirements(this Guid contributionType)
        {
            var annualQuantityHeader = "";
            var secondAnnualQuantityHeader = "";
            var annualReachHeader = "";
            bool isUrlRequired = false;

            var guidString = contributionType.ToString();

            switch (guidString)
            {
                case "e36464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Article"
                    annualQuantityHeader = "Number of Articles";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Views";
                    break;
                case "df6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Blog/Website Post"
                    annualQuantityHeader = "Number of Posts";
                    secondAnnualQuantityHeader = "Number of Subscribers";
                    annualReachHeader = "Annual Unique Visitors";
                    isUrlRequired = true;
                    break;
                case "db6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Author)"
                    annualQuantityHeader = "Number of Books";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Copies Sold";
                    break;
                case "dd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Book (Co-Author)"
                    annualQuantityHeader = "Number of Books";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Copies Sold";
                    break;
                case "f16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Conference (Staffing)"
                    annualQuantityHeader = "Number of Conferences";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Visitors";
                    break;
                case "0ce0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Docs.Microsoft.com Contribution"
                    annualQuantityHeader = "Pull Requests/Issues/Submissions";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "";
                    break;
                case "f96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Moderator"
                    annualQuantityHeader = "Number of Threads moderated";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "";
                    break;
                case "d96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (3rd Party forums)"
                    annualQuantityHeader = "Number of Answers";
                    secondAnnualQuantityHeader = "Number of Posts";
                    annualReachHeader = "Views of Answers";
                    isUrlRequired = true;
                    break;
                case "d76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Forum Participation (Microsoft Forums)"
                    annualQuantityHeader = "Number of Answers";
                    secondAnnualQuantityHeader = "Number of Posts";
                    annualReachHeader = "Views of Answers";
                    isUrlRequired = true;
                    break;
                case "f76464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Mentorship"
                    annualQuantityHeader = "Number of Mentorship Activity";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Mentees";
                    break;
                case "d2d96407-0304-e911-8171-3863bb2bca60": // "EnglishName": "Microsoft Open Source Projects"
                    annualQuantityHeader = "Number of Projects";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "";
                    break;
                case "414bcf30-e889-e511-8110-c4346bac0abc": // "EnglishName": "Non-Microsoft Open Source Projects"
                    annualQuantityHeader = "Project(s)";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Contributions";
                    break;
                case "fd6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer (User Group/Meetup/Local Events)"
                    annualQuantityHeader = "Meetings";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Members";
                    break;
                case "ef6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Organizer of Conference"
                    annualQuantityHeader = "Number of Conferences";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Attendees";
                    break;
                case "ff6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Other"
                    annualQuantityHeader = "Annual Quantity";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Annual Reach";
                    break;
                case "016564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Product Group Feedback (General)"
                    annualQuantityHeader = "Number of Events Participated";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Feedbacks Provided";
                    break;
                case "e96464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Sample Code/Projects/Tools"
                    annualQuantityHeader = "Number of Samples";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Downloads";
                    isUrlRequired = true;
                    break;
                case "fb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Site Owner"
                    annualQuantityHeader = "Number of Sites";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Visitors";
                    break;
                case "d16464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (Conference)"
                    annualQuantityHeader = "Number of Talks";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Attendees of Talks";
                    break;
                case "d56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Speaking (User Group/Meetup/Local events)"
                    annualQuantityHeader = "Number of Talks";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Attendees of Talks";
                    break;
                case "eb6464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Technical Social Media (Twitter, Facebook, LinkedIn...)"
                    annualQuantityHeader = "Number of Talks";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Followers";
                    isUrlRequired = true;
                    break;
                case "056564de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Translation Review, Feedback and Editing"
                    annualQuantityHeader = "Annual Quantity";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "";
                    break;
                case "e56464de-179a-e411-bbc8-6c3be5a82b68": // "EnglishName": "Video/Webcast/Podcast"
                    annualQuantityHeader = "Number of Videos";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "Number of Views";
                    isUrlRequired = true;
                    break;
                case "0ee0dc15-0304-e911-8171-3863bb2bca60": // "EnglishName": "Workshop/Volunteer/Proctor"
                    annualQuantityHeader = "Number of Events";
                    secondAnnualQuantityHeader = "";
                    annualReachHeader = "";
                    break;
            }

            return new Tuple<string, string, string, bool>(annualQuantityHeader, secondAnnualQuantityHeader, annualReachHeader, isUrlRequired);
        }
    }
}
