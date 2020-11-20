using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class DateInputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(DateTime), typeof(AppFrame), defaultValue: DateTime.Now.Date, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty DescriptionProperty =
         BindableProperty.Create(nameof(Description), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty IsRequiredProperty =
         BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(AppFrame), false, defaultBindingMode: BindingMode.OneTime);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public DateTime Value
        {
            get => (DateTime)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
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

        public DateInputView()
            => InitializeComponent();

        void DatePicker_Focused(object sender, FocusEventArgs e)
        {
            framePicker.BorderColor = (Color)Application.Current.Resources["primary"];
        }

        void DatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            framePicker.BorderColor = (Color)Application.Current.Resources["border_light_gray"];
        }
    }
}
