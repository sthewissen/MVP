using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MVP.Helpers;
using MvvmHelpers;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class UrlPreviewView : Grid
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay, propertyChanged: Url_Changed);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ImageUrlProperty =
            BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty HasValidUrlProperty =
            BindableProperty.Create(nameof(HasValidUrl), typeof(bool), typeof(AppFrame), false, defaultBindingMode: BindingMode.OneWay);

        static void Url_Changed(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                (bindable as UrlPreviewView).GetOpenGraphData().SafeFireAndForget();
            }
        }

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }

        public bool HasValidUrl
        {
            get => (bool)GetValue(HasValidUrlProperty);
            set => SetValue(HasValidUrlProperty, value);
        }

        public UrlPreviewView()
        {
            InitializeComponent();
        }

        public async Task GetOpenGraphData()
        {
            try
            {
                if (string.IsNullOrEmpty(Url) || (!Url.StartsWith("http://") && !Url.StartsWith("https://")))
                {
                    Title = string.Empty;
                    Description = string.Empty;
                    ImageUrl = string.Empty;
                    HasValidUrl = false;
                    return;
                }

                var openGraphData = await OpenGraph.ParseUrlAsync(Url);

                if (openGraphData.Metadata.ContainsKey("og:title"))
                    Title = HttpUtility.HtmlDecode(openGraphData.Metadata["og:title"].Value());

                if (openGraphData.Metadata.ContainsKey("og:description"))
                    Description = HttpUtility.HtmlDecode(openGraphData.Metadata["og:description"].Value());

                if (openGraphData.Metadata.ContainsKey("og:image"))
                    ImageUrl = openGraphData.Metadata["og:image"].First().Value;

                HasValidUrl = !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(ImageUrl);
            }
            catch
            {
                // Catch 404s etc, but don't really care much about it further.
                Title = string.Empty;
                Description = string.Empty;
                ImageUrl = string.Empty;
                HasValidUrl = false;
            }
        }
    }
}
