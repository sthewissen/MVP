using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class DateInputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(DateInputView), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(DateTime), typeof(DateInputView), defaultValue: DateTime.Now.Date, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty MinimumDateProperty =
            BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(DateInputView));

        public static readonly BindableProperty DescriptionProperty =
         BindableProperty.Create(nameof(Description), typeof(string), typeof(DateInputView), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty IsRequiredProperty =
         BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(DateInputView), false, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty IsValidProperty =
         BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(DateInputView), true, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ErrorsProperty =
         BindableProperty.Create(nameof(Errors), typeof(string), typeof(DateInputView), null, defaultBindingMode: BindingMode.OneWay);

        public string Errors
        {
            get => (string)GetValue(ErrorsProperty);
            set => SetValue(ErrorsProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

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

        public DateTime MinimumDate
        {
            get => (DateTime)GetValue(MinimumDateProperty);
            set => SetValue(MinimumDateProperty, value);
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
            framePicker.SetAppThemeColor(Frame.BorderColorProperty, (Color)Application.Current.Resources["primary"], (Color)Application.Current.Resources["light_primary"]);
        }

        void DatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            framePicker.SetAppThemeColor(Frame.BorderColorProperty, (Color)Application.Current.Resources["neutral_1"], (Color)Application.Current.Resources["neutral_3"]);
        }
    }
}
