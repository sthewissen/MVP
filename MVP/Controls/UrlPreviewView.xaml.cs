using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MVP.Extensions;
using MVP.Helpers;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class UrlPreviewView : StackLayout
    {
        CancellationTokenSource tokenSource;
        bool initialized = false;

        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(UrlPreviewView), string.Empty, defaultBindingMode: BindingMode.OneWay, propertyChanged: Url_Changed);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(UrlPreviewView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(UrlPreviewView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ImageUrlProperty =
            BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(UrlPreviewView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty HasValidUrlProperty =
            BindableProperty.Create(nameof(HasValidUrl), typeof(bool), typeof(UrlPreviewView), false, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty HasMetadataProperty =
            BindableProperty.Create(nameof(HasMetadata), typeof(bool), typeof(UrlPreviewView), false, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty StateProperty =
            BindableProperty.Create(nameof(State), typeof(LayoutState), typeof(UrlPreviewView), LayoutState.None, defaultBindingMode: BindingMode.OneWay);

        static void Url_Changed(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                (bindable as UrlPreviewView).OnUrlPropertyChanged();
            }
        }

        public LayoutState State
        {
            get => (LayoutState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
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

        public bool HasMetadata
        {
            get => (bool)GetValue(HasMetadataProperty);
            set => SetValue(HasMetadataProperty, value);
        }

        public UrlPreviewView()
            => InitializeComponent();


        protected void OnUrlPropertyChanged()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }

            tokenSource = new CancellationTokenSource();

            _ = Task.Delay(!initialized ? 0 : 2000, tokenSource.Token)
                .ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.Canceled)
                        return;

                    initialized = true;
                    GetOpenGraphData().SafeFireAndForget();
                });
        }

        public async Task GetOpenGraphData()
        {
            try
            {
                var result = Uri.TryCreate(Url, UriKind.Absolute, out var uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!result)
                {
                    Title = string.Empty;
                    Description = string.Empty;
                    ImageUrl = string.Empty;
                    HasValidUrl = false;
                    HasMetadata = false;
                    return;
                }

                State = LayoutState.Loading;

                var openGraphData = await OpenGraph.ParseUrlAsync(Url);

                if (openGraphData.Metadata.ContainsKey("og:title"))
                    Title = HttpUtility.HtmlDecode(openGraphData.Metadata["og:title"].Value());

                if (openGraphData.Metadata.ContainsKey("og:description"))
                    Description = HttpUtility.HtmlDecode(openGraphData.Metadata["og:description"].Value());

                if (openGraphData.Metadata.ContainsKey("og:image"))
                    ImageUrl = openGraphData.Metadata["og:image"].First().Value;

                HasValidUrl = true;
                HasMetadata = !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(ImageUrl);
            }
            catch
            {
                // Catch 404s etc, but don't really care much about it further.
                Title = string.Empty;
                Description = string.Empty;
                ImageUrl = string.Empty;
                HasValidUrl = false;
                HasMetadata = false;
            }
            finally
            {
                State = LayoutState.None;
            }
        }
    }
}
