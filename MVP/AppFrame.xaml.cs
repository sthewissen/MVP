using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVP
{
    public partial class AppFrame : Grid
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShadowOpacityProperty =
            BindableProperty.Create(nameof(ShadowOpacity), typeof(double), typeof(AppFrame), 0.0);

        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(AppFrame));

        public static readonly BindableProperty IsModalProperty =
            BindableProperty.Create(nameof(IsModal), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowPrimaryButtonProperty =
            BindableProperty.Create(nameof(ShowPrimaryButton), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty PrimaryButtonIconProperty =
            BindableProperty.Create(nameof(PrimaryButtonIcon), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowSecondaryButtonProperty =
            BindableProperty.Create(nameof(ShowSecondaryButton), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty SecondaryButtonIconProperty =
            BindableProperty.Create(nameof(SecondaryButtonIcon), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowBackButtonProperty =
            BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public bool IsModal
        {
            get => (bool)GetValue(IsModalProperty);
            set => SetValue(IsModalProperty, value);
        }

        public bool ShowPrimaryButton
        {
            get => (bool)GetValue(ShowPrimaryButtonProperty);
            set => SetValue(ShowPrimaryButtonProperty, value);
        }

        public string PrimaryButtonIcon
        {
            get => (string)GetValue(PrimaryButtonIconProperty);
            set => SetValue(PrimaryButtonIconProperty, value);
        }

        public bool ShowSecondaryButton
        {
            get => (bool)GetValue(ShowSecondaryButtonProperty);
            set => SetValue(ShowSecondaryButtonProperty, value);
        }

        public string SecondaryButtonIcon
        {
            get => (string)GetValue(SecondaryButtonIconProperty);
            set => SetValue(SecondaryButtonIconProperty, value);
        }

        public bool ShowBackButton
        {
            get => (bool)GetValue(ShowBackButtonProperty);
            set => SetValue(ShowBackButtonProperty, value);
        }

        public AppFrame()
            => InitializeComponent();
    }
}
