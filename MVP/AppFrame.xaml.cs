using System;
using System.Collections.Generic;
using MVP.Pages;
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

        public static readonly BindableProperty IsPrimaryButtonIconProperty =
            BindableProperty.Create(nameof(IsPrimaryButtonIcon), typeof(bool), typeof(AppFrame), true, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty PrimaryButtonTextProperty =
            BindableProperty.Create(nameof(PrimaryButtonText), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowSecondaryButtonProperty =
            BindableProperty.Create(nameof(ShowSecondaryButton), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty SecondaryButtonTextProperty =
            BindableProperty.Create(nameof(SecondaryButtonText), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowBackButtonProperty =
            BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(AppFrame), default(bool), defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ShowTabShadowProperty =
            BindableProperty.Create(nameof(ShowTabShadow), typeof(bool), typeof(AppFrame), true, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty OverlayContentProperty =
            BindableProperty.Create(nameof(OverlayContent), typeof(View), typeof(AppFrame));

        public static readonly BindableProperty OverlayControlTemplateProperty =
            BindableProperty.Create(nameof(OverlayControlTemplate), typeof(ControlTemplate), typeof(AppFrame));

        public View OverlayContent
        {
            get => (View)GetValue(OverlayContentProperty);
            set => SetValue(OverlayContentProperty, value);
        }

        public ControlTemplate OverlayControlTemplate
        {
            get => (ControlTemplate)GetValue(OverlayControlTemplateProperty);
            set => SetValue(OverlayControlTemplateProperty, value);
        }

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

        public bool IsPrimaryButtonIcon
        {
            get => (bool)GetValue(IsPrimaryButtonIconProperty);
            set => SetValue(IsPrimaryButtonIconProperty, value);
        }

        public string PrimaryButtonText
        {
            get => (string)GetValue(PrimaryButtonTextProperty);
            set => SetValue(PrimaryButtonTextProperty, value);
        }

        public bool ShowSecondaryButton
        {
            get => (bool)GetValue(ShowSecondaryButtonProperty);
            set => SetValue(ShowSecondaryButtonProperty, value);
        }

        public string SecondaryButtonText
        {
            get => (string)GetValue(SecondaryButtonTextProperty);
            set => SetValue(SecondaryButtonTextProperty, value);
        }

        public bool ShowBackButton
        {
            get => (bool)GetValue(ShowBackButtonProperty);
            set => SetValue(ShowBackButtonProperty, value);
        }

        public bool ShowTabShadow
        {
            get => (bool)GetValue(ShowTabShadowProperty);
            set => SetValue(ShowTabShadowProperty, value);
        }

        public AppFrame()
            => InitializeComponent();
    }
}
