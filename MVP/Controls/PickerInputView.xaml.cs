using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class PickerInputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(PickerInputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(PickerInputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PickerInputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(PickerInputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty PickerCommandProperty =
            BindableProperty.Create(nameof(PickerCommand), typeof(ICommand), typeof(PickerInputView), null, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsRequiredProperty =
            BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(PickerInputView), false, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(PickerInputView), true, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ErrorsProperty =
            BindableProperty.Create(nameof(Errors), typeof(string), typeof(PickerInputView), null, defaultBindingMode: BindingMode.OneWay);

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

        public string Value
        {
            get => (string)GetValue(ValueProperty);
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
