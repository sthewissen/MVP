using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class PickerInputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(object), typeof(AppFrame), null, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty PlaceholderProperty =
         BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty DescriptionProperty =
         BindableProperty.Create(nameof(Description), typeof(string), typeof(AppFrame), string.Empty, defaultBindingMode: BindingMode.OneTime);

        public static readonly BindableProperty PickerCommandProperty =
         BindableProperty.Create(nameof(PickerCommand), typeof(ICommand), typeof(AppFrame), null, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsRequiredProperty =
         BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(AppFrame), false, defaultBindingMode: BindingMode.OneTime);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public ICommand PickerCommand
        {
            get => (ICommand)GetValue(PickerCommandProperty);
            set => SetValue(PickerCommandProperty, value);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
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

        public PickerInputView()
            => InitializeComponent();
    }
}
