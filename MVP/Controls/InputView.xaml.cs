using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class InputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty DescriptionProperty =
         BindableProperty.Create(nameof(Description), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty PlaceholderProperty =
         BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty IsRequiredProperty =
         BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(AppFrame), false, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty IsMultiLineProperty =
         BindableProperty.Create(nameof(IsMultiLine), typeof(bool), typeof(AppFrame), false, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty KeyboardProperty =
         BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(AppFrame), Keyboard.Default, defaultBindingMode: BindingMode.OneTime);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        public bool IsMultiLine
        {
            get => (bool)GetValue(IsMultiLineProperty);
            set => SetValue(IsMultiLineProperty, value);
        }

        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public InputView()
            => InitializeComponent();

        void Entry_Focused(object sender, FocusEventArgs e)
        {
            frameEntry.BorderColor = (Color)Application.Current.Resources["primary"];
            frameEditor.BorderColor = (Color)Application.Current.Resources["primary"];
            //frameEntry.HasShadow = true;
        }

        void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            frameEntry.BorderColor = (Color)Application.Current.Resources["border_light_gray"];
            frameEditor.BorderColor = (Color)Application.Current.Resources["border_light_gray"];
            //frameEntry.HasShadow = false;
        }
    }
}
