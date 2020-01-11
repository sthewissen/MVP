using System;
using System.Collections.Generic;

namespace MVP.Models
{
    /// <summary>
    /// The activity view model.
    /// Note:
    /// The logic of activity is a little complicated. While editing an activity, the ActivityType should be non-editable;
    /// While adding an activity, the display name of AnnualQuantity/SecondAnnualQuantity/ReachScore should change with
    /// ActivityType changing, and validation rules are different while ActivityType changes.
    /// <remarks>
    /// Book (Author):
    /// [msft_title: required, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_annualquantity: required, 'Books']
    /// Book (Co-Author): same as Book (Author)
    /// Conference (booth presenter):
    /// [msft_title: required, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_annualquantity: required, 'Conferences']
    /// Conference (organizer): same as Conference (booth presenter)
    /// Speaking (Conference):
    /// [msft_title: required, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_annualquantity: required, 'Talks']
    /// Speaking (Local):Same as Speaking (Conference)
    /// Speaking (User group):Same as Speaking (Conference)
    /// Forum Moderator:
    /// [msft_title: required, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_annualquantity: required, 'Threads Moderated']
    /// Forum Participation (Microsoft Forums):
    /// [msft_title: recommend, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_url: required, 'Url']
    /// [msft_annualquantity: required, 'Answers']
    /// Forum Participation (3rd Party forums):
    /// [msft_title: recommend, 'Title']
    /// [msft_startdate: required,'Start Date']
    /// [msft_url: required, 'Url']
    /// [msft_annualquantity: required, 'Answers']
    /// [msft_secondannualquantity: required, 'Posts']
    /// </remarks>
    /// </summary>
    public partial class Activity
    {
        public int? PrivateSiteId { get; set; }
        public ActivityType ActivityType { get; set; }
        public ActivityTechnology ApplicableTechnology { get; set; }
        public DateTime DateOfActivity { get; set; }
        public string DateOfActivityFormatted { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndDateFormatted { get; set; }
        public string TitleOfActivity { get; set; }
        public string ReferenceUrl { get; set; }
        public Visibility ActivityVisibility { get; set; }
        public int? AnnualQuantity { get; set; }
        public int? SecondAnnualQuantity { get; set; }
        public int? AnnualReach { get; set; }
        public string Description { get; set; }
        public OnlineIdentity OnlineIdentity { get; set; }
        public SocialNetwork SocialNetwork { get; set; }
        public string AllAnswersUrl { get; set; }
        public string AllPostsUrl { get; set; }
        public bool? IsSystemCollected { get; set; }
        public bool? IsBelongToLatestAwardCycle { get; set; }

        public string DisplayMode { get; set; }
        public IList<int?> ChartColumnIndexes { get; set; }
        public string DescriptionSummaryFormat { get; set; }
        public string DataTableTitle { get; set; }
        public string SubtitleHeader { get; set; }
        public bool? IsAllowEdit { get; set; }
        public bool? IsAllowDelete { get; set; }
        public bool? IsFromBookmarklet { get; set; }
        public bool? Submitted { get; set; }
    }
}
